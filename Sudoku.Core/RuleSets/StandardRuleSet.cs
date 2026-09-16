using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;

namespace Sudoku.Core.RuleSets;

public class StandardRuleSet : IRuleSet
{
    private readonly IReadOnlyList<IConstraint> _constraints;

    public StandardRuleSet(IReadOnlyList<IReadOnlyList<Cell>> groups)
    {
        _constraints = groups
            .Select(group => (IConstraint)new UniqueGroupConstraint(group))
            .ToList();
    }

    public bool IsSatisfied()
    {
        foreach (var constraint in _constraints)
        {
            if (!constraint.IsSatisfied()) return false;
        }
        return true;
    }
}