namespace ConsoleApp;
public static class Program
{
    public static void Main()
    {
        using var input = new StreamReader(Console.OpenStandardInput());
        using var output = new StreamWriter(Console.OpenStandardOutput());
        ProcessData(input, output);
    }

    /// <summary>
    /// Выполнить обработку данных из входящего потока и записать результат в выводящий поток.
    /// </summary>
    /// <remarks> Этот метод вынесен отдельно для удобства тестирования.</remarks>
    public static void ProcessData(StreamReader input, StreamWriter output)
    {
        var dataSetCount = uint.Parse(input.ReadLine()!);
        for (var i = 0; i < dataSetCount; i++)
            output.WriteLine(ProcessDataSet(input));
    }

    /// <summary>
    /// Обработать один набор данных из входящего потока.
    /// </summary>
    static object ProcessDataSet(StreamReader input)
    {
        // читаем информацию об игре (размер поля и длину выигрышной последовательности.)
        var playInfo = PlayInfo.ReadPlayInfo(input);

        // читаем информацию об игровом поле
        var matrix = new char[playInfo.RowCount, playInfo.ColumnCount];
        for (var i = 0; i < playInfo.RowCount; i++)
        {
            var matrixLine = input.ReadLine()!;
            for (var j = 0; j < playInfo.ColumnCount; j++)
                matrix[i, j] = matrixLine[j];
        }

        var isPossibleWin = false;

        // проверяем наличие выигрышной комбинации в строках
        for (var i = 0; i < playInfo.RowCount; i++)
        {
            var rowWinState = new SequenceState(playInfo.WinLength);
            for (var j = 0; j < playInfo.ColumnCount; j++)
            {
                rowWinState.Update(matrix[i, j]);
                if (rowWinState.IsWin)
                {
                    return "NO";
                }
            }
            isPossibleWin |= rowWinState.IsPossibleWin;
        }

        // проверяем наличие выигрышной комбинации в столбцах
        for (var j = 0; j < playInfo.ColumnCount; j++)
        {
            var columnWinState = new SequenceState(playInfo.WinLength);
            for (var i = 0; i < playInfo.RowCount; i++)
            {
                columnWinState.Update(matrix[i, j]);
                if (columnWinState.IsWin)
                    return "NO";
            }
            isPossibleWin |= columnWinState.IsPossibleWin;
        }

        //проверяем наличие выигрышной комбинации в диагоналях с направлением вправо-вниз.
        for (var rowOffset = playInfo.WinLength - playInfo.RowCount; rowOffset <= playInfo.RowCount - playInfo.WinLength; rowOffset++)
        {
            var diagonalWinState = new SequenceState(playInfo.WinLength);
            for (var j = Math.Max(0, - rowOffset); j < playInfo.ColumnCount; j++)
            {
                var i = rowOffset + j;
                if (i < 0 || i >= playInfo.RowCount)
                    continue;
                diagonalWinState.Update(matrix[i, j]);
                if (diagonalWinState.IsWin)
                    return "NO";
            }
            isPossibleWin |= diagonalWinState.IsPossibleWin;
        }

        //проверяем наличие выигрышной комбинации в диагоналях с направлением вправо-вверх. /
        for (var columnOffset = playInfo.WinLength - playInfo.ColumnCount; columnOffset <= playInfo.ColumnCount - playInfo.WinLength; columnOffset++)
        {
            var diagonalWinState = new SequenceState(playInfo.WinLength);
            for (var i = Math.Max(0, - columnOffset); i < playInfo.RowCount; i++)
            {
                var j = columnOffset + i;
                if (j < 0 || j >= playInfo.ColumnCount)
                    continue;
                diagonalWinState.Update(matrix[i, j]);
                if (diagonalWinState.IsWin)
                    return "NO";
            }
            isPossibleWin |= diagonalWinState.IsPossibleWin;
        }

        return isPossibleWin ? "YES" : "NO";
    }

