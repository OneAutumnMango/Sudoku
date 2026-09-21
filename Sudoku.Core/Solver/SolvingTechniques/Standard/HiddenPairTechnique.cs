namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class HiddenPairTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Easy;
    private static readonly int _n = 2;
    private readonly HiddenNTechnique _hiddenNTechnique = new(_n);

    public int TryApply(Puzzle puzzle)
    {
        return _hiddenNTechnique.TryApply(puzzle);
    }
}

