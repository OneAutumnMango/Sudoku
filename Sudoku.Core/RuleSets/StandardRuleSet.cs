using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;
using Sudoku.Core.Utils;

namespace Sudoku.Core.RuleSets;

public sealed class StandardRuleSet : IRuleSet
{
    public IEnumerable<IConstraint> GetConstraints(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        foreach (var group in board.AllGroups)
            yield return new UniqueGroupConstraint(group);
    }

    public Option<IConstraint> FindFirstUnsatisfiedConstraint(Board board)
    {
        foreach (var constraint in GetConstraints(board))
        {
            if (!constraint.IsSatisfied())
                return new Option<IConstraint>(constraint);
        }

        return Option<IConstraint>.None;
    }

    public void ComputeAndFillCandidates(Board board)
    {
        foreach (var (_, _, cell) in board.EnumerateAllCells())
        {
            if (cell.Value == 0)
                cell.Candidates = Cell.AllCandidates;
        }

        foreach (var constraint in GetConstraints(board))
            constraint.ComputeAndFillCandidates();
    }
}
