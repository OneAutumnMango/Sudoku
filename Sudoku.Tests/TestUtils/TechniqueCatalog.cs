using Sudoku.Core;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver;
using Sudoku.Core.Solver.SolvingTechniques;
using Sudoku.Core.Solver.SolvingTechniques.Standard;

namespace Sudoku.Tests.TestUtils;

/// <summary>
/// Technique instances are addressed by name so xunit can serialise theory data.
/// GroupIntersectionTechnique is board-bound, hence the Puzzle argument on <see cref="Create"/>.
/// </summary>
public static class TechniqueCatalog
{
    public const string Pointing = "GroupIntersection(Pointing)";
    public const string BoxLine = "GroupIntersection(BoxLine)";

    public static readonly IReadOnlyList<string> Wrappers =
    [
        nameof(NakedSingleTechnique),
        nameof(HiddenSingleTechnique),
        nameof(NakedPairTechnique),
        nameof(HiddenPairTechnique),
        nameof(NakedTripleTechnique),
        nameof(HiddenTripleTechnique),
        nameof(PointingTechnique),
        nameof(BoxLineReductionTechnique),
        nameof(YWingTechnique),
        nameof(XWingTechnique),
        nameof(SwordfishTechnique),
        nameof(JellyfishTechnique),
    ];

    public static readonly IReadOnlyList<string> Engines =
    [
        "NakedN(2)", "NakedN(3)", "NakedN(4)",
        "HiddenN(1)", "HiddenN(2)", "HiddenN(3)", "HiddenN(4)",
        "Fish(2)", "Fish(3)", "Fish(4)",
        Pointing, BoxLine,
    ];

    public static IReadOnlyList<string> Names => [.. Wrappers, .. Engines];

    public static IEnumerable<object[]> AllNames => Names.Select(name => new object[] { name });

    public static IEnumerable<object[]> WrapperNames => Wrappers.Select(name => new object[] { name });

    /// <summary>Wrapper name paired with the engine configuration it must delegate to.</summary>
    public static IEnumerable<object[]> Delegations =>
    [
        [nameof(HiddenSingleTechnique), "HiddenN(1)"],
        [nameof(NakedPairTechnique), "NakedN(2)"],
        [nameof(HiddenPairTechnique), "HiddenN(2)"],
        [nameof(NakedTripleTechnique), "NakedN(3)"],
        [nameof(HiddenTripleTechnique), "HiddenN(3)"],
        [nameof(PointingTechnique), Pointing],
        [nameof(BoxLineReductionTechnique), BoxLine],
        [nameof(XWingTechnique), "Fish(2)"],
        [nameof(SwordfishTechnique), "Fish(3)"],
        [nameof(JellyfishTechnique), "Fish(4)"],
    ];

    public static ISolvingTechnique Create(string name, Puzzle puzzle)
    {
        if (name == Pointing || name == BoxLine)
        {
            var ruleSet = (IStandardRuleSet)puzzle.RuleSet;
            var lines = ruleSet.RowConstraints.Concat(ruleSet.ColumnConstraints);

            return name == Pointing
                ? new GroupIntersectionTechnique(ruleSet.BoxConstraints, lines)
                : new GroupIntersectionTechnique(lines, ruleSet.BoxConstraints);
        }

        return name switch
        {
            nameof(NakedSingleTechnique) => new NakedSingleTechnique(),
            nameof(HiddenSingleTechnique) => new HiddenSingleTechnique(),
            nameof(NakedPairTechnique) => new NakedPairTechnique(),
            nameof(HiddenPairTechnique) => new HiddenPairTechnique(),
            nameof(NakedTripleTechnique) => new NakedTripleTechnique(),
            nameof(HiddenTripleTechnique) => new HiddenTripleTechnique(),
            nameof(PointingTechnique) => new PointingTechnique(),
            nameof(BoxLineReductionTechnique) => new BoxLineReductionTechnique(),
            nameof(YWingTechnique) => new YWingTechnique(),
            nameof(XWingTechnique) => new XWingTechnique(),
            nameof(SwordfishTechnique) => new SwordfishTechnique(),
            nameof(JellyfishTechnique) => new JellyfishTechnique(),
            "NakedN(2)" => new NakedNTechnique(2),
            "NakedN(3)" => new NakedNTechnique(3),
            "NakedN(4)" => new NakedNTechnique(4),
            "HiddenN(1)" => new HiddenNTechnique(1),
            "HiddenN(2)" => new HiddenNTechnique(2),
            "HiddenN(3)" => new HiddenNTechnique(3),
            "HiddenN(4)" => new HiddenNTechnique(4),
            "Fish(2)" => new FishTechnique(2),
            "Fish(3)" => new FishTechnique(3),
            "Fish(4)" => new FishTechnique(4),
            _ => throw new ArgumentOutOfRangeException(nameof(name), name, "Unknown technique."),
        };
    }

    /// <summary>False for techniques whose return value is not a count of eliminated candidates.</summary>
    public static bool CountsEliminations(string name) =>
        name != nameof(NakedSingleTechnique) && !IsHidden(name);

    /// <summary>HiddenN returns the number of subsets found, not the number of candidates removed.</summary>
    public static bool IsHidden(string name) =>
        name.StartsWith("HiddenN(", StringComparison.Ordinal)
        || name is nameof(HiddenSingleTechnique)
            or nameof(HiddenPairTechnique)
            or nameof(HiddenTripleTechnique);

    public static Difficulty ExpectedDifficulty(string name) => name switch
    {
        nameof(NakedSingleTechnique) or nameof(HiddenSingleTechnique) => Difficulty.Simple,
        nameof(NakedPairTechnique) or nameof(HiddenPairTechnique) => Difficulty.Easy,
        nameof(NakedTripleTechnique) or nameof(HiddenTripleTechnique) or nameof(PointingTechnique) =>
            Difficulty.Intermediate,
        nameof(BoxLineReductionTechnique) or nameof(YWingTechnique) or nameof(XWingTechnique) =>
            Difficulty.Advanced,
        nameof(SwordfishTechnique) => Difficulty.Expert,
        nameof(JellyfishTechnique) => Difficulty.Master,
        _ => Difficulty.Unknown,
    };
}
