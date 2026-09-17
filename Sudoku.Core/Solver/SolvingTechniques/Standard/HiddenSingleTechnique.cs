namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class HiddenSingleTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Easy;

    public void TryApply(Puzzle puzzle)
    {
        // Intentionally not implemented.
    }
}

