namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class HiddenSingleTechnique : ISolvingTechnique
{
    private readonly HiddenNTechnique _hiddenNTechnique = new(1);

    public Difficulty Difficulty => Difficulty.Simple;

    public int TryApply(Puzzle puzzle)
    {
        return _hiddenNTechnique.TryApply(puzzle);
    }
}

