using Sudoku.Core;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver;
using Sudoku.Core.Solver.SolvingTechniques.Standard;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class HiddenSingleTechniqueTests
{
    [Fact]
    public void TryApply_WhenCandidateAppearsOnceInGroup_SetsValueAndReturnsTrue()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var board = puzzle.Board;

        for (var col = 0; col < 8; col++)
            board[0, col].Value = (byte)(col + 1);

        var technique = new HiddenSingleTechnique();

        var applied = technique.TryApply(puzzle);

        Assert.True(applied);
        Assert.Equal((byte)9, board[0, 8].Value);
        Assert.Equal(Difficulty.Easy, technique.Difficulty);
    }

    [Fact]
    public void TryApply_WhenCalledAfterApplyingSingle_ReturnsFalse()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        for (var col = 0; col < 8; col++)
            puzzle.SetCell(0, col, (byte)(col + 1));

        var technique = new HiddenSingleTechnique();

        Assert.True(technique.TryApply(puzzle));
        Assert.False(technique.TryApply(puzzle));
        Assert.Equal((byte)9, puzzle.Board[0, 8].Value);
    }

    [Fact]
    public void TryApply_WhenNoCandidateIsUniqueInGroup_ReturnsFalse()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var technique = new HiddenSingleTechnique();

        var applied = technique.TryApply(puzzle);

        Assert.False(applied);
    }

    [Fact]
    public void TryApply_WhenCandidatesChangeBetweenGroups_DoesNotCreateDuplicates()
    {
        var puzzle = CreatePuzzle(
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

        var applied = new HiddenSingleTechnique().TryApply(puzzle);

        Assert.True(applied);
        Assert.Equal((byte)2, puzzle.Board[1, 6].Value);
        Assert.Equal((byte)1, puzzle.Board[3, 5].Value);
        Assert.Equal((byte)2, puzzle.Board[5, 4].Value);
        Assert.Equal((byte)2, puzzle.Board[6, 7].Value);
        Assert.True(puzzle.RuleSet.FindFirstUnsatisfiedConstraint(puzzle.Board).IsNone);
    }

    private static Puzzle CreatePuzzle(int[,] values)
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        for (var row = 0; row < values.GetLength(0); row++)
            for (var col = 0; col < values.GetLength(1); col++)
                puzzle.SetCell(row, col, (byte)values[row, col]);

        return puzzle;
    }
}
