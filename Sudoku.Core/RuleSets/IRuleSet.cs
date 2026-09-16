using Sudoku.Core.Utils;
using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;

namespace Sudoku.Core.RuleSets;

public interface IRuleSet
{
    public void Initialize(Board board);
    public Option<IConstraint> FindFirstUnsatisfiedConstraint();
}