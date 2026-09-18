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
    public void TryApply_WhenNoCandidateIsUniqueInGroup_ReturnsFalse()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var technique = new HiddenSingleTechnique();

        var applied = technique.TryApply(puzzle);

        Assert.False(applied);
    }
}
