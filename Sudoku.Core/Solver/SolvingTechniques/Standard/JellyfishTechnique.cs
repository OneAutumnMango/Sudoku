namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class JellyfishTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Master;
    private static readonly int _n = 4;
    private readonly FishTechnique _fishTechnique = new(_n);

    public int TryApply(Puzzle puzzle)
    {
        return _fishTechnique.TryApply(puzzle);
    }
}

