namespace Sudoku.Core.Solver.Graphs;

public sealed class CandidateChainGraphSet
{
    private readonly Dictionary<byte, CandidateChainGraph> _graphs;

    public CandidateChainGraphSet(int candidateCount)
    {
        _graphs = Enumerable.Range(1, candidateCount)
            .Select(candidate => (byte)candidate)
            .ToDictionary(candidate => candidate, candidate => new CandidateChainGraph(candidate));
    }

    public IReadOnlyCollection<byte> Candidates => _graphs.Keys;

    public CandidateChainGraph this[byte candidate] => _graphs[candidate];
}