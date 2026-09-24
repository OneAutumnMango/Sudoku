using Sudoku.Core;
using Sudoku.Core.RuleSets;

namespace Sudoku.Tests.TestUtils;

public static class PuzzleFactory
{
    private const string SolvedGrid =
        "123456789" +
        "456789123" +
        "789123456" +
        "234567891" +
        "567891234" +
        "891234567" +
        "345678912" +
        "678912345" +
        "912345678";

    public static Puzzle Empty() => new(new StandardRuleSet());

    public static Puzzle FromGrid(int[,] values) => new(new StandardRuleSet(), values);

    public static Puzzle Solved() => FromString(SolvedGrid);

    /// <summary>Parses a 81 cell grid, ignoring whitespace. '.' and '0' mean empty.</summary>
    public static Puzzle FromString(string grid)
    {
        ArgumentNullException.ThrowIfNull(grid);

        var cells = grid.Where(c => !char.IsWhiteSpace(c)).ToArray();

        if (cells.Length != 81)
            throw new ArgumentException($"Expected 81 cells but got {cells.Length}.", nameof(grid));

        var values = new int[9, 9];

        for (var index = 0; index < cells.Length; index++)
        {
            var c = cells[index];

            if (c != '.' && (c < '0' || c > '9'))
                throw new ArgumentException($"Unexpected character '{c}' at index {index}.", nameof(grid));

            values[index / 9, index % 9] = c == '.' ? 0 : c - '0';
        }

        return FromGrid(values);
    }

    /// <summary>Narrows a cell to exactly the given candidates.</summary>
    public static Puzzle WithCandidates(this Puzzle puzzle, int row, int col, params byte[] candidates)
    {
        ushort mask = 0;

        foreach (var candidate in candidates)
            mask |= (ushort)(1 << (candidate - 1));

        puzzle.Board[row, col].IntersectCandidates(mask);
        return puzzle;
    }

    /// <summary>Copies values and eliminated candidates into a fresh puzzle.</summary>
    public static Puzzle Clone(Puzzle puzzle)
    {
        var values = new int[9, 9];

        foreach (var (row, col, cell) in puzzle.Board.EnumerateAllCells())
            values[row, col] = cell.Value;

        var clone = FromGrid(values);

        foreach (var (row, col, cell) in puzzle.Board.EnumerateEmptyCells())
            clone.Board[row, col].IntersectCandidates(cell.Candidates);

        return clone;
    }
}
