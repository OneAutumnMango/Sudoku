using Sudoku.Core;
using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Tests.RuleSets;

public class StandardRuleSetTests
{
    [Fact]
    public void IsSatisfied_WithValidBoard_ReturnsTrue()
    {
        var board = CreateSolvedBoard();
        var groups = board.Rows.Concat(board.Columns).Concat(board.Blocks).ToList();
        var ruleSet = new StandardRuleSet(groups);

        Assert.True(ruleSet.IsSatisfied());
    }

    [Fact]
    public void IsSatisfied_WithDuplicateInRow_ReturnsFalse()
    {
        var board = CreateSolvedBoard();
        board[0, 0].Value = 5;

        var groups = board.Rows.Concat(board.Columns).Concat(board.Blocks).ToList();
        var ruleSet = new StandardRuleSet(groups);

        Assert.False(ruleSet.IsSatisfied());
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
