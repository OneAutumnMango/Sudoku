using System.Numerics;
using Sudoku.Core.Grid;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class HiddenNTechnique : ISolvingTechnique
{
    private readonly int _n;

    public HiddenNTechnique(int n)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(n, 2);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(n, 4);
        _n = n;
    }

    public Difficulty Difficulty => Difficulty.Unknown;

    public int TryApply(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        var applied = 0;

        foreach (var constraint in puzzle.RuleSet.GetConstraints(puzzle.Board))
        {
            // all empty cells with 2+ candidates
            var candidates = constraint.Cells
                .Where(cell => cell.Value == 0)
                .Where(cell => BitOperations.PopCount(cell.Candidates) > 1)  // removing this would work for N=1 too i think but then naked single would be applying the change
                .ToList();

            foreach (var subset in GetCombinations(candidates, _n))
            {
                // union of candidates in the subset
                var unionMask = subset.Aggregate(
                    (ushort)0,
                    (mask, cell) => (ushort)(mask | cell.Candidates));

                if (BitOperations.PopCount(unionMask) == _n)  // nakedN found skipping
                    continue;

                foreach (var cell in constraint.Cells)
                {
                    if (cell.Value != 0 || subset.Contains(cell))
                        continue;

                    unionMask &= (ushort)~cell.Candidates;
                }

                if (BitOperations.PopCount(unionMask) != _n)
                    continue;

                // safeguard to ensure mask is correct
                if (subset.Any(cell => (cell.Candidates & unionMask) == 0))
                    continue;

                // remove candidates not in the union mask from each cell in the subset
                foreach (var cell in subset)
                {
                    // cant set cell candidates so have to remove the others
                    var candsToRemove = (ushort)(cell.Candidates & ~unionMask);
                    puzzle.RemoveCandidates(cell, candsToRemove);
                }

                applied++;
            }
        }

        return applied;
    }

    /**
     * Generates all combinations of a given size from a list of cells.
     *
     * @param cells The list of cells to generate combinations from.
     * @param count The size of each combination.
     * @param startIndex The starting index for the combination generation.
     * @param selected The currently selected cells for the combination.
     * @return An iterator of all cell combinations.
     */
    private static IEnumerable<IReadOnlyList<Cell>> GetCombinations(
        IReadOnlyList<Cell> cells,
        int count,
        int startIndex = 0,
        List<Cell>? selected = null)
    {
        selected ??= [];

        if (selected.Count == count)
        {
            yield return selected.ToList();
            yield break;
        }

        for (var index = startIndex; index <= cells.Count - (count - selected.Count); index++)
        {
            selected.Add(cells[index]);

            foreach (var combination in GetCombinations(cells, count, index + 1, selected))
                yield return combination;

            selected.RemoveAt(selected.Count - 1);
        }
    }
}