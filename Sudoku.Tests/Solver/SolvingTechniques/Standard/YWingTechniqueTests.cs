using Sudoku.Core.Solver;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class YWingTechniqueTests
{
    [Fact]
    public void TryApply_WhenWingsSitInPivotRowAndColumn_RemovesZFromTheCellSeeingBothWings()
    {
        // pivot (0,0)={1,2}, wings (0,4)={1,3} and (4,0)={2,3}, so 3 goes from (4,4)
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 4, 1, 3)
            .WithCandidates(4, 0, 2, 3);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new YWingTechnique().TryApply(puzzle);

        Assert.Equal(1, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, new Elimination(4, 4, 3));
        CandidateAssert.AppliedMatchesDiff(applied, snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenWingsSitInPivotRowAndBox_RemovesZFromInsideThosePivotGroups()
    {
        // pivot (0,0)={1,2}, wings (0,4)={1,3} and (1,1)={2,3}; the targets in row 0 are
        // only visible if the wing's shared-with-pivot group is not excluded
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 4, 1, 3)
            .WithCandidates(1, 1, 2, 3);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new YWingTechnique().TryApply(puzzle);

        Assert.Equal(5, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            new Elimination(0, 1, 3), new Elimination(0, 2, 3),
            new Elimination(1, 3, 3), new Elimination(1, 4, 3), new Elimination(1, 5, 3));
    }

    [Fact]
    public void TryApply_DoesNotEliminateFromFilledCells()
    {
        var puzzle = PuzzleFactory.Empty();
        puzzle.SetCell(1, 5, 9);
        puzzle.WithCandidates(0, 0, 1, 2)
              .WithCandidates(0, 4, 1, 3)
              .WithCandidates(1, 1, 2, 3);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new YWingTechnique().TryApply(puzzle);

        Assert.Equal(4, applied);
        CandidateAssert.NoFilledCellTouched(snapshot, puzzle.Board);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            new Elimination(0, 1, 3), new Elimination(0, 2, 3),
            new Elimination(1, 3, 3), new Elimination(1, 4, 3));
    }

    [Fact]
    public void TryApply_WhenTwoYWingsExist_AppliesBoth()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 4, 1, 3)
            .WithCandidates(4, 0, 2, 3)
            .WithCandidates(8, 8, 4, 5)
            .WithCandidates(8, 3, 4, 6)
            .WithCandidates(3, 8, 5, 6);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new YWingTechnique().TryApply(puzzle);

        Assert.Equal(2, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            new Elimination(3, 3, 6), new Elimination(4, 4, 3));
    }

    [Fact]
    public void TryApply_WhenBothWingsShareTheSamePivotGroup_ReturnsZero()
    {
        // a valid Y-Wing, but this implementation requires the wings to reach the pivot
        // through two different constraints, so the elimination of 3 from row 0 is missed
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 4, 1, 3)
            .WithCandidates(0, 7, 2, 3);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        Assert.Equal(0, new YWingTechnique().TryApply(puzzle));
        CandidateAssert.NothingEliminated(snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenAWingSharesBothPivotCandidates_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 4, 1, 2)
            .WithCandidates(4, 0, 2, 3);

        Assert.Equal(0, new YWingTechnique().TryApply(puzzle));
    }

    [Fact]
    public void TryApply_WhenWingsDoNotShareTheSameZ_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 4, 1, 3)
            .WithCandidates(4, 0, 2, 4);

        Assert.Equal(0, new YWingTechnique().TryApply(puzzle));
    }

    [Fact]
    public void TryApply_WhenCalledTwice_SecondCallReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 4, 1, 3)
            .WithCandidates(4, 0, 2, 3);

        var technique = new YWingTechnique();

        Assert.Equal(1, technique.TryApply(puzzle));
        Assert.Equal(0, technique.TryApply(puzzle));
    }

    [Fact]
    public void TryApply_OnSparseGridWithAYWing_RemovesSixFromBothSeenCells()
    {
        // topleft is 0,0
        // cell 1,4 is pivot with candidates {7,9}
        // wings are 2,5 with candidates {6,7} and 6,4 with candidates {6,9}
        var puzzle = PuzzleFactory.FromGrid(
            new int[,]
            {
                { 5, 9, 7, 0, 0, 0, 0, 1, 0 },
                { 4, 1, 6, 0, 0, 8, 0, 5, 3 },
                { 8, 2, 3, 1, 5, 0, 4, 0, 9 },
                { 0, 0, 4, 0, 0, 0, 0, 9, 1 },
                { 0, 0, 9, 0, 0, 0, 0, 0, 2 },
                { 0, 3, 2, 8, 0, 9, 5, 4, 7 },
                { 3, 7, 8, 5, 0, 1, 0, 2, 4 },
                { 2, 6, 1, 0, 8, 4, 0, 3, 5 },
                { 9, 4, 5, 0, 2, 0, 1, 0, 0 }
            });

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new YWingTechnique().TryApply(puzzle);

        Assert.Equal(2, applied);
        CandidateAssert.NoFilledCellTouched(snapshot, puzzle.Board);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            new Elimination(0, 4, 6), new Elimination(8, 5, 6));
    }

    [Fact]
    public void TryApply_OnEmptyBoard_ReturnsZero()
    {
        Assert.Equal(0, new YWingTechnique().TryApply(PuzzleFactory.Empty()));
    }

    [Fact]
    public void TryApply_OnSolvedBoard_ReturnsZero()
    {
        Assert.Equal(0, new YWingTechnique().TryApply(PuzzleFactory.Solved()));
    }

    [Fact]
    public void Difficulty_IsAdvanced()
    {
        Assert.Equal(Difficulty.Advanced, new YWingTechnique().Difficulty);
    }
}
