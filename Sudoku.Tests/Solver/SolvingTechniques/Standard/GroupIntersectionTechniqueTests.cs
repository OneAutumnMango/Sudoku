using Sudoku.Core;
using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver.SolvingTechniques.Standard;

namespace Sudoku.Tests.Solver.SolvingTechniques.Standard;

public class GroupIntersectionTechniqueTests
{
    [Fact]
    public void TryApply_WhenReferenceCandidatesAreContainedInAffectedGroup_RemovesCandidatesOutsideReferenceGroup()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var ruleSet = (StandardRuleSet)puzzle.RuleSet;
        var referenceConstraint = ruleSet.GetContainingBox(puzzle.Board[0, 0]);
        var affectedConstraint = ruleSet.GetContainingRow(puzzle.Board[0, 0]);

        SetCandidates(puzzle.Board[0, 0], 1, 2);
        SetCandidates(puzzle.Board[0, 1], 1, 2);
        for (var row = 0; row < 3; row++)
        {
            for (var col = 0; col < 3; col++)
            {
                if (row == 0 && col < 2)
                    continue;

                SetCandidates(puzzle.Board[row, col], 2, 3, 4, 5, 6, 7, 8, 9);
            }
        }

        var applied = new GroupIntersectionTechnique(
            [referenceConstraint],
            [affectedConstraint])
            .TryApply(puzzle);

        Assert.Equal(6, applied);
        Assert.DoesNotContain((byte)1, puzzle.Board[0, 3].GetCandidates());
        Assert.Contains((byte)2, puzzle.Board[0, 3].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenReferenceCandidatesAreContainedInBox_RemovesCandidatesOutsideReferenceGroup()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var ruleSet = (StandardRuleSet)puzzle.RuleSet;
        var referenceConstraint = ruleSet.GetContainingRow(puzzle.Board[0, 0]);
        var affectedConstraint = ruleSet.GetContainingBox(puzzle.Board[0, 0]);

        SetCandidates(puzzle.Board[0, 0], 1, 2);
        SetCandidates(puzzle.Board[0, 1], 1, 2);
        for (var col = 2; col < 9; col++)
            SetCandidates(puzzle.Board[0, col], 2, 3, 4, 5, 6, 7, 8, 9);

        var applied = new GroupIntersectionTechnique(
            [referenceConstraint],
            [affectedConstraint])
            .TryApply(puzzle);

        Assert.Equal(6, applied);
        Assert.DoesNotContain((byte)1, puzzle.Board[1, 0].GetCandidates());
        Assert.DoesNotContain((byte)1, puzzle.Board[2, 1].GetCandidates());
    }

    [Fact]
    public void TryApply_WhenReferenceCandidatesAreNotContainedInAffectedGroup_ReturnsZero()
    {
        var puzzle = new Puzzle(new StandardRuleSet());
        var ruleSet = (StandardRuleSet)puzzle.RuleSet;
        var referenceConstraint = ruleSet.GetContainingBox(puzzle.Board[0, 0]);
        var affectedConstraint = ruleSet.GetContainingRow(puzzle.Board[0, 0]);

        var applied = new GroupIntersectionTechnique(
            [referenceConstraint],
            [affectedConstraint])
            .TryApply(puzzle);

        Assert.Equal(0, applied);
    }

    private static void SetCandidates(Cell cell, params byte[] candidates)
    {
        for (byte candidate = 1; candidate <= 9; candidate++)
        {
            if (!candidates.Contains(candidate))
                cell.RemoveCandidate(candidate);
        }
    }
}
