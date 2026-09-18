using Sudoku.Core.Grid;

namespace Sudoku.Core.Constraints;

public class UniqueGroupConstraint(IReadOnlyList<Cell> cells) : IConstraint
{
    public IReadOnlyList<Cell> Cells { get; } = cells;

    public bool IsSatisfied()
    {
        var seen = new HashSet<byte>();
        foreach (var cell in Cells)
        {
            if (cell.Value == 0)
                continue;

            if (seen.Contains(cell.Value))
                return false;

            seen.Add(cell.Value);
        }
        return true;
    }

    public void ComputeAndFillCandidates()
    {
        ushort used = 0;

        foreach (var cell in Cells)
        {
            if (cell.Value != 0)
                used |= (ushort)(1 << (cell.Value - 1));
        }

        // candidates for all cells are the same so invert what is already used and apply to each cell
        ushort candidates = (ushort)(Cell.AllCandidates & ~used);

        foreach (var cell in Cells)
        {
            if (cell.Value == 0)
                cell.IntersectCandidates(candidates);
        }
    }
}
