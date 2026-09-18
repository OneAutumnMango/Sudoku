using Sudoku.Core;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver;

namespace Sudoku.Tests.Solver;

public class HumanlikeSolverTests
{
    [Fact]
    public void Solve_ReportsTechniqueAndDifficultyUsage()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        for (var col = 0; col < 8; col++)
            puzzle.SetCell(0, col, (byte)(col + 1));

        var solver = new HumanlikeSolver(puzzle);

        solver.Solve();

        var techniqueUsage = solver.GetTechniqueUsageCount();
        var difficultyUsage = solver.GetDifficultyUsageCount();

        Assert.Equal((byte)9, puzzle.Board[0, 8].Value);
        Assert.Equal(1, techniqueUsage["NakedSingleTechnique"]);
        Assert.Equal(1, difficultyUsage[Difficulty.Simple]);
        Assert.Equal(0, difficultyUsage[Difficulty.Easy]);
    }

    [Fact]
    public void Solve_WhenCalledAgainDoesNotChangeUsageCounts()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        for (var col = 0; col < 8; col++)
            puzzle.SetCell(0, col, (byte)(col + 1));

        var solver = new HumanlikeSolver(puzzle);
        solver.Solve();
        var techniqueUsage = solver.GetTechniqueUsageCount();
        var difficultyUsage = solver.GetDifficultyUsageCount();

        solver.Solve();

        Assert.Equal(techniqueUsage, solver.GetTechniqueUsageCount());
        Assert.Equal(difficultyUsage, solver.GetDifficultyUsageCount());
    }
}
