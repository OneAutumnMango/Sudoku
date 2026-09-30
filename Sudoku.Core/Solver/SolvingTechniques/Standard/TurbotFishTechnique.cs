using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver.Graphs;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public class TurbotFishTechnique : ISolvingTechnique
{
    public Difficulty Difficulty { get; } = Difficulty.Advanced;
    private IReadOnlyList<CandidateChainEdgeType> pattern = [CandidateChainEdgeType.Strong, CandidateChainEdgeType.Weak, CandidateChainEdgeType.Strong];

    public int TryApply(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        if (puzzle.RuleSet is not IStandardRuleSet ruleSet)
            throw new ArgumentException(
                "Turbot Fish technique requires standard Sudoku rules.",
                nameof(puzzle));

        int applied = 0;

        var graphs = CandidateChainGraphGenerator.GenerateStrongAndWeakGraph(puzzle);

        foreach (var (cand, graph) in graphs)
        {
            var matches = CandidateChainPatternMatcher.Match(graph, pattern);

            foreach (var match in matches)
            {
                // skip matches where cands were removed from another match
                if (match.Vertices.Any(v => v.Value != 0 || !v.HasCandidate(cand)))
                    continue;

                var ends = match.Vertices
                    .Where(vertex => match.GetEdges(vertex).Count == 1)
                    .ToArray();

                var cellsSeeingFirstEnd = ruleSet.GetContainingConstraints(ends[0])
                    .SelectMany(constraint => constraint.Cells)
                    .ToHashSet();

                var cellsSeeingSecondEnd = ruleSet.GetContainingConstraints(ends[1])
                    .SelectMany(constraint => constraint.Cells)
                    .ToHashSet();

                cellsSeeingFirstEnd.IntersectWith(cellsSeeingSecondEnd);
                cellsSeeingFirstEnd.ExceptWith(match.Vertices);

                foreach (var cell in cellsSeeingFirstEnd)
                {
                    if (cell.Value != 0 || !cell.HasCandidate(cand))
                        continue;

                    cell.RemoveCandidate(cand);
                    applied++;
                }
            }
        }

        return applied;
    }
}