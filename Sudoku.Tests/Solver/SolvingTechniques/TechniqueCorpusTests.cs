using Sudoku.Core.Solver;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques;

public class TechniqueCorpusTests
{
    [Theory]
    [MemberData(nameof(Puzzles))]
    public void Solve_ReachesTheStoredSolutionAtTheStoredDifficulty(int index)
    {
        var entry = PuzzleCorpus.Get(index);
        var puzzle = PuzzleFactory.FromString(entry.Puzzle);

        var difficulty = new HumanlikeSolver(puzzle).Solve();

        Assert.True(difficulty.IsSome, $"Puzzle {index} was not solved.");
        Assert.Equal(entry.Solution, puzzle.Board.ToCompactString());
        Assert.Equal(entry.Difficulty, difficulty.Value);
    }

    public static IEnumerable<object[]> Puzzles => PuzzleCorpus.Sample(40);
}
