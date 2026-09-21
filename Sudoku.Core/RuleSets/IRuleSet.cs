using Sudoku.Core.Utils;
using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;

namespace Sudoku.Core.RuleSets;

public interface IRuleSet
{
    public Board Board { get; }
    public IEnumerable<IConstraint> GetConstraints();
    public Option<IConstraint> FindFirstUnsatisfiedConstraint();
    public void ComputeAndFillCandidates();
    public void UpdateCandidates(Cell changedCell);
}

public interface IStandardRuleSet : IRuleSet
{
    public IConstraint GetContainingRow(Cell cell);
    public IConstraint GetContainingColumn(Cell cell);
    public IConstraint GetContainingBox(Cell cell);
}
