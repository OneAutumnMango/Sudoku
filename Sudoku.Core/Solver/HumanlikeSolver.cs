using Sudoku.Core.Solver.SolvingTechniques;

namespace Sudoku.Core.Solver;

public class HumanlikeSolver
{
    private readonly Puzzle _puzzle;

    private readonly Dictionary<ISolvingTechnique, int> _techniqueUsageCount = [];

    private void RecordTechniqueUsage(ISolvingTechnique technique)
    {
        _techniqueUsageCount[technique] =
            _techniqueUsageCount.GetValueOrDefault(technique, 0) + 1;
    }

    public HumanlikeSolver(Puzzle puzzle)
    {
        _puzzle = puzzle;
    }
}