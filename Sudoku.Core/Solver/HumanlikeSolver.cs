using Sudoku.Core.Solver.SolvingTechniques;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Core.Utils;

namespace Sudoku.Core.Solver;

public class HumanlikeSolver(Puzzle puzzle)
{
    private readonly Puzzle _puzzle = puzzle;

    private readonly Dictionary<ISolvingTechnique, int> _techniqueUsageCount = [];

    public Option<Difficulty> Solve()
    {
        ISolvingTechnique[] techniques =
        [
            new NakedSingleTechnique(),
            new NakedPairTechnique(),
            new NakedTripleTechnique(),

            new HiddenSingleTechnique(),
            new HiddenPairTechnique(),
            new HiddenTripleTechnique(),

            new PointingTechnique(),

            new BoxLineReductionTechnique(),

            // x wing, y wing
        ];

        foreach (var difficulty in Enum.GetValues<Difficulty>())
        {
            if (difficulty == Difficulty.Unknown)
                continue;

            var changed = 1;
            while (changed > 0)
            {
                changed = 0;
                foreach (var technique in techniques.Where(technique => technique.Difficulty <= difficulty))
                {
                    var applied = technique.TryApply(_puzzle);
                    _techniqueUsageCount[technique] =
                        _techniqueUsageCount.GetValueOrDefault(technique, 0) + applied;
                    changed += applied;
                }
            }
        }

        // puzzle diff is highest difficulty used
        if (IsSolved())
            return new Option<Difficulty>(
                GetDifficultyUsageCount()
                    .Where(kv => kv.Value > 0)
                    .MaxBy(kv => kv.Key)
                    .Key
            );

        return Option<Difficulty>.None;
    }

    public bool IsSolved()
    {
        // check if all cells filled
        if (_puzzle.Board.EnumerateEmptyCells().Any())
            return false;
        return _puzzle.RuleSet.FindFirstUnsatisfiedConstraint().IsNone;
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