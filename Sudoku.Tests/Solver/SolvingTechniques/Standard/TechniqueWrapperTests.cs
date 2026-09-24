using Sudoku.Core;
using Sudoku.Core.Solver.SolvingTechniques.Standard;
using Sudoku.Tests.TestUtils;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class TechniqueWrapperTests
{
    [Theory]
    [MemberData(nameof(TechniqueCatalog.WrapperNames), MemberType = typeof(TechniqueCatalog))]
    public void Wrapper_ExposesExpectedNameAndDifficulty(string name)
    {
        var technique = TechniqueCatalog.Create(name, PuzzleFactory.Empty());

        Assert.Equal(name, technique.Name);
        Assert.Equal(TechniqueCatalog.ExpectedDifficulty(name), technique.Difficulty);
    }

    [Theory]
    [MemberData(nameof(TechniqueCatalog.Delegations), MemberType = typeof(TechniqueCatalog))]
    public void Wrapper_ProducesTheSameResultAsTheEngineItDelegatesTo(string wrapper, string engine)
    {
        var wrapperPuzzle = Scenario(wrapper);
        var enginePuzzle = Scenario(wrapper);

        var wrapperSnapshot = CandidateSnapshot.Capture(wrapperPuzzle.Board);
        var engineSnapshot = CandidateSnapshot.Capture(enginePuzzle.Board);

        var wrapperApplied = TechniqueCatalog.Create(wrapper, wrapperPuzzle).TryApply(wrapperPuzzle);
        var engineApplied = TechniqueCatalog.Create(engine, enginePuzzle).TryApply(enginePuzzle);

        Assert.True(wrapperApplied > 0, $"{wrapper} scenario did not trigger the technique.");
        Assert.Equal(engineApplied, wrapperApplied);
        CandidateAssert.Eliminated(wrapperSnapshot, wrapperPuzzle.Board,
            [.. engineSnapshot.RemovedSince(enginePuzzle.Board)]);
    }

    [Fact]
    public void NakedSingleTechnique_PlacesEveryCellThatHasOneCandidateLeft()
    {
        var puzzle = PuzzleFactory.Empty()
            .WithCandidates(0, 0, 7)
            .WithCandidates(4, 4, 3);

        var snapshot = CandidateSnapshot.Capture(puzzle.Board);
        var applied = new NakedSingleTechnique().TryApply(puzzle);
        var placed = snapshot.PlacedSince(puzzle.Board);

        Assert.Equal(2, applied);
        Assert.Equal(2, placed.Count);
        Assert.Contains((0, 0, (byte)7), placed);
        Assert.Contains((4, 4, (byte)3), placed);
    }

    [Fact]
    public void NakedSingleTechnique_WhenNoCellHasASingleCandidate_ReturnsZero()
    {
        Assert.Equal(0, new NakedSingleTechnique().TryApply(PuzzleFactory.Empty()));
    }

    [Fact]
    public void PointingTechnique_WhenRuleSetIsNotStandard_Throws()
    {
        var puzzle = new Puzzle(new NonStandardRuleSet());

        Assert.Throws<ArgumentException>(() => new PointingTechnique().TryApply(puzzle));
    }

    [Fact]
    public void BoxLineReductionTechnique_WhenRuleSetIsNotStandard_Throws()
    {
        var puzzle = new Puzzle(new NonStandardRuleSet());

        Assert.Throws<ArgumentException>(() => new BoxLineReductionTechnique().TryApply(puzzle));
    }

    private static Puzzle Scenario(string wrapper) => wrapper switch
    {
        nameof(HiddenSingleTechnique) => HiddenSingleScenario(),
        nameof(NakedPairTechnique) => PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 1, 1, 2),
        nameof(HiddenPairTechnique) => HiddenSubsetScenario(2),
        nameof(NakedTripleTechnique) => PuzzleFactory.Empty()
            .WithCandidates(0, 0, 1, 2)
            .WithCandidates(0, 1, 1, 3)
            .WithCandidates(0, 2, 2, 3),
        nameof(HiddenTripleTechnique) => HiddenSubsetScenario(3),
        nameof(PointingTechnique) => ConfinedBoxScenario(),
        nameof(BoxLineReductionTechnique) => ConfinedRowScenario(),
        nameof(XWingTechnique) => FishScenario([1, 4], [2, 6]),
        nameof(SwordfishTechnique) => FishScenario([0, 4, 8], [1, 4, 7]),
        nameof(JellyfishTechnique) => FishScenario([0, 2, 4, 6], [0, 3, 5, 8]),
        _ => throw new ArgumentOutOfRangeException(nameof(wrapper), wrapper, "No scenario defined."),
    };

    private static Puzzle HiddenSingleScenario()
    {
        var puzzle = PuzzleFactory.Empty();

        for (var col = 1; col < 9; col++)
            puzzle.Board[0, col].RemoveCandidate(1);

        return puzzle;
    }

    private static Puzzle HiddenSubsetScenario(int n)
    {
        var puzzle = PuzzleFactory.Empty();
        byte[] hidden = [1, 2, 3];
        byte[] filler = [4, 5, 6, 7, 8, 9];

        for (var col = 0; col < n; col++)
            puzzle.WithCandidates(0, col, [.. hidden.Take(n), filler[col]]);

        for (var col = n; col < 9; col++)
            puzzle.WithCandidates(0, col, filler);

        return puzzle;
    }

    private static Puzzle ConfinedBoxScenario()
    {
        var puzzle = PuzzleFactory.Empty();

        foreach (var (row, col) in new[] { (0, 2), (1, 0), (1, 1), (1, 2), (2, 0), (2, 1), (2, 2) })
            puzzle.Board[row, col].RemoveCandidate(4);

        return puzzle;
    }

    private static Puzzle ConfinedRowScenario()
    {
        var puzzle = PuzzleFactory.Empty();

        for (var col = 2; col < 9; col++)
            puzzle.Board[0, col].RemoveCandidate(4);

        return puzzle;
    }

    private static Puzzle FishScenario(int[] rows, int[] columns)
    {
        var puzzle = PuzzleFactory.Empty();

        foreach (var row in rows)
        {
            for (var col = 0; col < 9; col++)
            {
                if (!columns.Contains(col))
                    puzzle.Board[row, col].RemoveCandidate(5);
            }
        }

        return puzzle;
    }
}
