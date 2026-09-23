using Sudoku.Core;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver.SolvingTechniques.Standard;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class YWingTechniqueTests
{
    [Fact]
    public void TryApply_WhenNoYWingPatternExists_ReturnsZeroChanges()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var technique = new YWingTechnique();

        var applied = technique.TryApply(puzzle);

        Assert.Equal(0, applied);
    }

    [Fact]
    public void TryApply_WhenBoardIsFull_ReturnsZeroChanges()
    {
        var puzzle = CreatePuzzle(
            new int[,]
            {
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
                { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 1 },
                { 5, 6, 7, 8, 9, 1, 2, 3, 4 },
                { 8, 9, 1, 2, 3, 4, 5, 6, 7 },
                { 3, 4, 5, 6, 7, 8, 9, 1, 2 },
                { 6, 7, 8, 9, 1, 2, 3, 4, 5 },
                { 9, 1, 2, 3, 4, 5, 6, 7, 8 }
            });

        var technique = new YWingTechnique();
        var applied = technique.TryApply(puzzle);

        Assert.Equal(0, applied);
    }

    [Fact]
    public void TryApply_DoesNotApplyWhenPivotAndWingsShareConstraint()
    {
        // Create a scenario where pivot, XZ wing, and YZ wing are all in the same row
        var puzzle = new Puzzle(new StandardRuleSet());
        
        // Fill most of the board to constrain candidates
        puzzle.SetCell(0, 0, 1);
        puzzle.SetCell(0, 1, 2);
        puzzle.SetCell(0, 2, 3);
        puzzle.SetCell(0, 3, 4);
        puzzle.SetCell(0, 4, 5);
        puzzle.SetCell(0, 5, 6);
        // Leave positions 6, 7, 8 empty with candidates 7,8,9
        
        var technique = new YWingTechnique();
        var applied = technique.TryApply(puzzle);
        
        // Should not apply Y-Wing when all cells are in same constraint
        Assert.Equal(0, applied);
    }

    [Fact]
    public void TryApply_DoesntEliminateFilledCellsCandidates()
    {
        var puzzle = CreatePuzzle(
            new int[,]
            {
                { 1, 2, 3, 4, 5, 6, 7, 8, 9 },
                { 4, 5, 6, 7, 8, 9, 1, 2, 3 },
                { 7, 8, 9, 1, 2, 3, 4, 5, 6 },
                { 2, 3, 4, 5, 6, 7, 8, 9, 1 },
                { 5, 6, 7, 8, 9, 1, 2, 3, 4 },
                { 8, 9, 1, 2, 3, 4, 5, 6, 7 },
                { 3, 4, 5, 6, 7, 8, 9, 1, 2 },
                { 6, 7, 8, 9, 1, 2, 3, 4, 5 },
                { 9, 1, 2, 3, 4, 5, 6, 7, 8 }
            });

        puzzle.ClearCell(1, 2);
        puzzle.ClearCell(2, 1);
        puzzle.ClearCell(3, 0);

        var technique = new YWingTechnique();
        var applied = technique.TryApply(puzzle);

        // No Y-Wing patterns to apply in this solved puzzle
        Assert.Equal(0, applied);
    }

    [Fact]
    public void TryApply_SparseGridWithYWingPattern_EliminatesCorrectively()
    {
        var puzzle = CreatePuzzle(
            new int[,]
            {
                { 5, 9, 7, 0, 0, 0, 0, 1, 0 },
                { 4, 1, 6, 0, 0, 8, 0, 5, 3 },
                { 8, 2, 3, 1, 5, 0, 4, 0, 9 },
                { 0, 0, 4, 0, 0, 0, 0, 9, 1 },
                { 0, 0, 9, 0, 0, 0, 0, 0, 2 },
                { 0, 3, 2, 8, 0, 9, 5, 4, 7 },
                { 3, 7, 8, 5, 0, 1, 0, 2, 4 },
                { 2, 6, 1, 0, 8, 4, 0, 3, 5 },
                { 9, 4, 5, 0, 2, 0, 1, 0, 0 }
            });

        // topleft is 0,0
        // cell 1,4 is pivot with candidates {7,9}
        // wings are 2,5 with candidates {6,7} and 6,4 with candidates {6,9}
        // cand 6 should be eliminated from 0,4 and 8,5

        // Count total candidates before
        int candidatesBefore = 0;
        foreach (var (_, _, cell) in puzzle.Board.EnumerateEmptyCells())
            candidatesBefore += System.Numerics.BitOperations.PopCount(cell.Candidates);

        var technique = new YWingTechnique();
        var applied = technique.TryApply(puzzle);

        // Count total candidates after
        int candidatesAfter = 0;
        foreach (var (_, _, cell) in puzzle.Board.EnumerateEmptyCells())
            candidatesAfter += System.Numerics.BitOperations.PopCount(cell.Candidates);

        // Y-Wing should eliminate exactly 2 candidates
        Assert.Equal(2, applied);
        Assert.Equal(candidatesBefore - candidatesAfter, applied);

        // Verify that candidate 6 is eliminated from the intersection cells
        Assert.False(puzzle.Board[0, 4].HasCandidate(6), "Candidate 6 should be eliminated from (0,4)");
        Assert.False(puzzle.Board[8, 5].HasCandidate(6), "Candidate 6 should be eliminated from (8,5)");
    }

    private static Puzzle CreatePuzzle(int[,] values)
    {
        return new Puzzle(new StandardRuleSet(), values);
    }
}
