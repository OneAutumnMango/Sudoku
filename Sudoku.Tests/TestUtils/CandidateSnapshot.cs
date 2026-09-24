using Sudoku.Core.Grid;

namespace Sudoku.Tests.TestUtils;

public readonly record struct Elimination(int Row, int Col, byte Candidate)
{
    public override string ToString() => $"r{Row}c{Col}-{Candidate}";
}

public sealed class CandidateSnapshot
{
    private readonly ushort[,] _candidates = new ushort[9, 9];
    private readonly byte[,] _values = new byte[9, 9];

    private CandidateSnapshot(Board board)
    {
        foreach (var (row, col, cell) in board.EnumerateAllCells())
        {
            _candidates[row, col] = cell.Candidates;
            _values[row, col] = cell.Value;
        }
    }

    public static CandidateSnapshot Capture(Board board) => new(board);

    public IReadOnlyList<Elimination> RemovedSince(Board board) => Diff(board, gained: false);

    public IReadOnlyList<Elimination> GainedSince(Board board) => Diff(board, gained: true);

    public int TotalRemoved(Board board) => RemovedSince(board).Count;

    public IReadOnlyList<(int Row, int Col, byte Value)> PlacedSince(Board board)
    {
        var placed = new List<(int, int, byte)>();

        foreach (var (row, col, cell) in board.EnumerateAllCells())
        {
            if (_values[row, col] == 0 && cell.Value != 0)
                placed.Add((row, col, cell.Value));
        }

        return placed;
    }

    private IReadOnlyList<Elimination> Diff(Board board, bool gained)
    {
        var diff = new List<Elimination>();

        foreach (var (row, col, cell) in board.EnumerateAllCells())
        {
            var before = _candidates[row, col];
            var after = cell.Candidates;
            var changed = gained
                ? (ushort)(after & ~before)
                : (ushort)(before & ~after);

            for (byte candidate = 1; candidate <= 9; candidate++)
            {
                if ((changed & (1 << (candidate - 1))) != 0)
                    diff.Add(new Elimination(row, col, candidate));
            }
        }

        return diff;
    }

    public bool WasFilled(int row, int col) => _values[row, col] != 0;
}
