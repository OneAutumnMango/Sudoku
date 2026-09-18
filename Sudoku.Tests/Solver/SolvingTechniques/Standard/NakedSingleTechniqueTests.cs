using Sudoku.Core;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver;
using Sudoku.Core.Solver.SolvingTechniques.Standard;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class NakedSingleTechniqueTests
{
    [Fact]
    public void TryApply_WhenBoardHasSingleCandidate_ReturnsOneChange()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        for (var col = 0; col < 8; col++)
            puzzle.SetCell(0, col, (byte)(col + 1));

        var technique = new NakedSingleTechnique();

        var applied = technique.TryApply(puzzle);

        Assert.Equal(1, applied);
        Assert.Equal((byte)9, puzzle.Board[0, 8].Value);
        Assert.Equal(Difficulty.Simple, technique.Difficulty);
    }

    [Fact]
    public void TryApply_WhenBoardHasTwoSingles_ReturnsTwoChanges()
    {
        var puzzle = CreatePuzzle(
            new int[,]
            {
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
                { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 1 },
                { 5, 6, 7, 8, 9, 1, 2, 3, 4 },
                { 8, 9, 1, 2, 3, 4, 5, 6, 7 },
                { 3, 4, 5, 6, 7, 8, 9, 1, 2 },
                { 6, 7, 8, 9, 1, 2, 3, 4, 5 },
                { 9, 1, 2, 3, 4, 5, 6, 7, 8 }
            });

        puzzle.ClearCell(0, 8);
        puzzle.ClearCell(1, 5);

        var applied = new NakedSingleTechnique().TryApply(puzzle);

        Assert.Equal(2, applied);
        Assert.Equal((byte)9, puzzle.Board[0, 8].Value);
        Assert.Equal((byte)9, puzzle.Board[1, 5].Value);
    }

    [Fact]
    public void TryApply_WhenCalledAfterApplyingSingle_ReturnsZeroChanges()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        for (var col = 0; col < 8; col++)
            puzzle.SetCell(0, col, (byte)(col + 1));

        var technique = new NakedSingleTechnique();

        Assert.Equal(1, technique.TryApply(puzzle));
        Assert.Equal(0, technique.TryApply(puzzle));
        Assert.Equal((byte)9, puzzle.Board[0, 8].Value);
    }

    [Fact]
    public void TryApply_WhenBoardHasNoSingleCandidate_ReturnsZeroChanges()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var technique = new NakedSingleTechnique();

        var applied = technique.TryApply(puzzle);

        Assert.Equal(0, applied);
        Assert.All(puzzle.Board.EnumerateEmptyCells(), cell => Assert.NotEmpty(cell.cell.GetCandidates()));
    }

    [Fact]
    public void TryApply_WhenTwoSinglesBecomeConflicting_DoesNotCreateInvalidBoard()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        for (var col = 0; col < 7; col++)
            puzzle.SetCell(0, col, (byte)(col + 1));

        puzzle.SetCell(3, 7, 8);

        var applied = new NakedSingleTechnique().TryApply(puzzle);

        Assert.Equal(1, applied);
        Assert.Equal((byte)9, puzzle.Board[0, 7].Value);
        Assert.Equal((byte)0, puzzle.Board[0, 8].Value);

        Assert.Equal(1, new NakedSingleTechnique().TryApply(puzzle));
        Assert.Equal((byte)8, puzzle.Board[0, 8].Value);

        Assert.True(
            puzzle.RuleSet.FindFirstUnsatisfiedConstraint(puzzle.Board).IsNone,
            "NakedSingle created an invalid board by assigning conflicting singles from stale candidates.");
    }

    private static Puzzle CreatePuzzle(int[,] values)
    {
        return new Puzzle(new StandardRuleSet(), values);
    }
}
