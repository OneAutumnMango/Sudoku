using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Tests.RuleSets;

public class StandardRuleSetTests
{
    [Fact]
    public void FindFirstUnsatisfiedConstraint_WithValidBoard_ReturnsNone()
    {
        var board = CreateSolvedBoard();
        var ruleSet = new StandardRuleSet(board);

        var result = ruleSet.FindFirstUnsatisfiedConstraint();

        Assert.True(result.IsNone);
    }

    [Fact]
    public void FindFirstUnsatisfiedConstraint_WithDuplicateInRow_ReturnsSome()
    {
        var board = CreateSolvedBoard();
        board[0, 0].Value = 5;

        var ruleSet = new StandardRuleSet(board);

        var result = ruleSet.FindFirstUnsatisfiedConstraint();

        Assert.True(result.IsSome);
    }

    [Fact]
    public void GetConstraints_ReturnsTwentySevenStableConstraints()
    {
        var board = new Board(9);
        var ruleSet = new StandardRuleSet(board);

        var first = ruleSet.GetConstraints().ToList();
        var second = ruleSet.GetConstraints().ToList();

        Assert.Equal(27, first.Count);
        Assert.All(first.Zip(second), pair => Assert.Same(pair.First, pair.Second));
        Assert.All(
            board.EnumerateAllCells(),
            item => Assert.Equal(3, first.Count(constraint => constraint.Cells.Contains(item.cell))));
    }

    [Fact]
    public void GetConstraints_UsesDifferentInstancesForDifferentRuleSets()
    {
        var first = new StandardRuleSet(new Board(9)).GetConstraints().First();
        var second = new StandardRuleSet(new Board(9)).GetConstraints().First();

        Assert.NotSame(first, second);
    }

    [Fact]
    public void GetContainingRow_ReturnsCellRow()
    {
        const int rowIndex = 4;
        const int columnIndex = 7;
        var board = new Board(9);
        var ruleSet = new StandardRuleSet(board);

        var row = ruleSet.GetContainingRow(board[rowIndex, columnIndex]);

        Assert.Same(ruleSet.GetConstraints().ElementAt(rowIndex), row);
        Assert.Equal(board.Rows[rowIndex], row.Cells);
    }

    [Fact]
    public void GetContainingColumn_ReturnsCellColumn()
    {
        const int rowIndex = 4;
        const int columnIndex = 7;
        var board = new Board(9);
        var ruleSet = new StandardRuleSet(board);

        var column = ruleSet.GetContainingColumn(board[rowIndex, columnIndex]);

        Assert.Same(ruleSet.GetConstraints().ElementAt(9 + columnIndex), column);
        Assert.Equal(board.Columns[columnIndex], column.Cells);
    }

    [Fact]
    public void GetContainingBox_ReturnsCellBox()
    {
        const int rowIndex = 4;
        const int columnIndex = 7;
        var boxIndex = rowIndex / 3 * 3 + columnIndex / 3;
        var board = new Board(9);
        var ruleSet = new StandardRuleSet(board);

        var box = ruleSet.GetContainingBox(board[rowIndex, columnIndex]);

        Assert.Same(ruleSet.GetConstraints().ElementAt(18 + boxIndex), box);
        Assert.Equal(board.Blocks[boxIndex], box.Cells);
    }

    [Fact]
    public void GetContainingRow_WhenCellBelongsToAnotherBoard_Throws()
    {
        var board = new Board(9);
        var ruleSet = new StandardRuleSet(board);

        Assert.Throws<ArgumentException>(() =>
            ruleSet.GetContainingRow(new Board(9)[0, 0]));
    }

    private static Board CreateSolvedBoard()
    {
        var board = new Board(9);
        var values = new[,]
        {
            { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
            { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
            { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
            { 2, 3, 4, 5, 6, 7, 8, 9, 1 },
            { 5, 6, 7, 8, 9, 1, 2, 3, 4 },
            { 8, 9, 1, 2, 3, 4, 5, 6, 7 },
            { 3, 4, 5, 6, 7, 8, 9, 1, 2 },
            { 6, 7, 8, 9, 1, 2, 3, 4, 5 },
            { 9, 1, 2, 3, 4, 5, 6, 7, 8 }
        };

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                board[row, col].Value = (byte)values[row, col];
            }
        }

        return board;
    }
}
