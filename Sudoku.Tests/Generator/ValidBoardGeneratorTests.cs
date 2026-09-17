using Sudoku.Core.Generator;
using Sudoku.Core.RuleSets;

namespace Sudoku.Tests.Generator;

public class ValidBoardGeneratorTests
{
    [Fact]
    public void Generate_ReturnsValidBoard_ForStandardRules()
    {
        var ruleSet = new StandardRuleSet();
        var board = ValidBoardGenerator.Generate(ruleSet);

        Assert.True(ruleSet.FindFirstUnsatisfiedConstraint(board).IsNone);
    }
}
