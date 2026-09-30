using Sudoku.Core.Grid;

namespace Sudoku.Core.Solver.Graphs;

public sealed class CandidateChainGraph
{
    private readonly Dictionary<Cell, Dictionary<Cell, CandidateChainEdgeType>> _edges = [];

    public byte Candidate { get; }
    public IReadOnlyCollection<Cell> Vertices => _edges.Keys;

    public CandidateChainGraph(byte candidate)
    {
        Candidate = candidate;
    }

    public IReadOnlyDictionary<Cell, CandidateChainEdgeType> GetEdges(Cell vertex) => _edges[vertex];

    public void AddEdge(Cell first, Cell second, CandidateChainEdgeType edgeType)
    {
        AddDirectedEdge(first, second);
        AddDirectedEdge(second, first);

        void AddDirectedEdge(Cell from, Cell to)
        {
            if (!_edges.TryGetValue(from, out var neighbours))
            {
                neighbours = [];
                _edges[from] = neighbours;
            }

            if (!neighbours.TryGetValue(to, out var existingType)
                || edgeType == CandidateChainEdgeType.Strong
                || existingType != CandidateChainEdgeType.Strong)
            {
                neighbours[to] = edgeType;
            }
        }
    }
}