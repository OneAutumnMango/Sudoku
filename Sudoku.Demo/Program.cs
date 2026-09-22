using System.Text.Json;
using System.Text.Json.Serialization;
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

// var valid = puzzle.RuleSet.FindFirstUnsatisfiedConstraint().IsNone;
// Console.WriteLine($"Ruleset valid: {valid}");

// Console.WriteLine("\nTechniques applied by difficulty:");
// foreach (var (difficulty, count) in solver.GetDifficultyUsageCount())
//     Console.WriteLine($"{difficulty}: {count}");

// Console.WriteLine("\nTechniques applied:");
// foreach (var (technique, count) in solver.GetTechniqueUsageCount())
//     Console.WriteLine($"{technique}: {count}");

void generateNSolvable(int n, string filePath)
{
    var puzzles = new List<GeneratedPuzzle>();

    var options = new JsonSerializerOptions { WriteIndented = true };
    options.Converters.Add(new JsonStringEnumConverter());

    var numFailed = 0;

    // write all puzzles into json file at filepath one by one
    for (int i = 0; i < n; i++)
    {
        var puzzle = ValidBoardGenerator.GenerateMinimalBoard(new Puzzle(new StandardRuleSet()));
        var puzzleString = puzzle.Board.ToString();

        var solver = new HumanlikeSolver(puzzle);
        var maybeDifficulty = solver.Solve();
        if (maybeDifficulty.IsNone)
        {
            Console.WriteLine("Failed to solve puzzle, skipping...");
            numFailed++;
            i--;
            continue;
        }

        var difficulty = maybeDifficulty.Value;
        var solvedPuzzleString = puzzle.Board.ToCompactString();

        var generatedPuzzle = new GeneratedPuzzle(
            Puzzle: puzzleString,
            Solution: solvedPuzzleString,
            Difficulty: difficulty
        );

        puzzles.Add(generatedPuzzle);

        var json = JsonSerializer.Serialize(puzzles, options);
        File.WriteAllText(filePath, json);

        Console.WriteLine($"Generated {puzzles.Count}/{n} - {difficulty}");
    }

    Console.WriteLine($"Finished generating {puzzles.Count} puzzles.");
    var counts = puzzles
    .GroupBy(p => p.Difficulty)
    .ToDictionary(g => g.Key, g => g.Count());

    foreach (var difficulty in Enum.GetValues<Difficulty>())
    {
        counts.TryGetValue(difficulty, out var count);
        Console.WriteLine($"Number of {difficulty} puzzles: {count}");
    }
    Console.WriteLine($"Number of failed puzzles: {numFailed}");
}



GeneratedPuzzle? FindAndPrintPuzzleUsingTechnique(
    string techniqueName,
    int maxAttempts = 10_000)
{
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        Console.Write($"\rChecked {attempt}/{maxAttempts} puzzles...");

        var puzzle = ValidBoardGenerator.GenerateMinimalBoard(
            new Puzzle(new StandardRuleSet()));
        var puzzleString = puzzle.Board.ToString();
        var solver = new HumanlikeSolver(puzzle);
        var difficulty = solver.Solve();

        if (difficulty.IsNone)
            continue;

        var usage = solver.GetTechniqueUsageCount();
        if (!usage.TryGetValue(techniqueName, out var count) || count == 0)
            continue;

        Console.WriteLine();
        var result = new GeneratedPuzzle(
            puzzleString,
            puzzle.Board.ToString(),
            difficulty.Value);

        Console.WriteLine($"Found after {attempt} attempts using {techniqueName}:");
        Console.WriteLine($"Puzzle: \n{result.Puzzle}");
        Console.WriteLine($"Solution: \n{result.Solution}");
        Console.WriteLine($"Difficulty: {result.Difficulty}");
        Console.WriteLine("Technique usage:");
        foreach (var (technique, usageCount) in usage)
            Console.WriteLine($"{technique}: {usageCount}");

        return result;
    }

    Console.WriteLine();
    Console.WriteLine($"No puzzle found using {techniqueName} after {maxAttempts} attempts.");
    return null;
}

GeneratedPuzzle? FindAndPrintPuzzleWithDifficulty(
    Difficulty targetDifficulty,
    int maxAttempts = 10_000)
{
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        Console.Write($"\rChecked {attempt}/{maxAttempts} puzzles...");

        var puzzle = ValidBoardGenerator.GenerateMinimalBoard(
            new Puzzle(new StandardRuleSet()));
        var puzzleString = puzzle.Board.ToString();
        var solver = new HumanlikeSolver(puzzle);
        var difficulty = solver.Solve();

        if (difficulty.IsNone || difficulty.Value != targetDifficulty)
            continue;

        Console.WriteLine();
        var result = new GeneratedPuzzle(
            puzzleString,
            puzzle.Board.ToString(),
            difficulty.Value);

        Console.WriteLine($"Found after {attempt} attempts with difficulty {targetDifficulty}:");
        Console.WriteLine($"Puzzle:  {result.Puzzle}");
        Console.WriteLine($"Solution: {result.Solution}");
        Console.WriteLine($"Difficulty: {result.Difficulty}");
        Console.WriteLine("Technique usage:");
        foreach (var (technique, usageCount) in solver.GetTechniqueUsageCount())
            Console.WriteLine($"{technique}: {usageCount}");

        return result;
    }

    Console.WriteLine();
    Console.WriteLine($"No puzzle found with difficulty {targetDifficulty} after {maxAttempts} attempts.");
    return null;
}

// FindAndPrintPuzzleUsingTechnique("XWingTechnique");
// FindAndPrintPuzzleUsingTechnique("SwordfishTechnique");
// FindAndPrintPuzzleWithDifficulty(Difficulty.Advanced);

// generateNSolvable(10000, "generated_puzzles.json");

Console.WriteLine(". 8 . 2 . . . . 9\r\n. . 1 . . 5 . . .\r\n. . 6 7 . . . 3 .\r\n. 2 . . . . 1 . .\r\n. . . . 2 9 . . .\r\n. . 7 . . 6 5 . .\r\n. . 2 . 6 . . . .\r\n. 9 . . 5 4 . . 7\r\n6 . . . 7 . . 8 .");



public record GeneratedPuzzle(
    string Puzzle,
    string Solution,
    Difficulty Difficulty);

