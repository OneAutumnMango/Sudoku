using Sudoku.Core;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class SimpleColouringTechniqueTests
{
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
        Assert.Contains("candidate 4", Assert.Single(technique.Explanations));
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