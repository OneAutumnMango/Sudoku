using Sudoku.Core.Grid;
using Sudoku.Core.Constraints;
using Sudoku.Core.Utils;

namespace Sudoku.Core.RuleSets;

public sealed class StandardRuleSet : IStandardRuleSet
{
    private sealed class ConstraintCache
    {
        public required IReadOnlyList<IConstraint> Constraints { get; init; }
        public required IReadOnlyList<IConstraint> RowConstraints { get; init; }
        public required IReadOnlyList<IConstraint> ColumnConstraints { get; init; }
        public required IReadOnlyList<IConstraint> BoxConstraints { get; init; }
        public required IReadOnlyDictionary<Cell, IReadOnlyList<IConstraint>> ConstraintsByCell { get; init; }
    }

    private readonly ConstraintCache _cache;

    public StandardRuleSet() : this(new Board(9))
    {
    }

    public StandardRuleSet(Board board)
    {
        ArgumentNullException.ThrowIfNull(board);
        Board = board;

        var rowConstraints = board.Rows
            .Select(group => (IConstraint)new UniqueGroupConstraint(group))
            .ToList();
        var columnConstraints = board.Columns
            .Select(group => (IConstraint)new UniqueGroupConstraint(group))
            .ToList();
        var boxConstraints = board.Blocks
            .Select(group => (IConstraint)new UniqueGroupConstraint(group))
            .ToList();
        var constraints = rowConstraints
            .Concat(columnConstraints)
            .Concat(boxConstraints)
            .ToList();

        var constraintsByCell = board.EnumerateAllCells()
            .ToDictionary(
                item => item.cell,
                item => (IReadOnlyList<IConstraint>)constraints
                    .Where(constraint => constraint.Cells.Contains(item.cell))
                    .ToList());

        _cache = new ConstraintCache
        {
            Constraints = constraints,
            RowConstraints = rowConstraints,
            ColumnConstraints = columnConstraints,
            BoxConstraints = boxConstraints,
            ConstraintsByCell = constraintsByCell
        };
    }

    public Board Board { get; }
    public IReadOnlyList<IConstraint> RowConstraints => _cache.RowConstraints;
    public IReadOnlyList<IConstraint> ColumnConstraints => _cache.ColumnConstraints;
    public IReadOnlyList<IConstraint> BoxConstraints => _cache.BoxConstraints;

    public IEnumerable<IConstraint> GetConstraints()
    {
        return _cache.Constraints;
    }

    public IConstraint GetContainingRow(Cell cell)
    {
        return GetContainingConstraint(_cache.RowConstraints, cell, nameof(cell));
    }

    public IConstraint GetContainingColumn(Cell cell)
    {
        return GetContainingConstraint(_cache.ColumnConstraints, cell, nameof(cell));
    }

    public IConstraint GetContainingBox(Cell cell)
    {
        return GetContainingConstraint(_cache.BoxConstraints, cell, nameof(cell));
    }

    private static IConstraint GetContainingConstraint(
        IReadOnlyList<IConstraint> constraints,
        Cell cell,
        string parameterName)
    {
        ArgumentNullException.ThrowIfNull(cell, parameterName);

        var constraint = constraints.FirstOrDefault(constraint => constraint.Cells.Contains(cell));
        return constraint ?? throw new ArgumentException("Cell does not belong to this board.", parameterName);
    }

    public Option<IConstraint> FindFirstUnsatisfiedConstraint()
    {
        foreach (var constraint in GetConstraints())
        {
            if (!constraint.IsSatisfied())
                return new Option<IConstraint>(constraint);
        }

        return Option<IConstraint>.None;
    }

    public void ComputeAndFillCandidates()
    {
        foreach (var (_, _, cell) in Board.EnumerateAllCells())
        {
            if (cell.Value == 0)
                cell.ResetRuleCandidates();
        }

        foreach (var constraint in _cache.Constraints)
            constraint.ComputeAndFillCandidates();
    }

    public void UpdateCandidates(Cell changedCell)
    {
        ArgumentNullException.ThrowIfNull(changedCell);

        var affectedConstraints = _cache.ConstraintsByCell[changedCell];
        var affectedCells = affectedConstraints
            .SelectMany(constraint => constraint.Cells)
            .Where(cell => cell.Value == 0)
            .Distinct()
            .ToList();

        foreach (var cell in affectedCells)
        {
            cell.ResetRuleCandidates();

            foreach (var constraint in _cache.ConstraintsByCell[cell])
                cell.IntersectRuleCandidates(constraint.GetAllowedCandidates(cell));
        }
    }

    public IEnumerable<IConstraint> GetContainingConstraints(Cell cell)
    {
        return _cache.ConstraintsByCell[cell];
    }
}
