namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class XWingTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Advanced;
    private static readonly int _n = 2;
    private readonly FishTechnique _fishTechnique = new(_n);

    public int TryApply(Puzzle puzzle)
    {
        return _fishTechnique.TryApply(puzzle);
    }
}

