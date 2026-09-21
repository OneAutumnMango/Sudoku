namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

using Sudoku.Core.RuleSets;

public sealed class PointingTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Intermediate;

    public int TryApply(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        if (puzzle.RuleSet is not IStandardRuleSet standardRuleSet)
            throw new ArgumentException(
                "Pointing technique requires standard Sudoku rules.",
                nameof(puzzle));

        var rows = standardRuleSet.RowConstraints;
        var cols = standardRuleSet.ColumnConstraints;
        var boxes = standardRuleSet.BoxConstraints;

        return new GroupIntersectionTechnique(boxes, rows.Concat(cols))
            .TryApply(puzzle);
    }
}
