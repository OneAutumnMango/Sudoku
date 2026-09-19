using Sudoku.Core;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver.SolvingTechniques.Standard;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class NakedNTechniqueTests
{
    [Fact]
    public void TryApply_WhenNakedPairExists_RemovesPairCandidatesFromOtherCells()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        KeepCandidates(puzzle.Board[0, 0], 1, 2);
        KeepCandidates(puzzle.Board[0, 1], 1, 2);
        KeepCandidates(puzzle.Board[0, 2], 1, 2, 3);

        var changed = new NakedNTechnique(2).TryApply(puzzle);

        Assert.Equal(13, changed);
        Assert.Equal(new byte[] { 3 }, puzzle.Board[0, 2].GetCandidates());
        Assert.Equal(new byte[] { 1, 2 }, puzzle.Board[0, 0].GetCandidates());
        Assert.Equal(new byte[] { 1, 2 }, puzzle.Board[0, 1].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenNakedTripleExists_RemovesTripleCandidatesFromOtherCells()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        KeepCandidates(puzzle.Board[0, 0], 1, 2);
        KeepCandidates(puzzle.Board[0, 1], 1, 3);
        KeepCandidates(puzzle.Board[0, 2], 2, 3);
        KeepCandidates(puzzle.Board[0, 3], 1, 2, 3, 4);

        var changed = new NakedNTechnique(3).TryApply(puzzle);

        Assert.Equal(12, changed);
        Assert.Equal(new byte[] { 4 }, puzzle.Board[0, 3].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenNakedQuadExists_RemovesQuadCandidatesFromOtherCells()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        KeepCandidates(puzzle.Board[0, 0], 1, 2);
        KeepCandidates(puzzle.Board[0, 1], 1, 3);
        KeepCandidates(puzzle.Board[0, 2], 2, 4);
        KeepCandidates(puzzle.Board[0, 3], 3, 4);
        KeepCandidates(puzzle.Board[0, 4], 1, 2, 3, 4, 5);

        var changed = new NakedNTechnique(4).TryApply(puzzle);

        Assert.Equal(5, changed);
        Assert.Equal(new byte[] { 5 }, puzzle.Board[0, 4].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenNoNakedSubsetExists_ReturnsZeroChanges()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        KeepCandidates(puzzle.Board[0, 0], 1, 2);
        KeepCandidates(puzzle.Board[0, 1], 1, 3);

        Assert.Equal(0, new NakedNTechnique(2).TryApply(puzzle));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public void Constructor_WhenNIsOutsideSupportedRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NakedNTechnique(n));
    }

    [Fact]
    public void NakedPairTechnique_UsesNakedPairLogic()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        KeepCandidates(puzzle.Board[0, 0], 1, 2);
        KeepCandidates(puzzle.Board[0, 1], 1, 2);
        KeepCandidates(puzzle.Board[0, 2], 1, 2, 3);

        var changed = new NakedPairTechnique().TryApply(puzzle);

        Assert.Equal(13, changed);
        Assert.Equal(new byte[] { 3 }, puzzle.Board[0, 2].GetCandidates());
    }

    private static void KeepCandidates(Sudoku.Core.Grid.Cell cell, params byte[] candidates)
    {
        for (byte candidate = 1; candidate <= 9; candidate++)
        {
            if (!candidates.Contains(candidate))
                cell.RemoveCandidate(candidate);
        }
    }
}