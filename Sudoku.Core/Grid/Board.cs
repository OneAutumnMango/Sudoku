namespace Sudoku.Core.Grid;

public class Board
{
    private readonly Cell[,] _cells;
    private readonly List<Cell[]> _rows = new();
    private readonly List<Cell[]> _columns = new();
    private readonly List<Cell[]> _blocks = new();

    public IReadOnlyList<IReadOnlyList<Cell>> Rows { get; }
    public IReadOnlyList<IReadOnlyList<Cell>> Columns { get; }
    public IReadOnlyList<IReadOnlyList<Cell>> Blocks { get; }

    public Cell this[int row, int col] => _cells[row, col];

    public Board(int size)
    {
        // if (size <= 0) throw new ArgumentException("Size must be positive", nameof(size));
        if (size != 9)
            throw new ArgumentException("Size must be 9 rn", nameof(size)); // tmp
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
}