using Sudoku.Core;
using Sudoku.Core.Grid;
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

    [Fact]
    public void UpdateCell_WhenValueChanges_ReturnsTrueAndRefreshesCandidates()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        var changed = puzzle.UpdateCell(0, 0, 5);

        Assert.True(changed);
        Assert.Equal((byte)5, puzzle.Board[0, 0].Value);
        Assert.DoesNotContain((byte)5, puzzle.Board[0, 1].GetCandidates());
    }

    [Fact]
    public void UpdateCell_WhenValueIsUnchanged_ReturnsFalse()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        Assert.False(puzzle.UpdateCell(0, 0, 0));
    }

    [Fact]
    public void UpdateCell_WhenCellIsCleared_RestoresItsCandidates()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        puzzle.UpdateCell(0, 0, 5);
        puzzle.UpdateCell(0, 0, 0);

        Assert.Equal(Cell.AllCandidates, puzzle.Board[0, 0].Candidates);
    }

    [Fact]
    public void RuleSet_KeepsStableConstraintsForBoard()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        var first = puzzle.RuleSet.GetConstraints(puzzle.Board).ToList();
        var second = puzzle.RuleSet.GetConstraints(puzzle.Board).ToList();

        Assert.Equal(first.Count, second.Count);
        Assert.All(first.Zip(second), pair => Assert.Same(pair.First, pair.Second));
    }
}
