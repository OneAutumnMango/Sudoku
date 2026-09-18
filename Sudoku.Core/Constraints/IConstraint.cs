using Sudoku.Core.Grid;

namespace Sudoku.Core.Constraints;

public interface IConstraint
{
    public IReadOnlyList<Cell> Cells { get; }
    public bool IsSatisfied();
    void ComputeAndFillCandidates();
}
