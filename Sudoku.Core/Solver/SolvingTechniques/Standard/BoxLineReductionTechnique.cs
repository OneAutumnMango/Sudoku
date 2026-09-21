namespace Sudoku.Core.Solver.SolvingTechniques.Standard;

using Sudoku.Core.RuleSets;

public sealed class BoxLineReductionTechnique : ISolvingTechnique
{
    public Difficulty Difficulty => Difficulty.Advanced;

    public int TryApply(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        if (puzzle.RuleSet is not IStandardRuleSet standardRuleSet)
            throw new ArgumentException(
                "Box-line reduction requires standard Sudoku rules.",
                nameof(puzzle));

        var rows = standardRuleSet.RowConstraints;
        var cols = standardRuleSet.ColumnConstraints;
        var boxes = standardRuleSet.BoxConstraints;

        return new GroupIntersectionTechnique(rows.Concat(cols), boxes)
            .TryApply(puzzle);
    }
}

