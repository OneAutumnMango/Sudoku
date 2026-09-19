using Sudoku.Core;
using Sudoku.Core.Generator;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver;

// var puzzle = ValidBoardGenerator.GenerateMinimalBoard(new Puzzle(new StandardRuleSet()));

// Console.WriteLine("Minimal puzzle:");
// Console.WriteLine(puzzle.Board);
// Console.WriteLine();

// var solver = new HumanlikeSolver(puzzle);
// solver.Solve();

// Console.WriteLine("Solved puzzle:");
// Console.WriteLine(puzzle.Board);
// Console.WriteLine();

// var valid = puzzle.RuleSet.FindFirstUnsatisfiedConstraint(puzzle.Board).IsNone;
// Console.WriteLine($"Ruleset valid: {valid}");

// Console.WriteLine("\nTechniques applied by difficulty:");
// foreach (var (difficulty, count) in solver.GetDifficultyUsageCount())
//     Console.WriteLine($"{difficulty}: {count}");

// Console.WriteLine("\nTechniques applied:");
// foreach (var (technique, count) in solver.GetTechniqueUsageCount())
//     Console.WriteLine($"{technique}: {count}");



void findEpicAlert()
{
    var i = 0;
    while (true)
    {
        i++;
        var p = ValidBoardGenerator.GenerateMinimalBoard(new Puzzle(new StandardRuleSet()));

        Console.WriteLine("Minimal puzzle:");
        Console.WriteLine(p.Board);
        Console.WriteLine();

        var s = new HumanlikeSolver(p);
        s.Solve();
        if (!p.RuleSet.FindFirstUnsatisfiedConstraint(p.Board).IsNone)
            Console.WriteLine("Generated puzzle is invalid!!!!!!!!!!!!!!");

        Console.WriteLine(p.Board);
        Console.WriteLine();
        Console.WriteLine($"Solved puzzle {i}:");

        int epicsFound = 0;
        foreach (var (technique, count) in s.GetTechniqueUsageCount())
        {
            Console.WriteLine($"{technique}: {count}");
            if (count > 0)
            {
                if (technique == "HiddenPairTechnique")
                    epicsFound++;
                if (technique == "HiddenTripleTechnique")
                    epicsFound++;
            }
        }
        if (epicsFound >= 2 && s.IsSolved())
            return;
    }
}

findEpicAlert();
