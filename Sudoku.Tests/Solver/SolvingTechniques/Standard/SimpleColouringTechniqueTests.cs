using Sudoku.Core;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class SimpleColouringTechniqueTests
{
    // A chain of four conjugate pairs on candidate 5. Colours alternate along it, so the two ends
    // (0,0) and (1,1) land on the same colour and both sit in box 0, which also holds the spare
    // (2,2) so that box 0 is not itself a conjugate pair.
    private static readonly (int Row, int Column)[] CollidingChain =
        [(0, 0), (0, 4), (4, 4), (4, 1), (1, 1), (2, 2)];

    // The same chain without an end in box 0, so its two ends carry opposite colours.
    private static readonly (int Row, int Column)[] NonCollidingChain =
        [(0, 0), (0, 4), (4, 4), (4, 1)];

    [Fact]
    public void TryApply_WhenBothChainEndsShareABox_RemovesEveryCellOfTheCollidingColour()
    {
        var puzzle = WithCandidateOnlyIn(PuzzleFactory.Empty(), 5, CollidingChain);
        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        var applied = new SimpleColouringTechnique().TryApply(puzzle);

        Assert.Equal(3, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            new Elimination(0, 0, 5),
            new Elimination(1, 1, 5),
            new Elimination(4, 4, 5));
        CandidateAssert.AppliedMatchesDiff(applied, snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_LeavesTheOppositeColourAndCellsOutsideTheChainAlone()
    {
        var puzzle = WithCandidateOnlyIn(PuzzleFactory.Empty(), 5, CollidingChain);

        new SimpleColouringTechnique().TryApply(puzzle);

        Assert.True(puzzle.Board[0, 4].HasCandidate(5));
        Assert.True(puzzle.Board[4, 1].HasCandidate(5));
        Assert.True(puzzle.Board[2, 2].HasCandidate(5));
    }

    [Fact]
    public void TryApply_WhenTheChainEndsHaveOppositeColours_ReturnsZero()
    {
        var puzzle = WithCandidateOnlyIn(PuzzleFactory.Empty(), 5, NonCollidingChain);
        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        Assert.Equal(0, new SimpleColouringTechnique().TryApply(puzzle));
        CandidateAssert.NothingEliminated(snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenNoConjugatePairsExist_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty();
        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        Assert.Equal(0, new SimpleColouringTechnique().TryApply(puzzle));
        CandidateAssert.NothingEliminated(snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_ColoursEachCandidateIndependently()
    {
        // the transpose of the colliding chain, so candidate 6 collides in box 0 as well
        (int Row, int Column)[] transposed = [(0, 0), (4, 0), (4, 4), (1, 4), (1, 1), (2, 2)];

        var puzzle = WithCandidateOnlyIn(
            WithCandidateOnlyIn(PuzzleFactory.Empty(), 5, CollidingChain), 6, transposed);
        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        var applied = new SimpleColouringTechnique().TryApply(puzzle);

        Assert.Equal(6, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            new Elimination(0, 0, 5), new Elimination(0, 0, 6),
            new Elimination(1, 1, 5), new Elimination(1, 1, 6),
            new Elimination(4, 4, 5), new Elimination(4, 4, 6));
    }

    [Fact]
    public void TryApply_WhenCalledAgain_ReturnsZero()
    {
        var puzzle = WithCandidateOnlyIn(PuzzleFactory.Empty(), 5, CollidingChain);
        var technique = new SimpleColouringTechnique();

        Assert.Equal(3, technique.TryApply(puzzle));

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        Assert.Equal(0, technique.TryApply(puzzle));
        CandidateAssert.NothingEliminated(snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_DoesNotTouchFilledCells()
    {
        var puzzle = PuzzleFactory.Empty();
        puzzle.SetCell(8, 8, 1);
        WithCandidateOnlyIn(puzzle, 5, CollidingChain);
        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        new SimpleColouringTechnique().TryApply(puzzle);

        CandidateAssert.NoFilledCellTouched(snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenPuzzleIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new SimpleColouringTechnique().TryApply(null!));
    }

    [Fact]
    public void TryApply_OnReportedPuzzle_EliminatesFiveCandidateFours()
    {
        var puzzle = PuzzleFactory.FromString("""
            248.6....
            659.1..28
            1372.856.
            763824.5.
            .1...6..2
            .2..7...6
            586.3.2..
            47298.6..
            3916.28..
            """);
        ConfigureCandidateFours(puzzle);
        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var technique = new SimpleColouringTechnique();

        var applied = technique.TryApply(puzzle);

        Assert.Equal(5, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            new Elimination(1, 6, 4),
            new Elimination(2, 4, 4),
            new Elimination(6, 3, 4),
            new Elimination(6, 8, 4),
            new Elimination(8, 7, 4));
        CandidateAssert.NoFilledCellTouched(snapshot, puzzle.Board);
    }

    /// <summary>Leaves the candidate only in the given cells, so the conjugate pairs are exact.</summary>
    private static Puzzle WithCandidateOnlyIn(
        Puzzle puzzle,
        byte candidate,
        params (int Row, int Column)[] cells)
    {
        var kept = cells.ToHashSet();

        foreach (var (row, col, cell) in puzzle.Board.EnumerateEmptyCells())
            if (!kept.Contains((row, col)))
                cell.RemoveCandidate(candidate);

        return puzzle;
    }

    private static void ConfigureCandidateFours(Puzzle puzzle)
    {
        var chainCells = new HashSet<(int Row, int Column)>
        {
            (1, 3), (1, 6),
            (2, 4), (2, 8),
            (4, 2), (4, 6), (4, 7),
            (5, 2), (5, 6), (5, 7),
            (6, 3), (6, 7), (6, 8),
            (8, 4), (8, 7),
        };

        foreach (var (row, col, cell) in puzzle.Board.EnumerateEmptyCells())
            cell.IntersectCandidates(chainCells.Contains((row, col)) ? CandidateFour : (ushort)0);
    }

    private const ushort CandidateFour = 1 << 3;
}