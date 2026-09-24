using Sudoku.Core;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class HiddenSingleTechniqueTests
{
    [Fact]
    public void TryApply_WhenCandidateAppearsOnceInARow_ReducesThatCellWithoutPlacingIt()
    {
        var puzzle = PuzzleFactory.Empty();
        ConfineToCell(puzzle, 1, row: 0, col: 0);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new HiddenSingleTechnique().TryApply(puzzle);

        Assert.Equal(8, applied);
        CandidateAssert.HasCandidates(puzzle, 0, 0, 1);
        Assert.Equal(0, puzzle.Board[0, 0].Value);
        Assert.Equal(8, snapshot.TotalRemoved(puzzle.Board));
    }

    [Fact]
    public void TryApply_WhenCandidateAppearsOnceInABox_ReducesThatCell()
    {
        var puzzle = PuzzleFactory.Empty();

        for (var row = 0; row < 3; row++)
        {
            for (var col = 0; col < 3; col++)
            {
                if (row != 1 || col != 1)
                    puzzle.Board[row, col].RemoveCandidate(5);
            }
        }

        var applied = new HiddenSingleTechnique().TryApply(puzzle);

        Assert.Equal(8, applied);
        CandidateAssert.HasCandidates(puzzle, 1, 1, 5);
    }

    [Fact]
    public void TryApply_WhenSeveralHiddenSinglesExist_ReducesEachOne()
    {
        var puzzle = PuzzleFactory.Empty();
        ConfineToCell(puzzle, 1, row: 0, col: 0);
        ConfineToCell(puzzle, 2, row: 8, col: 8);

        var applied = new HiddenSingleTechnique().TryApply(puzzle);

        Assert.Equal(16, applied);
        CandidateAssert.HasCandidates(puzzle, 0, 0, 1);
        CandidateAssert.HasCandidates(puzzle, 8, 8, 2);
    }

    [Fact]
    public void TryApply_WhenCalledTwice_SecondCallReturnsZero()
    {
        var puzzle = PuzzleFactory.Empty();
        ConfineToCell(puzzle, 1, row: 0, col: 0);

        var technique = new HiddenSingleTechnique();

        Assert.Equal(8, technique.TryApply(puzzle));
        Assert.Equal(0, technique.TryApply(puzzle));
        CandidateAssert.HasCandidates(puzzle, 0, 0, 1);
    }

    [Fact]
    public void TryApply_WhenNoCandidateIsUniqueInAGroup_ReturnsZero()
    {
        Assert.Equal(0, new HiddenSingleTechnique().TryApply(PuzzleFactory.Empty()));
    }

    [Fact]
    public void TryApply_OnCorpusPuzzles_NeverRemovesASolutionCandidate()
    {
        foreach (var entry in PuzzleCorpus.Take(25))
        {
            var puzzle = PuzzleFactory.FromString(entry.Puzzle);

            new HiddenSingleTechnique().TryApply(puzzle);

            SolutionInvariant.AssertPreserved(puzzle, entry.Solution, "Hidden single broke the solution:");
        }
    }

    /// <summary>Leaves <paramref name="candidate"/> in only one cell of the given row.</summary>
    private static void ConfineToCell(Puzzle puzzle, byte candidate, int row, int col)
    {
        for (var other = 0; other < 9; other++)
        {
            if (other != col)
                puzzle.Board[row, other].RemoveCandidate(candidate);
        }
    }
}
