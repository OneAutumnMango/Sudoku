using System.Numerics;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class NakedPairTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Intermediate;
    private static readonly int _n = 2;
    private readonly NakedNTechnique _nakedNTechnique = new(_n);

    public int TryApply(Puzzle puzzle)
    {
        return _nakedNTechnique.TryApply(puzzle);
    }
}

