namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class NakedPairTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Intermediate;

    public bool TryApply(Puzzle puzzle)
    {
        // Intentionally not implemented.
        return false;
    }
}

