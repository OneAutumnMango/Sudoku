using Sudoku.Core;
using Sudoku.Core.Constraints;
using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Utils;

namespace Sudoku.Tests.TestUtils;

/// <summary>An IRuleSet that is deliberately not an IStandardRuleSet.</summary>
public sealed class NonStandardRuleSet : IRuleSet
{
    private readonly List<IConstraint> _constraints;

    public NonStandardRuleSet()
    {
        Board = new Board(9);
        _constraints = Board.Rows
            .Select(row => (IConstraint)new UniqueGroupConstraint(row))
            .ToList();
    }

    public Board Board { get; }

    public IEnumerable<IConstraint> GetConstraints() => _constraints;

    public Option<IConstraint> FindFirstUnsatisfiedConstraint()
    {
        foreach (var constraint in _constraints)
        {
            if (!constraint.IsSatisfied())
                return new Option<IConstraint>(constraint);
        }

        return Option<IConstraint>.None;
    }

    public void ComputeAndFillCandidates()
    {
        foreach (var (_, _, cell) in Board.EnumerateEmptyCells())
            cell.ResetRuleCandidates();

        foreach (var constraint in _constraints)
            constraint.ComputeAndFillCandidates();
    }

    public void UpdateCandidates(Cell changedCell) => ComputeAndFillCandidates();

    public IEnumerable<IConstraint> GetContainingConstraints(Cell cell) =>
        _constraints.Where(constraint => constraint.Cells.Contains(cell));
}
