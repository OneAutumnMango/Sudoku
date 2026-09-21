using Sudoku.Core.Constraints;
using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public class FishTechnique(int n) : ISolvingTechnique
{
    public Difficulty Difficulty { get; } = Difficulty.Advanced;
    private readonly int _n = n;

    public int TryApply(Puzzle puzzle)
    {
        var applied = 0;

        if (puzzle.RuleSet is not IStandardRuleSet ruleSet)
            throw new InvalidOperationException("FishTechniques must use IStandardRuleSet.");



        var rowCandidates = GetConstraintsAndCandidatesByValue(
            puzzle,
            ruleSet.RowConstraints);

        var columnCandidates = GetConstraintsAndCandidatesByValue(
            puzzle,
            ruleSet.ColumnConstraints);

        for (int value = 1; value <= puzzle.Board.Size; value++)
        {
            if (!rowCandidates.TryGetValue(value, out var rowList))
                continue;


            // check for row based fish n+ rows? <n? idk

            if (!columnCandidates.TryGetValue(value, out var columnList))
                continue;


            // check for column based fish
        }

        return applied;
    }

    private Dictionary<int, List<(IConstraint constraint, List<Cell> cells)>>
        GetConstraintsAndCandidatesByValue(
            Puzzle puzzle,
            IEnumerable<IConstraint> constraints)
    {
        var result = new Dictionary<int, List<(IConstraint constraint, List<Cell> cells)>>();

        foreach (var constraint in constraints)
        {
            var candidatesByValue = CountNCandidatesByValue(puzzle, constraint);

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

    private Dictionary<int, List<Cell>> CountNCandidatesByValue(Puzzle puzzle, IConstraint constraint)
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

        return candidateCells
            .Select((cells, val) => (val, cells))
            .Where(x => x.cells.Count == _n)
            .ToDictionary(x => x.val, x => x.cells);
    }
}