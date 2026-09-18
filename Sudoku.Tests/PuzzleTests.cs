using Sudoku.Core;
using Sudoku.Core.RuleSets;

namespace Sudoku.Tests;

public class PuzzleTests
{
    [Fact]
    public void Constructor_WithValues_LoadsBoard()
    {
        var values = new int[9, 9];
        values[0, 0] = 5;
        values[8, 8] = 9;
        var ruleset = new StandardRuleSet();

        var puzzle = new Puzzle(ruleset, values);

        Assert.Equal((byte)5, puzzle.Board[0, 0].Value);
        Assert.Equal((byte)9, puzzle.Board[8, 8].Value);
        Assert.Same(ruleset, puzzle.RuleSet);
    }
}
