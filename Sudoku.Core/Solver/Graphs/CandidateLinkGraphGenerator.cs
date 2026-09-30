using Sudoku.Core.Grid;

namespace Sudoku.Core.Solver.Graphs;

public enum CandidateLinkType
{
    Strong,
    Weak,
}

public static class CandidateLinkGraphGenerator
{
    public static Dictionary<byte, Dictionary<Cell, Dictionary<Cell, CandidateLinkType>>> GenerateStrongGraph(
        Puzzle puzzle)
    {
        return Generate(puzzle, includeWeakLinks: false);
    }

    public static Dictionary<byte, Dictionary<Cell, Dictionary<Cell, CandidateLinkType>>> GenerateStrongAndWeakGraph(
        Puzzle puzzle)
    {
        return Generate(puzzle, includeWeakLinks: true);
    }

    private static Dictionary<byte, Dictionary<Cell, Dictionary<Cell, CandidateLinkType>>> Generate(
        Puzzle puzzle,
        bool includeWeakLinks)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        var graphs = Enumerable.Range(1, puzzle.Board.Size)
            .Select(candidate => (byte)candidate)
            .ToDictionary(
                candidate => candidate,
                _ => new Dictionary<Cell, Dictionary<Cell, CandidateLinkType>>());

        foreach (var constraint in puzzle.RuleSet.GetConstraints())
        {
            var cellsByCandidate = Enumerable.Range(1, puzzle.Board.Size)
                .Select(candidate => (byte)candidate)
                .ToDictionary(candidate => candidate, _ => new List<Cell>());

            foreach (var cell in constraint.Cells)
            {
                if (cell.Value != 0)
                    continue;

                foreach (var candidate in cell.GetCandidates())
                    cellsByCandidate[candidate].Add(cell);
            }

            foreach (var (candidate, cells) in cellsByCandidate)
            {
                if (cells.Count == 2)
                {
                    AddLink(graphs[candidate], cells[0], cells[1], CandidateLinkType.Strong);
                    continue;
                }

                if (!includeWeakLinks || cells.Count < 2)
                    continue;

                for (var firstIndex = 0; firstIndex < cells.Count; firstIndex++)
                {
                    for (var secondIndex = firstIndex + 1; secondIndex < cells.Count; secondIndex++)
                        AddLink(graphs[candidate], cells[firstIndex], cells[secondIndex], CandidateLinkType.Weak);
                }
            }
        }

        return graphs;
    }

    private static void AddLink(
        Dictionary<Cell, Dictionary<Cell, CandidateLinkType>> graph,
        Cell first,
        Cell second,
        CandidateLinkType linkType)
    {
        AddDirectedLink(first, second);
        AddDirectedLink(second, first);

        void AddDirectedLink(Cell from, Cell to)
        {
            if (!graph.TryGetValue(from, out var neighbours))
            {
                neighbours = [];
                graph[from] = neighbours;
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