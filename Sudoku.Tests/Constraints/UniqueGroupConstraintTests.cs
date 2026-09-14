using Sudoku.Core;
using Sudoku.Core.Constraints;

namespace Sudoku.Tests.Constraints;

public class UniqueGroupConstraintTests
{
    [Fact]
    public void IsSatisfied_WithUniqueValues_ReturnsTrue()
    {
        int[] values = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        var cells = new List<Cell>();

        foreach (var value in values)
        {
            cells.Add(new Cell((byte)value));
        }

        var constraint = new UniqueGroupConstraint(cells);

        Assert.True(constraint.IsSatisfied());
    }

    [Fact]
    public void IsSatisfied_WithDuplicateValue_ReturnsFalse()
    {
        int[] values = { 1, 2, 3, 4, 5, 6, 7, 8, 1 };
        var cells = new List<Cell>();

        foreach (var value in values)
        {
            cells.Add(new Cell((byte)value));
        }

        var constraint = new UniqueGroupConstraint(cells);

        Assert.False(constraint.IsSatisfied());
    }

    [Fact]
    public void IsSatisfied_IgnoresZeroValues()
    {
        int[] values = { 0, 1, 0, 2, 3, 4, 5, 6, 7 };
        var cells = new List<Cell>();

        foreach (var value in values)
        {
            cells.Add(new Cell((byte)value));
        }

        var constraint = new UniqueGroupConstraint(cells);

        Assert.True(constraint.IsSatisfied());
    }
}
