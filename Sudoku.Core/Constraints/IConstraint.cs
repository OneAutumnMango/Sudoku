namespace Sudoku.Core.Constraints;

public interface IConstraint
{
    public bool IsSatisfied();
    void ComputeAndFillCandidates();
}