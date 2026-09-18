using System.Numerics;
using Sudoku.Core.Constraints;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public sealed class HiddenSingleTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Easy;

    public int TryApply(Puzzle puzzle)
    {
        var board = puzzle.Board;
        var ruleset = puzzle.RuleSet;

        ruleset.ComputeAndFillCandidates(board);

        var changed = 0;

        foreach (var constraint in ruleset.GetConstraints(board))
        {
            var constraintChanges = TryFindAndApplyHiddenSingle(constraint);
            if (constraintChanges == 0)
                continue;

            changed += constraintChanges;
            ruleset.ComputeAndFillCandidates(board);
        }

        return changed;
    }

    private int TryFindAndApplyHiddenSingle(IConstraint constraint)
    {
        ushort seen = 0b0;
        ushort multiple = 0b0;

        var cells = constraint.Cells.Where(cell => cell.Value == 0).ToList();

        foreach (var cell in cells)
        {
            multiple |= (ushort)(seen & cell.Candidates);
            seen |= cell.Candidates;
        }

        ushort candidates = (ushort)(seen & ~multiple);

        if (candidates == 0)
            return 0;

        var changed = 0;

        foreach (var cell in cells)
        {
            ushort hiddenSingle = (ushort)(cell.Candidates & candidates);

            if (hiddenSingle == 0)
                continue;

            // safety only has one "1" bit
            if (!BitOperations.IsPow2((uint)hiddenSingle))
                continue;

            int bit = BitOperations.TrailingZeroCount((uint)hiddenSingle);
            cell.Value = (byte)(bit + 1);
            changed++;
        }

        return changed;
    }
}

