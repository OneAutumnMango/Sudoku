namespace Sudoku.Core.Solver.SolvingTechniques;

public interface ISolvingTechnique
{
    string Name => GetType().Name;
    Difficulty Difficulty { get; }

    /// <summary>
    /// Applies the technique everywhere it fits and returns the number of candidates eliminated,
    /// or the number of values placed for techniques that solve cells outright.
    /// </summary>
    int TryApply(Puzzle puzzle);
}