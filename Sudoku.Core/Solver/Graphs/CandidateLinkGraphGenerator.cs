using Sudoku.Core.Grid;

namespace Sudoku.Core.Solver.Graphs;

public enum CandidateLinkType
{
    Strong,
    Weak,
}

public static class CandidateLinkGraphGenerator
{
    public static CandidateLinkGraph GenerateStrongGraph(Puzzle puzzle)
    {
        return Generate(puzzle, includeWeakLinks: false);
    }

    public static CandidateLinkGraph GenerateStrongAndWeakGraph(Puzzle puzzle)
    {
        return Generate(puzzle, includeWeakLinks: true);
    }

    private static CandidateLinkGraph Generate(
        Puzzle puzzle,
        bool includeWeakLinks)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        var graphs = new CandidateLinkGraph(puzzle.Board.Size);

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
                    graphs.AddLink(candidate, cells[0], cells[1], CandidateLinkType.Strong);
                    continue;
                }

                if (!includeWeakLinks || cells.Count < 2)
                    continue;

                for (var firstIndex = 0; firstIndex < cells.Count; firstIndex++)
                {
                    for (var secondIndex = firstIndex + 1; secondIndex < cells.Count; secondIndex++)
                        graphs.AddLink(candidate, cells[firstIndex], cells[secondIndex], CandidateLinkType.Weak);
                }
            }
        }

        return graphs;
    }
}