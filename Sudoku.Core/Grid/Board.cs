namespace Sudoku.Core.Grid;

public class Board
{
    public int Size { get; }

    private readonly Cell[,] _cells;
    private readonly List<Cell[]> _rows = [];
    private readonly List<Cell[]> _columns = [];
    private readonly List<Cell[]> _blocks = [];

    public IReadOnlyList<IReadOnlyList<Cell>> Rows { get; }
    public IReadOnlyList<IReadOnlyList<Cell>> Columns { get; }
    public IReadOnlyList<IReadOnlyList<Cell>> Blocks { get; }

    private IReadOnlyList<IReadOnlyList<Cell>>? _allGroups;
    public IReadOnlyList<IReadOnlyList<Cell>> AllGroups =>
        _allGroups ??= Rows
            .Cast<IReadOnlyList<Cell>>()
            .Concat(Columns.Cast<IReadOnlyList<Cell>>())
            .Concat(Blocks.Cast<IReadOnlyList<Cell>>())
            .ToList();

    public Cell this[int row, int col] => _cells[row, col];

    public Board(int size)
    {
        if (size <= 0)
            throw new ArgumentException("Size must be positive", nameof(size));
        if (size != 9)
            throw new ArgumentException("Size must be 9 rn", nameof(size)); // tmp

        Size = size;
        int boxSize = size / 3;

        _cells = new Cell[size, size];

        for (int row = 0; row < size; row++)
        {
            var rowCells = new Cell[size];
            for (int col = 0; col < size; col++)
            {
                rowCells[col] = new Cell();
                _cells[row, col] = rowCells[col];
            }

            _rows.Add(rowCells);
        }

        for (int col = 0; col < size; col++)
        {
            var colCells = new Cell[size];
            for (int row = 0; row < size; row++)
            {
                colCells[row] = _cells[row, col];
            }

            _columns.Add(colCells);
        }

        for (int blockRow = 0; blockRow < boxSize; blockRow++)
        {
            for (int blockCol = 0; blockCol < boxSize; blockCol++)
            {
                var blockCells = new Cell[boxSize * boxSize];
                int index = 0;

                for (int row = blockRow * boxSize; row < (blockRow + 1) * boxSize; row++)
                {
                    for (int col = blockCol * boxSize; col < (blockCol + 1) * boxSize; col++)
                    {
                        blockCells[index++] = _cells[row, col];
                    }
                }

                _blocks.Add(blockCells);
            }
        }

        Rows = _rows.Select(r => (IReadOnlyList<Cell>)r).ToList();
        Columns = _columns.Select(c => (IReadOnlyList<Cell>)c).ToList();
        Blocks = _blocks.Select(b => (IReadOnlyList<Cell>)b).ToList();
    }

    public Board(int[,] values) : this(9)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.GetLength(0) != Size || values.GetLength(1) != Size)
            throw new ArgumentException($"Values must be a {Size}x{Size} matrix.", nameof(values));

        for (var row = 0; row < Size; row++)
        {
            for (var col = 0; col < Size; col++)
            {
                var value = values[row, col];
                if (value < 0 || value > 9)
                    throw new ArgumentOutOfRangeException(nameof(values), value, "Cell values must be between 0 and 9.");

                _cells[row, col].Value = (byte)value;
            }
        }
    }

    public IEnumerable<(int row, int col, Cell cell)> EnumerateAllCells()
    {
        for (int row = 0; row < Size; row++)
            for (int col = 0; col < Size; col++)
                yield return (row, col, _cells[row, col]);
    }

    public IEnumerable<(int row, int col, Cell cell)> EnumerateFilledCells()
    {
        for (int row = 0; row < Size; row++)
            for (int col = 0; col < Size; col++)
                if (_cells[row, col].Value != 0)
                    yield return (row, col, _cells[row, col]);
    }

    public IEnumerable<(int row, int col, Cell cell)> EnumerateEmptyCells()
    {
        for (int row = 0; row < Size; row++)
            for (int col = 0; col < Size; col++)
                if (_cells[row, col].Value == 0)
                    yield return (row, col, _cells[row, col]);
    }

    public Board Clone()
    {
        var clone = new Board(Size);

        foreach (var (row, col, cell) in EnumerateAllCells())
        {
            clone[row, col].Value = cell.Value;
            clone[row, col].IsGiven = cell.IsGiven;
        }

        return clone;
    }

    public override string ToString()
    {
        var rows = new List<string>();

        for (int row = 0; row < Size; row++)
        {
            var values = new List<string>();
            for (int col = 0; col < Size; col++)
            {
                var value = _cells[row, col].Value;
                values.Add(value == 0 ? "." : value.ToString());
            }

            rows.Add(string.Join(" ", values));
        }

        return string.Join(Environment.NewLine, rows);
    }
}