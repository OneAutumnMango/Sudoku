using Sudoku.Core;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class FishTechniqueTests
{
    [Fact]
    public void TryApply_WhenRowXWingExists_RemovesCandidateFromFishColumns()
    {
        var puzzle = PuzzleFactory.Empty();
        RestrictRow(puzzle, 1, 5, 2, 6);
        RestrictRow(puzzle, 4, 5, 2, 6);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new FishTechnique(2).TryApply(puzzle);

        Assert.Equal(14, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, ExpectedGrid([0, 2, 3, 5, 6, 7, 8], [2, 6], 5));
        CandidateAssert.AppliedMatchesDiff(applied, snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenColumnXWingExists_RemovesCandidateFromFishRows()
    {
        var puzzle = PuzzleFactory.Empty();
        RestrictColumn(puzzle, 1, 7, 3, 7);
        RestrictColumn(puzzle, 5, 7, 3, 7);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new FishTechnique(2).TryApply(puzzle);

        Assert.Equal(14, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, ExpectedGrid([3, 7], [0, 2, 3, 4, 6, 7, 8], 7));
    }

    [Fact]
    public void TryApply_DoesNotEliminateFromFilledCells()
    {
        var puzzle = PuzzleFactory.Empty();
        puzzle.SetCell(7, 2, 3);
        puzzle.SetCell(7, 6, 4);
        RestrictRow(puzzle, 1, 5, 2, 6);
        RestrictRow(puzzle, 4, 5, 2, 6);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new FishTechnique(2).TryApply(puzzle);

        // row 7 is skipped in both fish columns because those cells are already solved
        Assert.Equal(12, applied);
        CandidateAssert.NoFilledCellTouched(snapshot, puzzle.Board);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, ExpectedGrid([0, 2, 3, 5, 6, 8], [2, 6], 5));
    }

    [Fact]
    public void TryApply_WhenSwordfishExists_RemovesCandidateFromThreeColumns()
    {
        var puzzle = PuzzleFactory.Empty();
        RestrictRow(puzzle, 0, 6, 1, 4, 7);
        RestrictRow(puzzle, 4, 6, 1, 7);
        RestrictRow(puzzle, 8, 6, 1, 4, 7);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new FishTechnique(3).TryApply(puzzle);

        Assert.Equal(18, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, ExpectedGrid([1, 2, 3, 5, 6, 7], [1, 4, 7], 6));
    }

    [Fact]
    public void TryApply_WhenJellyfishExists_RemovesCandidateFromFourColumns()
    {
        var puzzle = PuzzleFactory.Empty();
        foreach (var row in new[] { 0, 2, 4, 6 })
            RestrictRow(puzzle, row, 2, 0, 3, 5, 8);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new FishTechnique(4).TryApply(puzzle);

        Assert.Equal(20, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board, ExpectedGrid([1, 3, 5, 7, 8], [0, 3, 5, 8], 2));
    }

    [Fact]
    public void TryApply_WhenBaseRowsSpanAnExtraColumn_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty();
        RestrictRow(puzzle, 1, 5, 2, 6);
        RestrictRow(puzzle, 4, 5, 2, 7);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new FishTechnique(2).TryApply(puzzle);

        Assert.Equal(0, applied);
        CandidateAssert.NothingEliminated(snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenThreeBaseRowsSpanOnlyTwoColumns_ReturnsZero()
    {
        // A degenerate swordfish is a valid pattern, but this implementation requires the
        // base rows to span exactly n perpendicular groups.
        var puzzle = PuzzleFactory.Empty();
        RestrictRow(puzzle, 1, 5, 2, 6);
        RestrictRow(puzzle, 4, 5, 2, 6);
        RestrictRow(puzzle, 6, 5, 2, 6);

        Assert.Equal(0, new FishTechnique(3).TryApply(puzzle));
    }

    [Fact]
    public void TryApply_WhenCalledTwice_SecondCallReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty();
        RestrictRow(puzzle, 1, 5, 2, 6);
        RestrictRow(puzzle, 4, 5, 2, 6);

        var technique = new FishTechnique(2);

        Assert.Equal(14, technique.TryApply(puzzle));
        Assert.Equal(0, technique.TryApply(puzzle));
    }

    [Fact]
    public void TryApply_WhenNoFishExists_ReturnsZero()
    {
        Assert.Equal(0, new FishTechnique(2).TryApply(PuzzleFactory.Empty()));
    }

    [Fact]
    public void TryApply_WhenRuleSetIsNotStandard_Throws()
    {
        var puzzle = new Puzzle(new NonStandardRuleSet());

        Assert.Throws<ArgumentException>(() => new FishTechnique(2).TryApply(puzzle));
    }

    [Fact]
    public void TryApply_WhenPuzzleIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new FishTechnique(2).TryApply(null!));
    }

    private static void RestrictRow(Puzzle puzzle, int row, byte candidate, params int[] keepColumns)
    {
        for (var col = 0; col < 9; col++)
        {
            if (!keepColumns.Contains(col))
                puzzle.Board[row, col].RemoveCandidate(candidate);
        }
    }

    private static void RestrictColumn(Puzzle puzzle, int col, byte candidate, params int[] keepRows)
    {
        for (var row = 0; row < 9; row++)
        {
            if (!keepRows.Contains(row))
                puzzle.Board[row, col].RemoveCandidate(candidate);
        }
    }

    private static Elimination[] ExpectedGrid(int[] rows, int[] cols, byte candidate) =>
        [.. rows.SelectMany(row => cols.Select(col => new Elimination(row, col, candidate)))];
}
