namespace Sudoku.Core.Solver.SolvingTechniques;

public interface ISolvingTechnique
{
    string Name => GetType().Name;
    Difficulty Difficulty { get; }
    void TryApply(Puzzle puzzle);
}