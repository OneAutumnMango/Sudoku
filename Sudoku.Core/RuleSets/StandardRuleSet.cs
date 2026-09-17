using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;
using Sudoku.Core.Utils;

namespace Sudoku.Core.RuleSets;

public sealed class StandardRuleSet : IRuleSet
{
    public Option<IConstraint> FindFirstUnsatisfiedConstraint(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        foreach (var group in board.AllGroups)
        {
            var constraint = new UniqueGroupConstraint(group);
            if (!constraint.IsSatisfied())
                return new Option<IConstraint>(constraint);
        }

        return Option<IConstraint>.None;
    }
}