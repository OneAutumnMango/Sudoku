using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;
using Sudoku.Core.Utils;

namespace Sudoku.Core.RuleSets;

public class StandardRuleSet : IRuleSet
{
    private IReadOnlyList<IConstraint> _constraints = new List<IConstraint>();
    private bool _isInitialized = false;

    public void Initialize(Board board)
    {
        _constraints = board.AllGroups
            .Select(group => new UniqueGroupConstraint(group))
            .Cast<IConstraint>()
            .ToList();
        _isInitialized = true;
    }

    public Option<IConstraint> FindFirstUnsatisfiedConstraint()
    {
        if (!_isInitialized)
            throw new InvalidOperationException("RuleSet must be initialized before use");
        
        foreach (var constraint in _constraints)
        {
            if (!constraint.IsSatisfied()) return new Option<IConstraint>(constraint);
        }
        return Option<IConstraint>.None;
    }
}