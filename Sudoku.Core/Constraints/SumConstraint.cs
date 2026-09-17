using Sudoku.Core.Grid;

namespace Sudoku.Core.Constraints;

public class SumConstraint(IReadOnlyList<Cell> cells, int sum) : IConstraint
{
    private readonly IReadOnlyList<Cell> _cells = cells;
    private readonly int _sum = sum;

    public bool IsSatisfied()
    {
        int sum = 0;
        foreach (var cell in _cells)
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
