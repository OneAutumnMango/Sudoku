using Sudoku.Core.Grid;

namespace Sudoku.Tests.Grid;

public class BoardTests
{
    [Fact]
    public void EnumerateFilledCells_OnlyReturnsNonZeroCells()
    {
        var board = new Board(9);
        board[0, 0].Value = 5;
        board[0, 1].Value = 8;
        board[0, 2].Value = 0;

        var filled = board.EnumerateFilledCells().Select(x => x.cell).ToList();

        Assert.Equal(2, filled.Count);
        Assert.All(filled, cell => Assert.NotEqual((byte)0, cell.Value));
    }

    [Fact]
    public void EnumerateEmptyCells_OnlyReturnsZeroValueCells()
    {
        var board = new Board(9);
        board[0, 0].Value = 5;
        board[0, 1].Value = 0;

        var empty = board.EnumerateEmptyCells().Select(x => x.cell).ToList();

        Assert.Equal(80, empty.Count);
        Assert.All(empty, cell => Assert.Equal((byte)0, cell.Value));
    }

    [Fact]
    public void ToString_UsesDotsForEmptyCells()
    {
        var board = new Board(9);
        board[0, 0].Value = 1;
        board[0, 1].Value = 2;
        board[0, 2].Value = 0;

        var text = board.ToString();

        Assert.Contains("1 2 .", text);
    }

    [Fact]
    public void Constructor_WithValues_LoadsCells()
    {
        var values = new int[9, 9];
        values[0, 0] = 5;
        values[8, 8] = 9;

        var board = new Board(values);

        Assert.Equal((byte)5, board[0, 0].Value);
        Assert.Equal((byte)9, board[8, 8].Value);
        Assert.Equal(2, board.EnumerateFilledCells().Count());
    }

    [Fact]
    public void Constructor_WithWrongDimensions_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Board(new int[8, 9]));
    }
}
