namespace Sudoku.Core.Solver.SolvingTechniques;

public interface ISolvingTechnique
{
    string Name => GetType().Name;
    Difficulty Difficulty { get; }
    bool TryApply(Puzzle puzzle);
}