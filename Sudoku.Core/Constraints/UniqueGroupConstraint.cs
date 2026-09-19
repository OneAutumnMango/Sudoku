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

    public ushort GetAllowedCandidates(Cell cell)
    {
        if (!Cells.Contains(cell))
            throw new ArgumentException("Cell does not belong to this constraint.", nameof(cell));

        ushort used = 0;

        foreach (var candidateCell in Cells)
        {
            if (candidateCell.Value != 0)
                used |= (ushort)(1 << (candidateCell.Value - 1));
        }

        return (ushort)(Cell.AllCandidates & ~used);
    }

    public void ComputeAndFillCandidates()
    {
        foreach (var cell in Cells)
        {
            if (cell.Value == 0)
                cell.IntersectRuleCandidates(GetAllowedCandidates(cell));
        }
    }
}
