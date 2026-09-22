namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class SwordfishTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Expert;
    private static readonly int _n = 3;
    private readonly FishTechnique _fishTechnique = new(_n);

    public int TryApply(Puzzle puzzle)
    {
        return _fishTechnique.TryApply(puzzle);
    }
}

