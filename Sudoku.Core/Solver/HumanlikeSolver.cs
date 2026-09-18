using Sudoku.Core.Solver.SolvingTechniques;
using Sudoku.Core.Solver.SolvingTechniques.Standard;

namespace Sudoku.Core.Solver;

public class HumanlikeSolver(Puzzle puzzle)
{
    private readonly Puzzle _puzzle = puzzle;

    private readonly Dictionary<ISolvingTechnique, int> _techniqueUsageCount = [];

    public void Solve()
    {
        ISolvingTechnique[] techniques =
        [
            new NakedSingleTechnique(),
            new HiddenSingleTechnique()
        ];

        var changed = 1;
        while (changed > 0)
        {
            changed = 0;
            foreach (var technique in techniques)
            {
                var applied = technique.TryApply(_puzzle);
                _techniqueUsageCount[technique] =
                    _techniqueUsageCount.GetValueOrDefault(technique, 0) + applied;
                changed += applied;
            }
        }
    }

    public IReadOnlyDictionary<Difficulty, int> GetDifficultyUsageCount()
    {
        return Enum.GetValues<Difficulty>()
            .ToDictionary(
                difficulty => difficulty,
                difficulty => _techniqueUsageCount
                    .Where(usage => usage.Key.Difficulty == difficulty)
                    .Sum(usage => usage.Value));
    }

    public IReadOnlyDictionary<string, int> GetTechniqueUsageCount()
    {
        return _techniqueUsageCount
            .GroupBy(usage => usage.Key.Name)
            .ToDictionary(
                usage => usage.Key,
                usage => usage.Sum(item => item.Value));
    }
}