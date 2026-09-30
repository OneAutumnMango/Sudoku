using Sudoku.Core;
using Sudoku.Core.Solver.Graphs;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class CandidateLinkGraphGeneratorTests
{
    [Fact]
    public void GenerateStrongGraph_IncludesConjugatePairsOnly()
    {
        var puzzle = WithCandidateOnlyIn((0, 0), (0, 1), (1, 2));

        var graph = CandidateLinkGraphGenerator.GenerateStrongGraph(puzzle)[1];

        Assert.Equal(CandidateLinkType.Strong, graph.GetLinks(puzzle.Board[0, 0])[puzzle.Board[0, 1]]);
        Assert.False(graph.GetLinks(puzzle.Board[0, 0]).ContainsKey(puzzle.Board[1, 2]));
    }

    [Fact]
    public void GenerateStrongAndWeakGraph_UsesStrongLinksExclusively()
    {
        var puzzle = WithCandidateOnlyIn((0, 0), (0, 1), (1, 2));

        var graph = CandidateLinkGraphGenerator.GenerateStrongAndWeakGraph(puzzle)[1];

        Assert.Equal(CandidateLinkType.Strong, graph.GetLinks(puzzle.Board[0, 0])[puzzle.Board[0, 1]]);
        Assert.Equal(CandidateLinkType.Weak, graph.GetLinks(puzzle.Board[0, 0])[puzzle.Board[1, 2]]);
        Assert.Equal(CandidateLinkType.Weak, graph.GetLinks(puzzle.Board[1, 2])[puzzle.Board[0, 1]]);
    }

    [Fact]
    public void GenerateStrongGraph_WhenPuzzleIsNull_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => CandidateLinkGraphGenerator.GenerateStrongGraph(null!));
    }

    private static Puzzle WithCandidateOnlyIn(params (int Row, int Column)[] cells)
    {
        var puzzle = PuzzleFactory.Empty();
        var keptCells = cells.ToHashSet();

        foreach (var (row, column, cell) in puzzle.Board.EnumerateEmptyCells())
            cell.IntersectCandidates(keptCells.Contains((row, column)) ? (ushort)1 : (ushort)0);

        return puzzle;
    }
}