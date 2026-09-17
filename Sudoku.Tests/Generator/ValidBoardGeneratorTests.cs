using System.Reflection;
using Sudoku.Core.Generator;
using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Tests.Generator;

public class ValidBoardGeneratorTests
{
    private static readonly StandardRuleSet standardRuleSet = new();
    private static readonly Board fixedBoard = ValidBoardGenerator.Generate(standardRuleSet);
    private static readonly Board fixedMinimalBoard = ValidBoardGenerator.GenerateMinimalBoard(standardRuleSet);

    [Fact]
    public void Generate_ReturnsValidBoard_ForStandardRules()
    {
        Assert.True(standardRuleSet.FindFirstUnsatisfiedConstraint(fixedBoard.Clone()).IsNone);
    }

    [Fact]
    public void Minimise_Throws_WhenBoardContainsEmptyCells()
    {
        var board = new Board(9);
        var ruleSet = new StandardRuleSet();

        board[0, 0].Value = 1;
        board[0, 1].Value = 0;

        Assert.Throws<InvalidOperationException>(() => ValidBoardGenerator.Minimise(board, ruleSet));
    }

    [Fact]
    public void Minimise_Throws_WhenBoardViolatesRules()
    {
        var board = new Board(9);
        var ruleSet = new StandardRuleSet();

        board[0, 0].Value = 1;
        board[0, 1].Value = 1;

        for (int row = 0; row < 9; row++)
        {
            for (int col = 0; col < 9; col++)
            {
                if (row == 0 && col < 2) continue;
                board[row, col].Value = (byte)((row * 3 + col + 1) % 9 + 1);
            }
        }

        Assert.Throws<InvalidOperationException>(() => ValidBoardGenerator.Minimise(board, ruleSet));
    }

    [Fact]
    public void GenerateMinimalBoard_IsMinimal_WhenGreedyRemovalReturnsFalse()
    {
        var greedyRemoval = typeof(ValidBoardGenerator)
            .GetMethod("GreedyRemoval", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(greedyRemoval);

        var result = (bool)greedyRemoval!.Invoke(null, [fixedMinimalBoard.Clone(), standardRuleSet, Random.Shared])!;

        Assert.False(result);
    }

    [Fact]
    public void GenerateMinimalBoard_HasUniqueSolution()
    {
        var uniqueSolution = typeof(ValidBoardGenerator)
            .GetMethod("IsUniqueSolution", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(uniqueSolution);

        var result = (bool)uniqueSolution!.Invoke(null, [fixedMinimalBoard.Clone(), standardRuleSet])!;

        Assert.True(result);
    }

    [Fact]
    public void GenerateMinimalBoard_CountSolutions_IsOne()
    {
        var countSolutions = typeof(ValidBoardGenerator)
            .GetMethod("CountSolutions", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(countSolutions);

        var result = (int)countSolutions!.Invoke(null, [fixedMinimalBoard.Clone(), standardRuleSet, 2])!;

        Assert.Equal(1, result);
    }
}
