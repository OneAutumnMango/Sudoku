using Sudoku.Core.Grid;

namespace Sudoku.Core.Solver.Graphs;

public static class CandidateChainPatternMatcher
{
    public static IReadOnlyList<CandidateChainGraph> Match(
        CandidateChainGraph graph,
        IReadOnlyList<CandidateChainEdgeType> pattern)
    {
        ArgumentNullException.ThrowIfNull(graph);
        ArgumentNullException.ThrowIfNull(pattern);

        if (pattern.Count == 0)
            throw new ArgumentException("A chain pattern must contain at least one step.", nameof(pattern));

        var matches = new List<CandidateChainGraph>();
        var matchedPaths = new HashSet<IReadOnlyList<Cell>>(CellPathComparer.Instance);

        foreach (var startingCell in graph.Vertices)
        {
            var path = new List<Cell> { startingCell };
            var visited = new HashSet<Cell> { startingCell };
            Search(graph, pattern, startingCell, path, visited, matches, matchedPaths, 0);
        }

        return matches;
    }

    // DFS to find paths matching the requested steps.
    private static void Search(
        CandidateChainGraph graph,
        IReadOnlyList<CandidateChainEdgeType> pattern,
        Cell current,
        List<Cell> path,
        HashSet<Cell> visited,
        List<CandidateChainGraph> matches,
        HashSet<IReadOnlyList<Cell>> matchedPaths,
        int patternIndex)
    {
        if (patternIndex == pattern.Count)
        {
            if (matchedPaths.Add(path.ToArray()))
            {
                var match = new CandidateChainGraph(graph.Candidate);
                for (var index = 0; index < pattern.Count; index++)
                    match.AddEdge(path[index], path[index + 1], pattern[index]);
                matches.Add(match);
            }

            return;
        }

        foreach (var (neighbour, edgeType) in graph.GetEdges(current))
        {
            if (edgeType != pattern[patternIndex] || !visited.Add(neighbour))
                continue;

            path.Add(neighbour);
            Search(graph, pattern, neighbour, path, visited, matches, matchedPaths, patternIndex + 1);
            path.RemoveAt(path.Count - 1);
            visited.Remove(neighbour);
        }
    }

    private sealed class CellPathComparer : IEqualityComparer<IReadOnlyList<Cell>>
    {
        public static CellPathComparer Instance { get; } = new();

        public bool Equals(IReadOnlyList<Cell>? first, IReadOnlyList<Cell>? second)
        {
            if (first is null || second is null || first.Count != second.Count)
                return false;

            var sameDirection = true;
            var reverseDirection = true;

            for (var index = 0; index < first.Count; index++)
            {
                sameDirection &= ReferenceEquals(first[index], second[index]);
                reverseDirection &= ReferenceEquals(first[index], second[second.Count - index - 1]);
            }

            return sameDirection || reverseDirection;
        }

        public int GetHashCode(IReadOnlyList<Cell> path)
        {
            var forwardHash = new HashCode();
            var reverseHash = new HashCode();

            foreach (var cell in path)
                forwardHash.Add(cell);

            for (var index = path.Count - 1; index >= 0; index--)
                reverseHash.Add(path[index]);

            return Math.Min(forwardHash.ToHashCode(), reverseHash.ToHashCode());
        }
    }
}