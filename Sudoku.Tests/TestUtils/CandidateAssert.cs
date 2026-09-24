using Sudoku.Core;
using Sudoku.Core.Grid;

namespace Sudoku.Tests.TestUtils;

public static class CandidateAssert
{
    public static void HasCandidates(Puzzle puzzle, int row, int col, params byte[] expected)
    {
        Assert.Equal(expected, puzzle.Board[row, col].GetCandidates().ToArray());
    }

    /// <summary>Asserts the exact set of candidates removed since the snapshot, and that none were gained.</summary>
    public static void Eliminated(CandidateSnapshot snapshot, Board board, params Elimination[] expected)
    {
        Assert.Equal(Format(expected), Format(snapshot.RemovedSince(board)));
        Assert.Equal("", Format(snapshot.GainedSince(board)));
    }

    public static void NothingEliminated(CandidateSnapshot snapshot, Board board)
    {
        Eliminated(snapshot, board);
    }

    public static void NoFilledCellTouched(CandidateSnapshot snapshot, Board board)
    {
        var touched = snapshot.RemovedSince(board)
            .Concat(snapshot.GainedSince(board))
            .Where(e => snapshot.WasFilled(e.Row, e.Col) || board[e.Row, e.Col].Value != 0);

        Assert.Equal("", Format(touched));
    }

    /// <summary>For techniques whose return value counts individual candidate eliminations.</summary>
    public static void AppliedMatchesDiff(int applied, CandidateSnapshot snapshot, Board board)
    {
        Assert.Equal(snapshot.TotalRemoved(board), applied);
    }

    private static string Format(IEnumerable<Elimination> eliminations) =>
        string.Join(", ", eliminations.OrderBy(e => e.Row).ThenBy(e => e.Col).ThenBy(e => e.Candidate));
}
