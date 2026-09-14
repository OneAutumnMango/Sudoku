namespace Sudoku.Core.Constraints;

public class SumConstraint : IConstraint
{
    private readonly IReadOnlyList<Cell> _cells;
    private readonly int _sum;

    public SumConstraint(IReadOnlyList<Cell> cells, int sum)
    {
        _cells = cells;
        _sum = sum;
    }

    public bool IsSatisfied()
    {
        int sum = 0;
        foreach (var cell in _cells)
        {
            sum += cell.Value;
        }
        return sum == _sum;
    }
}
