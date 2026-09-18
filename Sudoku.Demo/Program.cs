using Sudoku.Core;
using Sudoku.Core.Generator;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Solver;

var puzzle = ValidBoardGenerator.GenerateMinimalBoard(new Puzzle(new StandardRuleSet()));

Console.WriteLine("Minimal puzzle:");
Console.WriteLine(puzzle.Board);
Console.WriteLine();

var solver = new HumanlikeSolver(puzzle);
solver.Solve();

Console.WriteLine("Solved puzzle:");
Console.WriteLine(puzzle.Board);
Console.WriteLine();
var valid = puzzle.RuleSet.FindFirstUnsatisfiedConstraint(puzzle.Board).IsNone;
Console.WriteLine($"Ruleset valid: {valid}");
Console.WriteLine("Changed cells by difficulty:");
foreach (var (difficulty, count) in solver.GetDifficultyUsageCount())
    Console.WriteLine($"{difficulty}: {count}");
Console.WriteLine("Changed cells by technique:");
foreach (var (technique, count) in solver.GetTechniqueUsageCount())
    Console.WriteLine($"{technique}: {count}");
