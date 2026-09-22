using Sudoku.Core.Constraints;
using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public class FishTechnique(int n) : ISolvingTechnique
{
    public Difficulty Difficulty { get; } = Difficulty.Unknown;
    private readonly int _n = n;

    public int TryApply(Puzzle puzzle)
    {
        var applied = 0;

        if (puzzle.RuleSet is not IStandardRuleSet ruleSet)
            throw new InvalidOperationException("FishTechniques must use IStandardRuleSet.");


        // Find rows and columns containing each candidate in two to _n cells.
        var rowCandidates = GetConstraintsAndCellsByCandidateValue(
            puzzle,
            ruleSet.RowConstraints);
        var columnCandidates = GetConstraintsAndCellsByCandidateValue(
            puzzle,
            ruleSet.ColumnConstraints);

        applied += ApplyFish(
            puzzle,
            rowCandidates,
            ruleSet.GetContainingColumn);
        applied += ApplyFish(
            puzzle,
            columnCandidates,
            ruleSet.GetContainingRow);

        return applied;
    }

    private int ApplyFish(
        Puzzle puzzle,
        Dictionary<int, List<(IConstraint constraint, List<Cell> cells)>> baseCandidates,
        Func<Cell, IConstraint> getPerpendicularConstraint)
    {
        var changes = 0;

        for (var value = 1; value <= puzzle.Board.Size; value++)
        {
            if (!baseCandidates.TryGetValue(value, out var baseList))
                continue;

            foreach (var baseGroupCombination in GetCombinations(baseList, _n))
            {
                // The candidate cells in those groups must occupy exactly _n perpendicular groups.
                var perpendicularGroups = baseGroupCombination
                    .SelectMany(group => group.cells)
                    .Select(getPerpendicularConstraint)
                    .Distinct()
                    .ToList();

                if (perpendicularGroups.Count != _n)
                    continue;

                // Remove the candidate from perpendicular groups outside the base groups.
                foreach (var group in perpendicularGroups)
                {
                    foreach (var cell in group.Cells)
                    {
                        if (baseGroupCombination.Any(baseGroup => baseGroup.constraint.Cells.Contains(cell)))
                            continue;

                        if (!cell.HasCandidate((byte)value))
                            continue;

                        puzzle.RemoveCandidate(cell, (byte)value);
                        changes++;
                    }
                }
            }
        }

        return changes;
    }

    private Dictionary<int, List<(IConstraint constraint, List<Cell> cells)>>
        GetConstraintsAndCellsByCandidateValue(
            Puzzle puzzle,
            IEnumerable<IConstraint> constraints)
    {
        var result = new Dictionary<int, List<(IConstraint constraint, List<Cell> cells)>>();

        // Group candidate cells by value for each constraint.
        foreach (var constraint in constraints)
        {
            var candidatesByValue = CountTwoToNCandidatesByValue(puzzle, constraint);

            foreach (var (value, cells) in candidatesByValue)
            {
                if (!result.TryGetValue(value, out var list))  // create if doesnt exist
                {
                    list = [];
                    result[value] = list;
                }

                list.Add((constraint, cells));
            }
        }
        return result;
    }

    private Dictionary<int, List<Cell>> CountTwoToNCandidatesByValue(Puzzle puzzle, IConstraint constraint)
    {
        var candidateCells = Enumerable
            .Range(0, puzzle.Board.Size + 1)
            .Select(_ => new List<Cell>())
            .ToList();

        foreach (var cell in constraint.Cells)
        {
            if (cell.Value != 0)
                continue;

            foreach (var cand in cell.GetCandidates())
                candidateCells[cand].Add(cell);
        }

        // A fish requires at least two and at most _n candidate cells in a group.
        return candidateCells
            .Select((cells, val) => (val, cells))
            .Where(x => x.cells.Count >= 2 && x.cells.Count <= _n)  // at least [2,n] cells
            .ToDictionary(x => x.val, x => x.cells);
    }

    private static IEnumerable<IReadOnlyList<T>> GetCombinations<T>(
        IReadOnlyList<T> items,
        int count,
        int startIndex = 0,
        List<T>? selected = null)
    {
        selected ??= [];

        if (selected.Count == count)
        {
            yield return selected.ToList();
            yield break;
        }

        for (var index = startIndex; index <= items.Count - (count - selected.Count); index++)
        {
            selected.Add(items[index]);

            foreach (var combination in GetCombinations(items, count, index + 1, selected))
                yield return combination;

            selected.RemoveAt(selected.Count - 1);
        }
    }
}