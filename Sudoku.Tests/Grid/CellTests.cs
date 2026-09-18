using Sudoku.Core;
using Sudoku.Core.Grid;

namespace Sudoku.Tests;

public class CellTests
{
    [Fact]
    public void DefaultCell_HasZeroValue_AllCandidatesAvailable()
    {
        var cell = new Cell();

        Assert.Equal((byte)0, cell.Value);
        Assert.False(cell.IsGiven);
        Assert.Equal(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }, cell.GetCandidates().ToArray());
    }

    [Fact]
    public void GivenCell_StoresValue_AndMarksGiven()
    {
        byte value = 7;
        var cell = new Cell(value);

        Assert.Equal(value, cell.Value);
        Assert.True(cell.IsGiven);
    }

    [Fact]
    public void AddCandidate_SetsCandidateBit_AndHasCandidateReturnsTrue()
    {
        var cell = new Cell();

        byte candidate = 3;
        cell.AddCandidate(candidate);

        Assert.True(cell.HasCandidate(candidate));
    }

    [Fact]
    public void RemoveCandidate_RemovesExistingCandidates()
    {
        var cell = new Cell();

        byte candidate1 = 1;
        byte candidate2 = 4;
        byte candidate3 = 8;

        cell.RemoveCandidate(candidate1);
        cell.RemoveCandidate(candidate2);
        cell.RemoveCandidate(candidate3);

        var candidates = cell.GetCandidates().ToArray();

        Assert.Equal(new byte[] { 2, 3, 5, 6, 7, 9 }, candidates);
    }


    [Fact]
    public void AddCandidate_WithInvalidValue_ThrowsArgumentOutOfRangeException()
    {
        var cell = new Cell();

        Assert.Throws<ArgumentOutOfRangeException>(() => cell.AddCandidate(10));
    }

    [Fact]
    public void HasCandidate_WithInvalidValue_ThrowsArgumentOutOfRangeException()
    {
        var cell = new Cell();

        Assert.Throws<ArgumentOutOfRangeException>(() => cell.HasCandidate(10));
    }
}
