using Sudoku.Core.Grid;

namespace Sudoku.Core.Constraints;

public interface IConstraint
{
    public IReadOnlyList<Cell> Cells { get; }
    public bool IsSatisfied();
    ushort GetAllowedCandidates(Cell cell) => Cell.AllCandidates;
    void ComputeAndFillCandidates();
}
