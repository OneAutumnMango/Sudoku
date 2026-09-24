using Sudoku.Core;

namespace Sudoku.Tests.TestUtils;

public static class SolutionInvariant
{
    /// <summary>
    /// Asserts nothing has ruled out the true solution: every placed value matches it and every
    /// empty cell still holds its solution digit as a candidate.
    /// </summary>
    public static void AssertPreserved(Puzzle puzzle, string solution, string context = "")
    {
        var violations = new List<string>();

        foreach (var (row, col, cell) in puzzle.Board.EnumerateAllCells())
        {
            var expected = (byte)(solution[row * 9 + col] - '0');

            if (cell.Value != 0 && cell.Value != expected)
                violations.Add($"r{row}c{col} holds {cell.Value} but solution is {expected}");
            else if (cell.Value == 0 && !cell.HasCandidate(expected))
                violations.Add($"r{row}c{col} lost solution candidate {expected}");
        }

        Assert.True(
            violations.Count == 0,
            $"{context}{string.Concat(violations.Select(v => $"\n  {v}"))}");
    }
}
