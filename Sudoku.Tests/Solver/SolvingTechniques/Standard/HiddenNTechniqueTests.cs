using Sudoku.Core;
using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver.SolvingTechniques.Standard;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class HiddenNTechniqueTests
{
    [Fact]
    public void TryApply_WhenHiddenPairExists_RemovesOtherCandidatesFromPairCells()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        KeepCandidates(puzzle.Board[0, 0], 1, 2, 3);
        KeepCandidates(puzzle.Board[0, 1], 1, 2, 4);
        for (var col = 2; col < 9; col++)
            KeepCandidates(puzzle.Board[0, col], 3, 4, 5, 6, 7, 8, 9);

        var changed = new HiddenNTechnique(2).TryApply(puzzle);

        Assert.True(changed > 0);
        Assert.Equal(new byte[] { 1, 2 }, puzzle.Board[0, 0].GetCandidates());
        Assert.Equal(new byte[] { 1, 2 }, puzzle.Board[0, 1].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenHiddenTripleExists_RemovesOtherCandidatesFromTripleCells()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        KeepCandidates(puzzle.Board[0, 0], 1, 2, 4);
        KeepCandidates(puzzle.Board[0, 1], 1, 3, 5);
        KeepCandidates(puzzle.Board[0, 2], 2, 3, 6);
        for (var col = 3; col < 9; col++)
            KeepCandidates(puzzle.Board[0, col], 4, 5, 6, 7, 8, 9);

        var changed = new HiddenNTechnique(3).TryApply(puzzle);

        Assert.True(changed > 0);
        Assert.Equal(new byte[] { 1, 2 }, puzzle.Board[0, 0].GetCandidates());
        Assert.Equal(new byte[] { 1, 3 }, puzzle.Board[0, 1].GetCandidates());
        Assert.Equal(new byte[] { 2, 3 }, puzzle.Board[0, 2].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenHiddenQuadExists_RemovesOtherCandidatesFromQuadCells()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        KeepCandidates(puzzle.Board[0, 0], 1, 2, 5);
        KeepCandidates(puzzle.Board[0, 1], 1, 3, 6);
        KeepCandidates(puzzle.Board[0, 2], 2, 4, 7);
        KeepCandidates(puzzle.Board[0, 3], 3, 4, 8);
        for (var col = 4; col < 9; col++)
            KeepCandidates(puzzle.Board[0, col], 5, 6, 7, 8, 9);

        var changed = new HiddenNTechnique(4).TryApply(puzzle);

        Assert.True(changed > 0);
        Assert.Equal(new byte[] { 1, 2 }, puzzle.Board[0, 0].GetCandidates());
        Assert.Equal(new byte[] { 1, 3 }, puzzle.Board[0, 1].GetCandidates());
        Assert.Equal(new byte[] { 2, 4 }, puzzle.Board[0, 2].GetCandidates());
        Assert.Equal(new byte[] { 3, 4 }, puzzle.Board[0, 3].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenNoHiddenSubsetExists_ReturnsZeroChanges()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        Assert.Equal(0, new HiddenNTechnique(2).TryApply(puzzle));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public void Constructor_WhenNIsOutsideSupportedRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new HiddenNTechnique(n));
    }

    private static void KeepCandidates(Cell cell, params byte[] candidates)
    {
        for (byte candidate = 1; candidate <= 9; candidate++)
        {
            if (!candidates.Contains(candidate))
                cell.RemoveCandidate(candidate);
        }
    }
}