    /// <summary>
    /// Класс содержит информацию о текущей партии в игре (размер поля и длину выигрышной последовательности)
    /// </summary>
    class PlayInfo
    {
        /// <summary>
        /// Количество строк.
        /// </summary>
        public int RowCount { get; set; }

        /// <summary>
        /// Количество колонок.
        /// </summary>
        public int ColumnCount { get; set; }

        /// <summary>
        /// Длина выигрышной последовательности.
        /// </summary>
        public int WinLength { get; set; }

        /// <summary>
        /// Считать информацию о текущей партии в игре из входных данных.
        /// </summary>
        public static PlayInfo ReadPlayInfo(StreamReader input)
        {
            var playInfo = new PlayInfo();
            playInfo.WinLength = int.Parse(input.ReadLine()!);
            var sizeParts = input.ReadLine()!.Split(' ');
            playInfo.RowCount = int.Parse(sizeParts[0]);
            playInfo.ColumnCount = int.Parse(sizeParts[1]);
            return playInfo;
        }
    }

    /// <summary>
    /// Состояние выигрыша последовательности символов.
    /// </summary>
    class SequenceState
    {
        /// <summary>
        /// Предыдущий символ последовательности.
        /// </summary>
        char _prevChar;

        /// <summary>
        /// Длина последовательности Х или О которая претендует на выигрыш.
        /// </summary>
        int _winSequenceLength;

        /// <summary>
        /// Длина последовательности Х или . которая претендует на возможный выигрыш.
        /// </summary>
        int _possibleWinSequenceLength = 0;

        /// <summary>
        /// Последовательность Х или . которая претендует на возможный выигрыш содержит точку (пустую ячейку).
        /// </summary>
        bool _possibleWinSequenceContainsDot = false;

        /// <summary>
        /// Длина выигрышной последовательности.
        /// </summary>
        public int WinLength { get; private set; }

        /// <summary>
        /// Партия может быть выиграна если поставить один Х.
        /// </summary>
        public bool IsPossibleWin { get; private set; }

        /// <summary>
        /// Партия уже выиграна одним из игроков. Последовательность содержит необходимое количество О или Х.
        /// </summary>
        public bool IsWin { get; private set; }

        /// <summary>
        /// Конструктор.
        /// </summary>
        /// <param name="winLength">Длина выигрышной последовательности.</param>
        public SequenceState(int winLength)
        {
            WinLength = winLength;
        }

        /// <summary>
        /// Рассчитать и обновить состояние последовательности на основе нового символа в последовательности.
        /// </summary>
        /// <param name="newChar">Новый символ последовательности.</param>
        public void Update(char newChar)
        {
            // Рассчитываем возможную выигрышную последовательность когда новый символ - точка.
            if (!IsPossibleWin && newChar == '.')
            {
                if (!_possibleWinSequenceContainsDot)
                {
                    _possibleWinSequenceLength++;
                    _possibleWinSequenceContainsDot = true;
                }
                else
                {
                    _possibleWinSequenceLength = _winSequenceLength + 1;
                }
                if (_possibleWinSequenceLength >= WinLength)
                    IsPossibleWin = true;
            }
            // Рассчитываем выигрышную последовательность когда новый символ - О.
            else if (newChar == 'O')
            {
                _possibleWinSequenceLength = 0;
                _possibleWinSequenceContainsDot = false;
                if (_prevChar == 'O')
                    _winSequenceLength++;
                else
                    _winSequenceLength = 1;

                if (_winSequenceLength >= WinLength)
                    IsWin = true;
            }
            // Рассчитываем выигрышную и возможно выигрышную последовательность когда новый символ - Х.
            else
            {
                if (_prevChar == 'X')
                    _winSequenceLength++;
                else
                    _winSequenceLength = 1;

                if (_winSequenceLength >= WinLength)
                    IsWin = true;

                _possibleWinSequenceLength++;

                if (_possibleWinSequenceLength >= WinLength)
                    IsPossibleWin = true;
            }

            _prevChar = newChar;
        }
    }
}
