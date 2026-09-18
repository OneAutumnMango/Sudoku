namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class NakedSingleTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Simple;

    public int TryApply(Puzzle puzzle)
    {
        var board = puzzle.Board;
        var ruleset = puzzle.RuleSet;
        var changed = 0;

        ruleset.ComputeAndFillCandidates(board);

        var singles = board.EnumerateEmptyCells()
            .Select(_ => _.cell)
            .Select(cell => (cell, candidate: cell.GetCandidates().ToList()))
            .Where(item => item.candidate.Count == 1)
            .ToList();

        foreach (var (cell, candidate) in singles)
        {
            ruleset.ComputeAndFillCandidates(board);

            // protect against stale candidates
            var currentCandidates = cell.GetCandidates().ToList();
            if (currentCandidates.Count != 1)
                continue;

            cell.Value = currentCandidates[0];
            changed++;
        }

        return changed;
    }
}
