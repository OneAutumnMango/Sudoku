using Sudoku.Core;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver;
using Sudoku.Core.Solver.SolvingTechniques.Standard;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class HiddenSingleTechniqueTests
{
    [Fact]
    public void TryApply_WhenCandidateAppearsOnceInGroup_ReturnsOneChange()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        SetCandidates(puzzle.Board[0, 0], 1, 2, 3);
        for (var col = 1; col < 9; col++)
            SetCandidates(puzzle.Board[0, col], 2, 3, 4, 5, 6, 7, 8, 9);

        var technique = new HiddenSingleTechnique();

        var applied = technique.TryApply(puzzle);

        Assert.Equal(1, applied);
        Assert.Equal(new byte[] { 1 }, puzzle.Board[0, 0].GetCandidates());
        Assert.Equal(Difficulty.Easy, technique.Difficulty);
    }

    [Fact]
    public void TryApply_WhenCalledAfterApplyingSingle_ReturnsZeroChanges()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        SetCandidates(puzzle.Board[0, 0], 1, 2, 3);
        for (var col = 1; col < 9; col++)
            SetCandidates(puzzle.Board[0, col], 2, 3, 4, 5, 6, 7, 8, 9);

        var technique = new HiddenSingleTechnique();

        Assert.Equal(1, technique.TryApply(puzzle));
        Assert.Equal(0, technique.TryApply(puzzle));
        Assert.Equal(new byte[] { 1 }, puzzle.Board[0, 0].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenNoCandidateIsUniqueInGroup_ReturnsZeroChanges()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var technique = new HiddenSingleTechnique();

        var applied = technique.TryApply(puzzle);

        Assert.Equal(0, applied);
    }

    [Fact]
    public void TryApply_WhenCandidatesChangeBetweenGroups_DoesNotCreateDuplicates()
    {
        var puzzle = CreateMultipleHiddenSinglePuzzle();

        var applied = new HiddenSingleTechnique().TryApply(puzzle);

        Assert.NotEqual(0, applied);
    }

    [Fact]
    public void TryApply_WhenMultipleHiddenSinglesExist_ReturnsNumberOfChangedCells()
    {
        var puzzle = CreateMultipleHiddenSinglePuzzle();

        var applied = new HiddenSingleTechnique().TryApply(puzzle);

        Assert.NotEqual(0, applied);
    }

    private static Puzzle CreateMultipleHiddenSinglePuzzle()
    {
        return CreatePuzzle(
            new int[,]
            {
                { 5, 0, 0, 0, 0, 8, 3, 4, 0 },
                { 0, 0, 0, 0, 0, 0, 0, 9, 0 },
                { 0, 0, 4, 2, 0, 0, 0, 6, 0 },
                { 0, 0, 0, 8, 3, 0, 0, 0, 0 },
                { 0, 9, 0, 5, 7, 4, 0, 0, 2 },
                { 0, 1, 0, 0, 0, 0, 0, 0, 0 },
                { 3, 0, 0, 0, 1, 5, 0, 0, 0 },
                { 0, 0, 8, 0, 0, 2, 0, 0, 4 },
                { 2, 0, 1, 0, 0, 0, 0, 0, 0 }
            });
    }

    private static Puzzle CreatePuzzle(int[,] values)
    {
        return new Puzzle(new StandardRuleSet(), values);
    }

    private static void SetCandidates(Sudoku.Core.Grid.Cell cell, params byte[] candidates)
    {
        for (byte candidate = 1; candidate <= 9; candidate++)
        {
            if (!candidates.Contains(candidate))
                cell.RemoveCandidate(candidate);
        }
    }
}
