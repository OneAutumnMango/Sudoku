using Sudoku.Core;
using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver.SolvingTechniques.Standard;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class FishTechniqueTests
{
    [Fact]
    public void TryApply_WhenRowFishExists_RemovesCandidatesFromFishColumnsOutsideFishRows()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var ruleSet = (StandardRuleSet)puzzle.RuleSet;
        var fishCells = new HashSet<Cell>
        {
            puzzle.Board[0, 2], puzzle.Board[0, 5],
            puzzle.Board[1, 2], puzzle.Board[1, 5]
        };

        for (var row = 2; row < 9; row++)
            fishCells.Add(puzzle.Board[row, 2]);

        foreach (var (_, _, cell) in puzzle.Board.EnumerateAllCells())
        {
            if (!fishCells.Contains(cell))
                cell.RemoveCandidate(1);
        }

        Assert.Equal(2, ruleSet.RowConstraints[0].Cells.Count(cell => cell.HasCandidate(1)));
        Assert.Equal(2, ruleSet.RowConstraints[1].Cells.Count(cell => cell.HasCandidate(1)));
        Assert.Equal(
            2,
            ruleSet.RowConstraints
                .Take(2)
                .SelectMany(row => row.Cells.Where(cell => cell.HasCandidate(1)))
                .Select(ruleSet.GetContainingColumn)
                .Distinct()
                .Count());

        var applied = new FishTechnique(2).TryApply(puzzle);

        Assert.Equal(7, applied);
        Assert.DoesNotContain((byte)1, puzzle.Board[2, 2].GetCandidates());
        Assert.DoesNotContain((byte)1, puzzle.Board[2, 5].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenColumnFishExists_RemovesCandidatesFromFishRowsOutsideFishColumns()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var fishCells = new HashSet<Cell>
        {
            puzzle.Board[2, 0], puzzle.Board[5, 0],
            puzzle.Board[2, 1], puzzle.Board[5, 1]
        };

        for (var column = 2; column < 9; column++)
            fishCells.Add(puzzle.Board[2, column]);

        foreach (var (_, _, cell) in puzzle.Board.EnumerateAllCells())
        {
            if (!fishCells.Contains(cell))
                cell.RemoveCandidate(1);
        }

        var applied = new FishTechnique(2).TryApply(puzzle);

        Assert.Equal(7, applied);
        Assert.DoesNotContain((byte)1, puzzle.Board[2, 2].GetCandidates());
        Assert.DoesNotContain((byte)1, puzzle.Board[5, 2].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenNoFishExists_ReturnsZero()
    {
        var puzzle = new Puzzle(new StandardRuleSet());

        Assert.Equal(0, new FishTechnique(2).TryApply(puzzle));
    }

}
