using Sudoku.Core.Solver;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class NakedNTechniqueTests
{
    [Fact]
    public void TryApply_WhenNakedPairExists_RemovesPairCandidatesFromRowAndBox()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 1, 1, 2)
            .WithCandidates(0, 2, 1, 2, 3);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new NakedNTechnique(2).TryApply(puzzle);

        // the pair shares a row and a box, so both constraints clear their own peers
        Assert.Equal(13, applied);
        CandidateAssert.HasCandidates(puzzle, 0, 0, 1, 2);
        CandidateAssert.HasCandidates(puzzle, 0, 1, 1, 2);
        CandidateAssert.HasCandidates(puzzle, 0, 2, 3);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, Removals([1, 2],
            (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8),
            (1, 0), (1, 1), (1, 2), (2, 0), (2, 1), (2, 2)));
    }

    [Fact]
    public void TryApply_CountsChangedCellsNotRemovedCandidates()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 1, 1, 2);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new NakedNTechnique(2).TryApply(puzzle);

        Assert.Equal(13, applied);
        Assert.Equal(26, snapshot.TotalRemoved(puzzle.Board));
    }

    [Fact]
    public void TryApply_WhenNakedPairSharesOnlyAColumn_RemovesFromThatColumnOnly()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(3, 0, 1, 2);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new NakedNTechnique(2).TryApply(puzzle);

        Assert.Equal(7, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, Removals([1, 2],
            (1, 0), (2, 0), (4, 0), (5, 0), (6, 0), (7, 0), (8, 0)));
    }

    [Fact]
    public void TryApply_WhenNakedPairSharesOnlyABox_RemovesFromThatBoxOnly()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(1, 1, 1, 2)
            .WithCandidates(2, 2, 1, 2);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new NakedNTechnique(2).TryApply(puzzle);

        Assert.Equal(7, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, Removals([1, 2],
            (0, 0), (0, 1), (0, 2), (1, 0), (1, 2), (2, 0), (2, 1)));
    }

    [Fact]
    public void TryApply_DoesNotEliminateFromFilledCells()
    {
        var puzzle = PuzzleFactory.Empty();
        puzzle.SetCell(0, 8, 9);
        puzzle.WithCandidates(0, 0, 1, 2)
              .WithCandidates(0, 1, 1, 2);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new NakedNTechnique(2).TryApply(puzzle);

        Assert.Equal(12, applied);
        CandidateAssert.NoFilledCellTouched(snapshot, puzzle.Board);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, Removals([1, 2],
            (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7),
            (1, 0), (1, 1), (1, 2), (2, 0), (2, 1), (2, 2)));
    }

    [Fact]
    public void TryApply_WhenTripleContainsASubsetPair_RemovesTripleCandidates()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 1, 2, 3)
            .WithCandidates(0, 2, 1, 2, 3)
            .WithCandidates(0, 3, 1, 2, 3, 4);

        var applied = new NakedNTechnique(3).TryApply(puzzle);

        Assert.Equal(12, applied);
        CandidateAssert.HasCandidates(puzzle, 0, 3, 4);
    }

    [Fact]
    public void TryApply_WhenThreeCellsSpanFourCandidates_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 1, 3, 4)
            .WithCandidates(0, 2, 1, 3);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        Assert.Equal(0, new NakedNTechnique(3).TryApply(puzzle));
        CandidateAssert.NothingEliminated(snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenNakedQuadExists_RemovesQuadCandidatesFromRow()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 1, 1, 3)
            .WithCandidates(0, 2, 2, 4)
            .WithCandidates(0, 3, 3, 4)
            .WithCandidates(0, 4, 1, 2, 3, 4, 5);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new NakedNTechnique(4).TryApply(puzzle);

        Assert.Equal(5, applied);
        CandidateAssert.HasCandidates(puzzle, 0, 4, 5);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, Removals([1, 2, 3, 4],
            (0, 4), (0, 5), (0, 6), (0, 7), (0, 8)));
    }

    [Fact]
    public void TryApply_WhenCalledTwice_SecondCallReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 1, 1, 2);

        var technique = new NakedNTechnique(2);

        Assert.Equal(13, technique.TryApply(puzzle));
        Assert.Equal(0, technique.TryApply(puzzle));
    }

    [Fact]
    public void TryApply_WhenNoNakedSubsetExists_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 1, 1, 3);

        Assert.Equal(0, new NakedNTechnique(2).TryApply(puzzle));
    }

    [Fact]
    public void TryApply_OnEmptyBoard_ReturnsZero()
    {
        Assert.Equal(0, new NakedNTechnique(2).TryApply(PuzzleFactory.Empty()));
    }

    [Fact]
    public void TryApply_WhenPuzzleIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new NakedNTechnique(2).TryApply(null!));
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(5)]
    public void Constructor_WhenNIsOutsideSupportedRange_Throws(int n)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new NakedNTechnique(n));
    }

    [Theory]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public void Constructor_WhenNIsSupported_DoesNotThrow(int n)
    {
        Assert.Equal(Difficulty.Unknown, new NakedNTechnique(n).Difficulty);
    }

    private static Elimination[] Removals(byte[] candidates, params (int Row, int Col)[] cells) =>
        [.. cells.SelectMany(cell => candidates.Select(c => new Elimination(cell.Row, cell.Col, c)))];
}
