using Sudoku.Core;
using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;

namespace Sudoku.Tests.Constraints;

public class UniqueGroupConstraintTests
{
    [Fact]
    public void IsSatisfied_WithUniqueValues_ReturnsTrue()
    {
        int[] values = [1, 2, 3, 4, 5, 6, 7, 8, 9];
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
        int[] values = [1, 2, 3, 4, 5, 6, 7, 8, 1];
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
        int[] values = [0, 1, 0, 2, 3, 4, 5, 6, 7];
        var cells = new List<Cell>();

        foreach (var value in values)
        {
            cells.Add(new Cell((byte)value));
        }

        var constraint = new UniqueGroupConstraint(cells);

        Assert.True(constraint.IsSatisfied());
    }

    [Fact]
    public void ComputeCandidates_RemovesAlreadyUsedDigitsFromEmptyCell()
    {
        var cells = new List<Cell>
        {
            new(1),
            new(3),
            new(5),
            new(7),
            new(2),
            new(4),
            new(6),
            new(8),
            new()
        };

        var constraint = new UniqueGroupConstraint(cells);

        constraint.ComputeAndFillCandidates();

        Assert.Equal(new byte[] { 9 }, cells[8].GetCandidates().ToArray());
    }

    [Fact]
    public void ComputeCandidates_LeavesOnlyRemainingCandidatesForMultipleEmptyCells()
    {
        var cells = new List<Cell>
        {
            new(1),
            new(0),
            new(3),
            new(0),
            new(5),
            new(0),
            new(7),
            new(9),
            new(0)
        };

        var constraint = new UniqueGroupConstraint(cells);

        constraint.ComputeAndFillCandidates();

        Assert.Equal(new byte[] { 2, 4, 6, 8 }, cells[1].GetCandidates().OrderBy(x => x).ToArray());
        Assert.Equal(new byte[] { 2, 4, 6, 8 }, cells[3].GetCandidates().OrderBy(x => x).ToArray());
        Assert.Equal(new byte[] { 2, 4, 6, 8 }, cells[5].GetCandidates().OrderBy(x => x).ToArray());
        Assert.Equal(new byte[] { 2, 4, 6, 8 }, cells[8].GetCandidates().OrderBy(x => x).ToArray());
    }
}
