using Sudoku.Core;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class TurbotFishTechniqueTests
{
    [Fact]
    public void TryApply_SwsChain_RemovesCandidatesSeeingBothEndpoints()
    {
        // One-based chain: (2,2) --strong-- (2,7) --weak-- (5,7) --strong-- (5,3).
        (int Row, int Column)[] candidateCells =
        [
            (1, 1), (1, 6),
            (4, 2), (4, 6), (6, 6),
            (0, 2), (2, 2), (3, 1), (5, 1),
        ];
        var puzzle = PuzzleFactory.Empty();
        var keptCells = candidateCells.ToHashSet();

        foreach (var (row, column, cell) in puzzle.Board.EnumerateEmptyCells())
            if (!keptCells.Contains((row, column)))
                cell.RemoveCandidate(Candidate);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);

        var applied = new TurbotFishTechnique().TryApply(puzzle);

        Assert.Equal(4, applied);
        CandidateAssert.Eliminated(snapshot, puzzle.Board,
            new Elimination(0, 2, Candidate),
            new Elimination(2, 2, Candidate),
            new Elimination(3, 1, Candidate),
            new Elimination(5, 1, Candidate));
        CandidateAssert.AppliedMatchesDiff(applied, snapshot, puzzle.Board);
    }

    private const byte Candidate = 5;
}