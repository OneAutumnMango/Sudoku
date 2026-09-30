using Sudoku.Core.Grid;
using Sudoku.Core.Solver.Graphs;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class CandidateChainPatternMatcherTests
{
    [Fact]
    public void Match_ReturnsEachSwsPathOnceAsASubgraph()
    {
        var graph = new CandidateChainGraph(5);
        var first = new Cell();
        var second = new Cell();
        var third = new Cell();
        var fourth = new Cell();

        graph.AddEdge(first, second, CandidateChainEdgeType.Strong);
        graph.AddEdge(second, third, CandidateChainEdgeType.Weak);
        graph.AddEdge(third, fourth, CandidateChainEdgeType.Strong);

        var matches = CandidateChainPatternMatcher.Match(
            graph,
            [CandidateChainEdgeType.Strong, CandidateChainEdgeType.Weak, CandidateChainEdgeType.Strong]);

        var match = Assert.Single(matches);
        Assert.Equal(5, match.Candidate);
        Assert.Equal(4, match.Vertices.Count);
        Assert.Equal(CandidateChainEdgeType.Strong, match.GetEdges(first)[second]);
        Assert.Equal(CandidateChainEdgeType.Weak, match.GetEdges(second)[third]);
        Assert.Equal(CandidateChainEdgeType.Strong, match.GetEdges(third)[fourth]);
    }
}