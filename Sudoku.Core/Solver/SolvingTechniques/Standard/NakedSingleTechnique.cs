namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class NakedSingleTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Simple;

    public bool TryApply(Puzzle puzzle)
    {
        var board = puzzle.Board;
        var ruleset = puzzle.RuleSet;
        var applied = false;

        ruleset.ComputeAndFillCandidates(board);

        var cells = board.EnumerateEmptyCells()
            .Select(_ => _.cell)
            .Select(cell => (cell, candidates: cell.GetCandidates()))
            .Where(cell => cell.candidates.Count() == 1);

        foreach (var (cell, candidates) in cells)
        {
            cell.Value = candidates.First();
            applied = true;
        }

        return applied;
    }
}
