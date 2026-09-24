using Sudoku.Core;
using Sudoku.Core.Solver;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class HiddenNTechniqueTests
{
    [Fact]
    public void TryApply_WhenHiddenPairExistsInRow_RemovesOtherCandidatesFromPairCells()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2, 3)
            .WithCandidates(0, 1, 1, 2, 4);
        RestrictRest(puzzle, 0, [0, 1], [3, 4, 5, 6, 7, 8, 9]);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new HiddenNTechnique(2).TryApply(puzzle);

        Assert.Equal(2, applied);
        Assert.Equal(2, snapshot.TotalRemoved(puzzle.Board));
        CandidateAssert.Eliminated(snapshot, puzzle.Board, new Elimination(0, 0, 3), new Elimination(0, 1, 4));
        CandidateAssert.HasCandidates(puzzle, 0, 0, 1, 2);
        CandidateAssert.HasCandidates(puzzle, 0, 1, 1, 2);
    }

    [Fact]
    public void TryApply_WhenHiddenPairExistsInBox_RemovesOtherCandidatesFromPairCells()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(1, 1, 1, 2, 3)
            .WithCandidates(2, 2, 1, 2, 4);

        foreach (var (row, col) in new[] { (0, 0), (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1) })
            puzzle.WithCandidates(row, col, 3, 4, 5, 6, 7, 8, 9);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new HiddenNTechnique(2).TryApply(puzzle);

        Assert.Equal(2, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, new Elimination(1, 1, 3), new Elimination(2, 2, 4));
    }

    [Fact]
    public void TryApply_WhenHiddenTripleExists_RemovesOtherCandidatesFromTripleCells()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2, 4)
            .WithCandidates(0, 1, 1, 3, 5)
            .WithCandidates(0, 2, 2, 3, 6);
        RestrictRest(puzzle, 0, [0, 1, 2], [4, 5, 6, 7, 8, 9]);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new HiddenNTechnique(3).TryApply(puzzle);

        Assert.Equal(3, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            new Elimination(0, 0, 4), new Elimination(0, 1, 5), new Elimination(0, 2, 6));
        CandidateAssert.HasCandidates(puzzle, 0, 0, 1, 2);
        CandidateAssert.HasCandidates(puzzle, 0, 1, 1, 3);
        CandidateAssert.HasCandidates(puzzle, 0, 2, 2, 3);
    }

    [Fact]
    public void TryApply_WhenHiddenQuadExists_RemovesOtherCandidatesFromQuadCells()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2, 5)
            .WithCandidates(0, 1, 1, 3, 6)
            .WithCandidates(0, 2, 2, 4, 7)
            .WithCandidates(0, 3, 3, 4, 8);
        RestrictRest(puzzle, 0, [0, 1, 2, 3], [5, 6, 7, 8, 9]);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new HiddenNTechnique(4).TryApply(puzzle);

        Assert.Equal(4, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            new Elimination(0, 0, 5), new Elimination(0, 1, 6),
            new Elimination(0, 2, 7), new Elimination(0, 3, 8));
    }

    [Fact]
    public void TryApply_WhenHiddenSingleExists_ReducesToOneCandidateWithoutPlacingIt()
    {
        var puzzle = PuzzleFactory.Empty();
        for (var col = 1; col < 9; col++)
            puzzle.Board[0, col].RemoveCandidate(1);

        var applied = new HiddenNTechnique(1).TryApply(puzzle);

        Assert.Equal(8, applied);
        CandidateAssert.HasCandidates(puzzle, 0, 0, 1);
        Assert.Equal(0, puzzle.Board[0, 0].Value);
    }

    [Fact]
    public void TryApply_WhenSubsetIsAlreadyANakedSet_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 1, 1, 2);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        Assert.Equal(0, new HiddenNTechnique(2).TryApply(puzzle));
        CandidateAssert.NothingEliminated(snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenHiddenSetDoesNotSpanAllSubsetCells_ReturnsZero()
    {
        // 1 and 2 are confined to a single cell, so this implementation refuses to treat
        // {[0,0], [0,1]} as a hidden pair even though it spans exactly two candidates.
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2, 5)
            .WithCandidates(0, 1, 6, 7);
        RestrictRest(puzzle, 0, [0, 1], [3, 4, 5, 6, 7, 8, 9]);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        Assert.Equal(0, new HiddenNTechnique(2).TryApply(puzzle));
        CandidateAssert.NothingEliminated(snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenCalledTwice_SecondCallReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2, 3)
            .WithCandidates(0, 1, 1, 2, 4);
        RestrictRest(puzzle, 0, [0, 1], [3, 4, 5, 6, 7, 8, 9]);

        var technique = new HiddenNTechnique(2);

        Assert.Equal(2, technique.TryApply(puzzle));
        Assert.Equal(0, technique.TryApply(puzzle));
    }

    [Fact]
    public void TryApply_OnEmptyBoard_ReturnsZero()
    {
        Assert.Equal(0, new HiddenNTechnique(2).TryApply(PuzzleFactory.Empty()));
    }

    [Fact]
    public void TryApply_OnSolvedBoard_ReturnsZero()
    {
        Assert.Equal(0, new HiddenNTechnique(2).TryApply(PuzzleFactory.Solved()));
    }

    [Fact]
    public void TryApply_WhenPuzzleIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new HiddenNTechnique(2).TryApply(null!));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(5)]
    public void Constructor_WhenNIsOutsideSupportedRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new HiddenNTechnique(n));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Constructor_WhenNIsSupported_DoesNotThrow(int n)
    {
        Assert.Equal(Difficulty.Unknown, new HiddenNTechnique(n).Difficulty);
    }

    private static void RestrictRest(Puzzle puzzle, int row, int[] skipColumns, byte[] candidates)
    {
        for (var col = 0; col < 9; col++)
        {
            if (!skipColumns.Contains(col))
                puzzle.WithCandidates(row, col, candidates);
        }
    }
}
