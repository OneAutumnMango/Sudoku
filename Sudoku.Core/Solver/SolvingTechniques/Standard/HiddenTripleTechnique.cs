namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class HiddenTripleTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Advanced;
    private static readonly int _n = 3;
    private readonly HiddenNTechnique _hiddenNTechnique = new(_n);

    public int TryApply(Puzzle puzzle)
    {
        return _hiddenNTechnique.TryApply(puzzle);
    }
}

