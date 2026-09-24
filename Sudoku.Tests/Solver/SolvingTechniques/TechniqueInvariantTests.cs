using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques;

public class TechniqueInvariantTests
{
    [Theory]
    [MemberData(nameof(TechniqueCatalog.AllNames), MemberType = typeof(TechniqueCatalog))]
    public void TryApply_OnEmptyBoard_ReturnsZero(string name)
    {
        var puzzle = PuzzleFactory.Empty();

        Assert.Equal(0, TechniqueCatalog.Create(name, puzzle).TryApply(puzzle));
    }

    [Theory]
    [MemberData(nameof(TechniqueCatalog.AllNames), MemberType = typeof(TechniqueCatalog))]
    public void TryApply_OnSolvedBoard_ReturnsZero(string name)
    {
        var puzzle = PuzzleFactory.Solved();

        Assert.Equal(0, TechniqueCatalog.Create(name, puzzle).TryApply(puzzle));
    }

    [Theory]
    [MemberData(nameof(TechniqueCatalog.AllNames), MemberType = typeof(TechniqueCatalog))]
    public void TryApply_NeverTouchesAFilledCell(string name)
    {
        if (TechniqueCatalog.PlacesValues(name))
            return;

        foreach (var entry in PuzzleCorpus.Take(20))
        {
            var puzzle = PuzzleFactory.FromString(entry.Puzzle);
            var snapshot = CandidateSnapshot.Capture(puzzle.Board);

            TechniqueCatalog.Create(name, puzzle).TryApply(puzzle);

            CandidateAssert.NoFilledCellTouched(snapshot, puzzle.Board);
        }
    }

    [Theory]
    [MemberData(nameof(TechniqueCatalog.AllNames), MemberType = typeof(TechniqueCatalog))]
    public void TryApply_NeverRemovesASolutionCandidate(string name)
    {
        foreach (var entry in PuzzleCorpus.Take(20))
        {
            var puzzle = PuzzleFactory.FromString(entry.Puzzle);

            for (var pass = 0; pass < 20; pass++)
            {
                if (TechniqueCatalog.Create(name, puzzle).TryApply(puzzle) == 0)
                    break;
            }

            SolutionInvariant.AssertPreserved(puzzle, entry.Solution, $"{name} broke the solution:");
        }
    }

    [Theory]
    [MemberData(nameof(TechniqueCatalog.AllNames), MemberType = typeof(TechniqueCatalog))]
    public void TryApply_ReturnValueMatchesItsDocumentedUnit(string name)
    {
        if (!TechniqueCatalog.CountsEliminations(name))
            return;

        foreach (var entry in PuzzleCorpus.Take(20))
        {
            var puzzle = PuzzleFactory.FromString(entry.Puzzle);
            var snapshot = CandidateSnapshot.Capture(puzzle.Board);

            var applied = TechniqueCatalog.Create(name, puzzle).TryApply(puzzle);

            CandidateAssert.AppliedMatchesDiff(applied, snapshot, puzzle.Board);
        }
    }
}
