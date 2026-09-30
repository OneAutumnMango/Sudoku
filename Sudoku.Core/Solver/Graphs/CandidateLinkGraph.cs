using Sudoku.Core.Grid;

namespace Sudoku.Core.Solver.Graphs;

public sealed class CandidateLinkGraph
{
    private readonly Dictionary<byte, Dictionary<Cell, Dictionary<Cell, CandidateLinkType>>> _graphs;

    public CandidateLinkGraph(int candidateCount)
    {
        _graphs = Enumerable.Range(1, candidateCount)
            .Select(candidate => (byte)candidate)
            .ToDictionary(
                candidate => candidate,
                _ => new Dictionary<Cell, Dictionary<Cell, CandidateLinkType>>());
    }

    public IReadOnlyDictionary<Cell, Dictionary<Cell, CandidateLinkType>> this[byte candidate] =>
        _graphs[candidate];

    public void AddLink(byte candidate, Cell first, Cell second, CandidateLinkType linkType)
    {
        AddDirectedLink(first, second);
        AddDirectedLink(second, first);

        void AddDirectedLink(Cell from, Cell to)
        {
            if (!_graphs[candidate].TryGetValue(from, out var neighbours))
            {
                neighbours = [];
                _graphs[candidate][from] = neighbours;
            }

            if (!neighbours.TryGetValue(to, out var existingType)
                || linkType == CandidateLinkType.Strong
                || existingType != CandidateLinkType.Strong)
            {
                neighbours[to] = linkType;
            }
        }
    }
}