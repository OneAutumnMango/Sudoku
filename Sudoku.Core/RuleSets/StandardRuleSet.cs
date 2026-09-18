using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;
using Sudoku.Core.Utils;

namespace Sudoku.Core.RuleSets;

public sealed class StandardRuleSet : IRuleSet
{
    private sealed class ConstraintCache
    {
        public required IReadOnlyList<IConstraint> Constraints { get; init; }
        public required IReadOnlyDictionary<Cell, IReadOnlyList<IConstraint>> ConstraintsByCell { get; init; }
    }

    private readonly Dictionary<Board, ConstraintCache> _cacheByBoard = [];

    public IEnumerable<IConstraint> GetConstraints(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);

        return GetCache(board).Constraints;
    }

    private ConstraintCache GetCache(Board board)
    {
        if (!_cacheByBoard.TryGetValue(board, out var cache))
        {
            var constraints = board.AllGroups
                .Select(group => (IConstraint)new UniqueGroupConstraint(group))
                .ToList();

            var constraintsByCell = board.EnumerateAllCells()
                .ToDictionary(
                    item => item.cell,
                    item => (IReadOnlyList<IConstraint>)constraints
                        .Where(constraint => constraint.Cells.Contains(item.cell))
                        .ToList());

            cache = new ConstraintCache
            {
                Constraints = constraints,
                ConstraintsByCell = constraintsByCell
            };
            _cacheByBoard[board] = cache;
        }

        return cache;
    }

    public Option<IConstraint> FindFirstUnsatisfiedConstraint(Board board)
    {
        foreach (var constraint in GetConstraints(board))
        {
            if (!constraint.IsSatisfied())
                return new Option<IConstraint>(constraint);
        }

        return Option<IConstraint>.None;
    }

    public void ComputeAndFillCandidates(Board board)
    {
        var cache = GetCache(board);

        foreach (var (_, _, cell) in board.EnumerateAllCells())
        {
            if (cell.Value == 0)
                cell.Candidates = Cell.AllCandidates;
        }

        foreach (var constraint in cache.Constraints)
            constraint.ComputeAndFillCandidates();
    }

    public void UpdateCandidates(Board board, Cell changedCell)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(changedCell);

        var cache = GetCache(board);
        var affectedConstraints = cache.ConstraintsByCell[changedCell];
        var affectedCells = affectedConstraints
            .SelectMany(constraint => constraint.Cells)
            .Where(cell => cell.Value == 0)
            .Distinct()
            .ToList();

        foreach (var cell in affectedCells)
        {
            cell.Candidates = Cell.AllCandidates;

            foreach (var constraint in cache.ConstraintsByCell[cell])
                cell.IntersectCandidates(constraint.GetAllowedCandidates(cell));
        }
    }
}
