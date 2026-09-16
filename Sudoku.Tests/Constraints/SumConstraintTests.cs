using System.Linq;
using Sudoku.Core;
using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;

namespace Sudoku.Tests.Constraints;

public class SumConstraintTests
{
    [Fact]
    public void IsSatisfied_WhenSumMatches_ReturnsTrue()
    {
        byte[] values = { 1, 2, 3, 4, 5, 6 };
        var cells = new List<Cell>
        {
            new Cell(values[0]), new Cell(values[1]), new Cell(values[2]),
            new Cell(values[3]), new Cell(values[4]), new Cell(values[5])
        };

        var constraint = new SumConstraint(cells, values.Sum(x => (int)x));

        Assert.True(constraint.IsSatisfied());
    }

    [Fact]
    public void IsSatisfied_WhenSumDoesNotMatch_ReturnsFalse()
    {
        byte[] values = { 1, 2, 3, 4, 5, 6 };
        var cells = new List<Cell>
        {
            new Cell(values[0]), new Cell(values[1]), new Cell(values[2]),
            new Cell(values[3]), new Cell(values[4]), new Cell(values[5])
        };

        var constraint = new SumConstraint(cells, values.Sum(x => (int)x) + 1);

        Assert.False(constraint.IsSatisfied());
    }
}