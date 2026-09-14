namespace Sudoku.Core.Constraints;

public class UniqueGroupConstraint : IConstraint
{
    private readonly IReadOnlyList<Cell> _cells;

    public UniqueGroupConstraint(IReadOnlyList<Cell> cells)
    {
        _cells = cells;
    }

    public bool IsSatisfied()
    {
        var seen = new HashSet<byte>();
        foreach (var cell in _cells)
        {
            if (cell.Value == 0) continue;
            if (seen.Contains(cell.Value)) return false;
            seen.Add(cell.Value);
        }
        return true;
    }

}