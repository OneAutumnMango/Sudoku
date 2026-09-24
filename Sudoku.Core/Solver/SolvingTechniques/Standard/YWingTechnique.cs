using System.Numerics;
using Sudoku.Core.Constraints;
using Sudoku.Core.Grid;

namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

public class YWingTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Advanced;

    public int TryApply(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        int applied = 0;

        var pivotCandidates = puzzle.Board.EnumerateEmptyCells()
            .Select(_ => _.cell)
            .Where(cell => BitOperations.PopCount(cell.Candidates) == 2);
 
        foreach (var pivot in pivotCandidates) 
        {
            var xy = pivot.GetCandidates().ToArray();
            var x = xy[0];
            var y = xy[1];

            var zToWingCandMap =
                Enumerable.Range(1, puzzle.Board.Size)
                    .Where(z => z != x && z != y)
                    .ToDictionary(
                        z => (byte)z,
                        _ => new Dictionary<int, List<(IConstraint, Cell)>>()
                        {
                            [x] = [],
                            [y] = []
                        });
            
            foreach (var constraint in puzzle.RuleSet.GetContainingConstraints(pivot))
            {
                foreach (var cell in constraint.Cells)
                {
                    if (cell != pivot && 
                        cell.Value == 0 &&
                        BitOperations.PopCount(cell.Candidates) == 2)
                    {
                        var wingCandidates = cell.GetCandidates();
                        bool hasX = wingCandidates.Contains(x);
                        bool hasY = wingCandidates.Contains(y);

                        if (!(hasX ^ hasY))  // Must have exactly one candidate in common with the pivot cell
                            continue;

                        var sharedWithPivot = hasX ? x : y;
                        var z = wingCandidates.First(c => c != sharedWithPivot);

                        zToWingCandMap[z][sharedWithPivot].Add((constraint, cell));
                    }
                }
            }

            // for all combinations of two YZ XZ pairs within differing constraints to each other
            // eliminate Z candidates from the intersection of the constraints holding YZ XZ

            foreach (var (z, wings) in zToWingCandMap)
            {
                var xwings = wings[x];
                var ywings = wings[y];

                if (xwings.Count == 0 || ywings.Count == 0)
                    continue;

                foreach (var (_, xCell) in xwings)
                {
                    foreach (var (_, yCell) in ywings)
                    {
                        if (xCell == yCell)
                            continue;  // cell may be added twice under two different constraints (ie if in the row and box of pivot)

                        // get intersections of all constraints with eachother to form a set of cells where Z can be eliminated

                        var xCellSet = puzzle.RuleSet.GetContainingConstraints(xCell)
                            .SelectMany(c => c.Cells)
                            .ToHashSet();
                        var yCellSet = puzzle.RuleSet.GetContainingConstraints(yCell)
                            .SelectMany(c => c.Cells)
                            .ToHashSet();
                        var intersection = xCellSet.Intersect(yCellSet);

                        foreach (var cell in intersection)
                        {
                            if (cell == pivot || cell == xCell || cell == yCell)
                                continue;

                            if (cell.Value != 0 || !cell.HasCandidate(z))
                                continue;

                            cell.RemoveCandidate(z);
                            applied++;
                        }
                    }
                }
            }
        }

        return applied;
    }
}
