using Sudoku.Core;
using Sudoku.Core.Constraints;
using Sudoku.Core.Grid;
using Sudoku.Core.Utils;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public class GroupIntersectionTechnique(IEnumerable<IConstraint> referenceConstraints, IEnumerable<IConstraint> affectedConstraints) : ISolvingTechnique
{
    public Difficulty Difficulty { get; } = Difficulty.Unknown;
    private readonly IReadOnlyList<IConstraint> _referenceConstraints = referenceConstraints.ToList();
    private readonly IReadOnlyList<IConstraint> _affectedConstraints = affectedConstraints.ToList();

    public int TryApply(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        var applied = 0;
        var maxIntersectionCells = (int)Math.Sqrt(puzzle.Board.Size);

        foreach (var referenceConstraint in _referenceConstraints)
        {
            // listed cells by candidate
            var candidateCells = Enumerable
                .Range(0, puzzle.Board.Size + 1)
                .Select(_ => new List<Cell>())
                .ToList();

            foreach (var cell in referenceConstraint.Cells)
            {
                if (cell.Value != 0)
                    continue;

                foreach (var cand in cell.GetCandidates())
                    candidateCells[cand].Add(cell);
            }

            for (var cand = 1; cand < puzzle.Board.Size + 1; cand++)
            {
                var cells = candidateCells[cand];

                if (cells.Count < 2 || cells.Count > maxIntersectionCells)
                    continue;

                // constrait containing all cells or Option.None
                var containingConstraint = Option<IConstraint>.None;
                foreach (var affectedConstraint in _affectedConstraints)
                {
                    if (cells.All(cell => affectedConstraint.Cells.Contains(cell)))
                    {
                        containingConstraint = new Option<IConstraint>(affectedConstraint);
                        break;
                    }
                }

                if (containingConstraint.IsNone)
                    continue;

                foreach (var cell in containingConstraint.Value.Cells)
                {
                    if (cell.Value != 0 || cells.Contains(cell))
                        continue;

                    if (!cell.HasCandidate((byte)cand))
                        continue;

                    puzzle.RemoveCandidate(cell, (byte)cand);
                    applied += 1;
                }
            }
        }
        return applied;
    }
}