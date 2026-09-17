namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class GuessTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Expert;

    public void TryApply(Puzzle puzzle)
    {
        // Intentionally not implemented.
    }
}

