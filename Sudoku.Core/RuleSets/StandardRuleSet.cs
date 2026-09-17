using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;
using Sudoku.Core.Utils;

namespace Sudoku.Core.RuleSets;

public sealed class StandardRuleSet : IRuleSet
{
    public Option<IConstraint> FindFirstUnsatisfiedConstraint(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        return board.AllGroups
            .Select(group => new UniqueGroupConstraint(group))
            .Cast<IConstraint>()
            .Where(constraint => !constraint.IsSatisfied())
            .Select(constraint => new Option<IConstraint>(constraint))
            .FirstOrDefault();
    }
}