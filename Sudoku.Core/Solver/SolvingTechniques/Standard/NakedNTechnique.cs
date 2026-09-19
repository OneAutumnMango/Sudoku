using System.Numerics;
using Sudoku.Core.Grid;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class NakedNTechnique : ISolvingTechnique
{
    private readonly int _n;

    public NakedNTechnique(int n)
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
            // all empty cells with 2 to N candidates
            var candidates = constraint.Cells
                .Where(cell => cell.Value == 0)
                .Where(cell =>
                {
                    var count = BitOperations.PopCount(cell.Candidates);
                    return count > 1 && count <= _n;
                })
                .ToList();

            foreach (var subset in GetCombinations(candidates, _n))
            {
                // union of candidates in the subset
                var subsetMask = subset.Aggregate(
                    (ushort)0,
                    (mask, cell) => (ushort)(mask | cell.Candidates));

                // if union count not N
                if (BitOperations.PopCount(subsetMask) != _n)
                    continue;

                foreach (var cell in constraint.Cells)
                {
                    if (cell.Value != 0 || subset.Contains(cell))
                        continue;

                    // remove union's candidates
                    var before = cell.Candidates;
                    cell.IntersectCandidates((ushort)~subsetMask);

                    if (cell.Candidates != before)
                        applied++;
                }
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