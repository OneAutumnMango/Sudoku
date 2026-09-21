using System.Reflection;
using Sudoku.Core;
using Sudoku.Core.Generator;
using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Tests.Generator;

public class ValidBoardGeneratorTests
{
    [Fact]
    public void Generate_ReturnsValidBoard_ForStandardRules()
    {
        var fixedPuzzle = ValidBoardGenerator.Generate(new Puzzle(new StandardRuleSet()));

        Assert.True(new StandardRuleSet(fixedPuzzle.Board).FindFirstUnsatisfiedConstraint().IsNone);
    }

    [Fact]
    public void GenerateMinimalBoard_DoesNotHave81FilledSquares()
    {
        var fixedMinimalPuzzle = ValidBoardGenerator.GenerateMinimalBoard(new Puzzle(new StandardRuleSet()));
        var filledSquares = fixedMinimalPuzzle.Board.EnumerateFilledCells().Count();

        Assert.True(filledSquares < 81, $"Expected fewer than 81 filled squares, but found {filledSquares}.");
    }

    [Fact]
    public void Minimise_Throws_WhenBoardContainsEmptyCells()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var board = puzzle.Board;

        board[0, 0].Value = 1;
        board[0, 1].Value = 0;

        Assert.Throws<InvalidOperationException>(() =>
        {
            ValidBoardGenerator.Minimise(puzzle);
        });
    }

    [Fact]
    public void Minimise_Throws_WhenBoardViolatesRules()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var board = puzzle.Board;

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

        Assert.Throws<InvalidOperationException>(() =>
        {
            ValidBoardGenerator.Minimise(puzzle);
        });
    }

    [Fact]
    public void GenerateMinimalBoard_IsMinimal_WhenGreedyRemovalReturnsFalse()
    {
        var greedyRemoval = typeof(ValidBoardGenerator)
            .GetMethod("GreedyRemoval", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(greedyRemoval);

        var result = (bool)greedyRemoval!
            .Invoke(null, [ValidBoardGenerator.GenerateMinimalBoard(
                new Puzzle(new StandardRuleSet())), Random.Shared])!;

        Assert.False(result);
    }

    [Fact]
    public void GenerateMinimalBoard_HasUniqueSolution()
    {
        var uniqueSolution = typeof(ValidBoardGenerator)
            .GetMethod("IsUniqueSolution", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(uniqueSolution);

        var result = (bool)uniqueSolution!
            .Invoke(null, [ValidBoardGenerator.GenerateMinimalBoard(new Puzzle(new StandardRuleSet()))])!;

        Assert.True(result);
    }

    [Fact]
    public void GenerateMinimalBoard_CountSolutions_IsOne()
    {
        var countSolutions = typeof(ValidBoardGenerator)
            .GetMethod("CountSolutions", BindingFlags.NonPublic | BindingFlags.Static);

        Assert.NotNull(countSolutions);

        var result = (int)countSolutions!
            .Invoke(null, [ValidBoardGenerator.GenerateMinimalBoard(new Puzzle(new StandardRuleSet())), 2])!;

        Assert.Equal(1, result);
    }
}
