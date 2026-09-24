using Sudoku.Core;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class GroupIntersectionTechniqueTests
{
    [Fact]
    public void TryApply_WhenBoxCandidateIsConfinedToARow_RemovesItFromTheRestOfThatRow()
    {
        var puzzle = PuzzleFactory.Empty();
        ConfineInBox(puzzle, 0, 4, (0, 0), (0, 1));

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = Pointing(puzzle).TryApply(puzzle);

        Assert.Equal(6, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            Removals(4, (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8)));
        CandidateAssert.AppliedMatchesDiff(applied, snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenBoxCandidateIsConfinedToAColumn_RemovesItFromTheRestOfThatColumn()
    {
        var puzzle = PuzzleFactory.Empty();
        ConfineInBox(puzzle, 0, 4, (0, 0), (1, 0), (2, 0));

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = Pointing(puzzle).TryApply(puzzle);

        Assert.Equal(6, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            Removals(4, (3, 0), (4, 0), (5, 0), (6, 0), (7, 0), (8, 0)));
    }

    [Fact]
    public void TryApply_WhenBoxCandidateCellsShareNoLine_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty();
        ConfineInBox(puzzle, 0, 4, (0, 0), (1, 1));

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        Assert.Equal(0, Pointing(puzzle).TryApply(puzzle));
        CandidateAssert.NothingEliminated(snapshot, puzzle.Board);
    }

    [Fact]
    public void TryApply_WhenCandidateSpansMoreCellsThanAnIntersection_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty();
        ConfineInBox(puzzle, 0, 4, (0, 0), (0, 1), (0, 2), (1, 0));

        Assert.Equal(0, Pointing(puzzle).TryApply(puzzle));
    }

    [Fact]
    public void TryApply_WhenCandidateIsConfinedToASingleCell_ReturnsZero()
    {
        // a single cell is a hidden single, which this technique deliberately ignores
        var puzzle = PuzzleFactory.Empty();
        ConfineInBox(puzzle, 0, 4, (0, 0));

        Assert.Equal(0, Pointing(puzzle).TryApply(puzzle));
    }

    [Fact]
    public void TryApply_WhenRowCandidateIsConfinedToABox_RemovesItFromTheRestOfThatBox()
    {
        var puzzle = PuzzleFactory.Empty();
        ConfineInRow(puzzle, 0, 4, 0, 1);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = BoxLine(puzzle).TryApply(puzzle);

        Assert.Equal(6, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            Removals(4, (1, 0), (1, 1), (1, 2), (2, 0), (2, 1), (2, 2)));
    }

    [Fact]
    public void TryApply_WhenColumnCandidateIsConfinedToABox_RemovesItFromTheRestOfThatBox()
    {
        var puzzle = PuzzleFactory.Empty();
        ConfineInColumn(puzzle, 0, 4, 0, 1);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = BoxLine(puzzle).TryApply(puzzle);

        Assert.Equal(6, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            Removals(4, (0, 1), (0, 2), (1, 1), (1, 2), (2, 1), (2, 2)));
    }

    [Fact]
    public void TryApply_PointingAndBoxLineUseOppositeReferenceGroups()
    {
        var pointingBoard = PuzzleFactory.Empty();
        ConfineInBox(pointingBoard, 0, 4, (0, 0), (0, 1));

        var boxLineBoard = PuzzleFactory.Empty();
        ConfineInBox(boxLineBoard, 0, 4, (0, 0), (0, 1));

        Assert.Equal(6, Pointing(pointingBoard).TryApply(pointingBoard));
        Assert.Equal(0, BoxLine(boxLineBoard).TryApply(boxLineBoard));
    }

    [Fact]
    public void TryApply_DoesNotEliminateFromFilledCells()
    {
        var puzzle = PuzzleFactory.Empty();
        puzzle.SetCell(0, 7, 9);
        ConfineInBox(puzzle, 0, 4, (0, 0), (0, 1));

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = Pointing(puzzle).TryApply(puzzle);

        Assert.Equal(5, applied);
        CandidateAssert.NoFilledCellTouched(snapshot, puzzle.Board);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            Removals(4, (0, 3), (0, 4), (0, 5), (0, 6), (0, 8)));
    }

    [Fact]
    public void TryApply_WhenCalledTwice_SecondCallReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty();
        ConfineInBox(puzzle, 0, 4, (0, 0), (0, 1));

        Assert.Equal(6, Pointing(puzzle).TryApply(puzzle));
        Assert.Equal(0, Pointing(puzzle).TryApply(puzzle));
    }

    [Fact]
    public void TryApply_OnEmptyBoard_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty();

        Assert.Equal(0, Pointing(puzzle).TryApply(puzzle));
        Assert.Equal(0, BoxLine(puzzle).TryApply(puzzle));
    }

    [Fact]
    public void TryApply_OnSolvedBoard_ReturnsZero()
    {
        var puzzle = PuzzleFactory.Solved();

        Assert.Equal(0, Pointing(puzzle).TryApply(puzzle));
        Assert.Equal(0, BoxLine(puzzle).TryApply(puzzle));
    }

    [Fact]
    public void TryApply_WhenPuzzleIsNull_Throws()
    {
        var technique = Pointing(PuzzleFactory.Empty());

        Assert.Throws<ArgumentNullException>(() => technique.TryApply(null!));
    }

    private static GroupIntersectionTechnique Pointing(Puzzle puzzle)
    {
        var ruleSet = (IStandardRuleSet)puzzle.RuleSet;
        return new(ruleSet.BoxConstraints, ruleSet.RowConstraints.Concat(ruleSet.ColumnConstraints));
    }

    private static GroupIntersectionTechnique BoxLine(Puzzle puzzle)
    {
        var ruleSet = (IStandardRuleSet)puzzle.RuleSet;
        return new(ruleSet.RowConstraints.Concat(ruleSet.ColumnConstraints), ruleSet.BoxConstraints);
    }

    /// <summary>Leaves <paramref name="candidate"/> only in the given cells of the box at (boxRow*3, boxCol*3).</summary>
    private static void ConfineInBox(Puzzle puzzle, int box, byte candidate, params (int Row, int Col)[] keep)
    {
        var baseRow = box / 3 * 3;
        var baseCol = box % 3 * 3;

        for (var row = baseRow; row < baseRow + 3; row++)
        {
            for (var col = baseCol; col < baseCol + 3; col++)
            {
                if (!keep.Contains((row, col)))
                    puzzle.Board[row, col].RemoveCandidate(candidate);
            }
        }
    }

    private static void ConfineInRow(Puzzle puzzle, int row, byte candidate, params int[] keepColumns)
    {
        for (var col = 0; col < 9; col++)
        {
            if (!keepColumns.Contains(col))
                puzzle.Board[row, col].RemoveCandidate(candidate);
        }
    }

    private static void ConfineInColumn(Puzzle puzzle, int col, byte candidate, params int[] keepRows)
    {
        for (var row = 0; row < 9; row++)
        {
            if (!keepRows.Contains(row))
                puzzle.Board[row, col].RemoveCandidate(candidate);
        }
    }

    private static Elimination[] Removals(byte candidate, params (int Row, int Col)[] cells) =>
        [.. cells.Select(cell => new Elimination(cell.Row, cell.Col, candidate))];
}
