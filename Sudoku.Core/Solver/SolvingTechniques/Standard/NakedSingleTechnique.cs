namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class NakedSingleTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Simple;

    public void TryApply(Puzzle puzzle)
    {
        // Intentionally not implemented.
    }
}
