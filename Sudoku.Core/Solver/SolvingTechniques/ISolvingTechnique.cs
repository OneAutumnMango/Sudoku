namespace Sudoku.Core.Solver.SolvingTechniques;

public interface ISolvingTechnique
{
    string Name => GetType().Name;
    Difficulty Difficulty { get; }
    int TryApply(Puzzle puzzle);
}