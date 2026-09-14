using Sudoku.Core;

namespace Sudoku.Tests;

public class CellTests
{
    [Fact]
    public void DefaultCell_HasZeroValue_NoCandidatesSelected()
    {
        var cell = new Cell();

        Assert.Equal((byte)0, cell.Value);
        Assert.False(cell.IsGiven);
        Assert.Empty(cell.GetCandidates());
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
        cell.addCandidate(candidate);

        Assert.True(cell.HasCandidate(candidate));
    }

    [Fact]
    public void AddCandidate_WithMultipleValues_EnumeratesOnlyThoseCandidates()
    {
        var cell = new Cell();

        byte candidate1 = 1;
        byte candidate2 = 4;
        byte candidate3 = 8;

        cell.addCandidate(candidate1);
        cell.addCandidate(candidate2);
        cell.addCandidate(candidate3);

        var candidates = cell.GetCandidates().ToArray();

        Assert.Equal(new[] { candidate1, candidate2, candidate3 }, candidates);
    }

    [Fact]
    public void AddCandidate_WithInvalidValue_ThrowsArgumentOutOfRangeException()
    {
        var cell = new Cell();

        Assert.Throws<ArgumentOutOfRangeException>(() => cell.addCandidate(9));
    }

    [Fact]
    public void HasCandidate_WithInvalidValue_ThrowsArgumentOutOfRangeException()
    {
        var cell = new Cell();

        Assert.Throws<ArgumentOutOfRangeException>(() => cell.HasCandidate(9));
    }
}
