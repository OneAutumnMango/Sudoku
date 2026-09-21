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
        var ruleset = new StandardRuleSet(new Board(9));

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

        var first = puzzle.RuleSet.GetConstraints().ToList();
        var second = puzzle.RuleSet.GetConstraints().ToList();

        Assert.Equal(first.Count, second.Count);
        Assert.All(first.Zip(second), pair => Assert.Same(pair.First, pair.Second));
    }

    [Fact]
    public void UpdateCell_UpdatesOnlyRelatedPeers()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        puzzle.UpdateCell(0, 0, 5);

        Assert.DoesNotContain((byte)5, puzzle.Board[0, 1].GetCandidates());
        Assert.DoesNotContain((byte)5, puzzle.Board[1, 0].GetCandidates());
        Assert.DoesNotContain((byte)5, puzzle.Board[1, 1].GetCandidates());
        Assert.Contains((byte)5, puzzle.Board[1, 4].GetCandidates());
    }

    [Fact]
    public void UpdateCell_WhenClearedRestoresCandidateToPeers()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        puzzle.UpdateCell(0, 0, 5);
        puzzle.UpdateCell(0, 0, 0);

        Assert.Contains((byte)5, puzzle.Board[0, 1].GetCandidates());
        Assert.Contains((byte)5, puzzle.Board[1, 0].GetCandidates());
        Assert.Contains((byte)5, puzzle.Board[1, 1].GetCandidates());
    }

    [Fact]
    public void UpdateCell_CandidateMasksMatchFullRebuild()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        puzzle.UpdateCell(0, 0, 5);
        puzzle.UpdateCell(0, 1, 3);
        puzzle.UpdateCell(1, 0, 6);
        puzzle.UpdateCell(0, 1, 0);

        var beforeRebuild = puzzle.Board.EnumerateAllCells()
            .ToDictionary(item => (item.row, item.col), item => item.cell.Candidates);

        puzzle.RuleSet.ComputeAndFillCandidates();

        foreach (var (row, col, cell) in puzzle.Board.EnumerateAllCells())
            Assert.Equal(beforeRebuild[(row, col)], cell.Candidates);
    }

    [Fact]
    public void UpdateCell_LongMutationSequenceMatchesFreshCandidateBuild()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        puzzle.UpdateCell(0, 0, 5);
        puzzle.UpdateCell(0, 1, 3);
        puzzle.UpdateCell(1, 0, 6);
        puzzle.UpdateCell(4, 4, 9);
        puzzle.UpdateCell(8, 8, 8);
        puzzle.UpdateCell(0, 1, 0);
        puzzle.UpdateCell(1, 0, 0);
        puzzle.UpdateCell(3, 3, 5);
        puzzle.UpdateCell(0, 1, 3);
        puzzle.UpdateCell(4, 4, 0);
        puzzle.UpdateCell(7, 7, 4);

        var values = new int[9, 9];
        foreach (var (row, col, cell) in puzzle.Board.EnumerateAllCells())
            values[row, col] = cell.Value;

        var reference = new Puzzle(new StandardRuleSet(), values);

        foreach (var (row, col, cell) in puzzle.Board.EnumerateAllCells())
        {
            Assert.Equal(reference.Board[row, col].Value, cell.Value);
            Assert.True(
                reference.Board[row, col].Candidates == cell.Candidates,
                $"Candidate mismatch at row {row}, column {col}: "
                + $"expected {reference.Board[row, col].Candidates}, actual {cell.Candidates}.");
        }
    }

    [Fact]
    public void UpdateCell_DoesNotRestoreCandidatesRemovedByTechnique()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        puzzle.RemoveCandidate(0, 1, 5);
        puzzle.UpdateCell(0, 0, 4);
        puzzle.UpdateCell(0, 0, 0);
        puzzle.RuleSet.ComputeAndFillCandidates();

        Assert.DoesNotContain((byte)5, puzzle.Board[0, 1].GetCandidates());
    }
}
