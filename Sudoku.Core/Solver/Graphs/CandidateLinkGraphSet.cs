namespace Sudoku.Core.Solver.Graphs;

public sealed class CandidateLinkGraphSet
{
    private readonly Dictionary<byte, CandidateLinkGraph> _graphs;

    public CandidateLinkGraphSet(int candidateCount)
    {
        _graphs = Enumerable.Range(1, candidateCount)
            .Select(candidate => (byte)candidate)
            .ToDictionary(candidate => candidate, candidate => new CandidateLinkGraph(candidate));
    }

    public IReadOnlyCollection<byte> Candidates => _graphs.Keys;

    public CandidateLinkGraph this[byte candidate] => _graphs[candidate];
}