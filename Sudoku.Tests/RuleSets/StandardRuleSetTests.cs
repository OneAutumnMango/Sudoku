using Sudoku.Core;
using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Tests.RuleSets;

public class StandardRuleSetTests
{
    [Fact]
    public void FindFirstUnsatisfiedConstraint_WithValidBoard_ReturnsNone()
    {
        var board = CreateSolvedBoard();
        var ruleSet = new StandardRuleSet();
        ruleSet.Initialize(board);

        var result = ruleSet.FindFirstUnsatisfiedConstraint();

        Assert.True(result.IsNone);
    }

    [Fact]
    public void FindFirstUnsatisfiedConstraint_WithDuplicateInRow_ReturnsSome()
    {
        var board = CreateSolvedBoard();
        board[0, 0].Value = 5;

        var ruleSet = new StandardRuleSet();
        ruleSet.Initialize(board);

        var result = ruleSet.FindFirstUnsatisfiedConstraint();

        Assert.True(result.IsSome);
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
