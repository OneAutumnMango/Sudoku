using System.Numerics;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class NakedTripleTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Intermediate;
    private readonly int _n = 3;

    public int TryApply(Puzzle puzzle)
    {
        return new NakedNTechnique(_n).TryApply(puzzle);
    }
}

