using Sudoku.Core.Utils;
using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;

namespace Sudoku.Core.RuleSets;

public interface IRuleSet
{
    public IEnumerable<IConstraint> GetConstraints(Board board);
    public Option<IConstraint> FindFirstUnsatisfiedConstraint(Board board);
    public void ComputeAndFillCandidates(Board board);
}
