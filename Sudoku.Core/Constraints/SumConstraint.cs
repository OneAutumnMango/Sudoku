using Sudoku.Core.Grid;

namespace Sudoku.Core.Constraints;

public class SumConstraint(IReadOnlyList<Cell> cells, int sum) : IConstraint
{
    public IReadOnlyList<Cell> Cells { get; } = cells;
    private readonly int _sum = sum;

    public bool IsSatisfied()
    {
        int sum = 0;
        foreach (var cell in Cells)
        {
            sum += cell.Value;
        }
        return sum == _sum;
    }

    public void ComputeAndFillCandidates()
    {
        throw new NotImplementedException();
    }
}
