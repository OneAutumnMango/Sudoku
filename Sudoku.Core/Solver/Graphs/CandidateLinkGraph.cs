using Sudoku.Core.Grid;

namespace Sudoku.Core.Solver.Graphs;

public sealed class CandidateLinkGraph
{
    private readonly Dictionary<Cell, Dictionary<Cell, CandidateLinkType>> _links = [];

    public byte Candidate { get; }
    public IReadOnlyCollection<Cell> Cells => _links.Keys;

    public CandidateLinkGraph(byte candidate)
    {
        Candidate = candidate;
    }

    public IReadOnlyDictionary<Cell, CandidateLinkType> GetLinks(Cell cell) => _links[cell];

    public void AddLink(Cell first, Cell second, CandidateLinkType linkType)
    {
        AddDirectedLink(first, second);
        AddDirectedLink(second, first);

        void AddDirectedLink(Cell from, Cell to)
        {
            if (!_links.TryGetValue(from, out var neighbours))
            {
                neighbours = [];
                _links[from] = neighbours;
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