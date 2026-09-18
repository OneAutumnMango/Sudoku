namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class NakedSingleTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Simple;

    public int TryApply(Puzzle puzzle)
    {
        var board = puzzle.Board;
        var changed = 0;

        var singles = board.EnumerateEmptyCells()
            .Select(_ => _.cell)
            .Select(cell => (cell, candidate: cell.GetCandidates().ToList()))
            .Where(item => item.candidate.Count == 1)
            .ToList();

        foreach (var (cell, candidate) in singles)
        {
            // protect against stale candidates
            var currentCandidates = cell.GetCandidates().ToList();
            if (currentCandidates.Count != 1)
                continue;

            if (puzzle.UpdateCell(cell, currentCandidates[0]))
                changed++;
        }

        return changed;
    }
}
