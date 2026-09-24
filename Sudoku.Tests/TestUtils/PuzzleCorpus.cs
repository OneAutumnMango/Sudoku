using System.Text.Json;
using System.Text.Json.Serialization;
using Sudoku.Core.Solver;

namespace Sudoku.Tests.TestUtils;

public sealed record GeneratedPuzzle(string Puzzle, string Solution, Difficulty Difficulty);

public static class PuzzleCorpus
{
    private const string FileName = "generated_puzzles.json";

    private static readonly Lazy<IReadOnlyList<GeneratedPuzzle>> Corpus = new(Load);

    public static IReadOnlyList<GeneratedPuzzle> All => Corpus.Value;

    public static GeneratedPuzzle Get(int index) => All[index];

    /// <summary>A deterministic spread of indices across the whole corpus.</summary>
    public static IEnumerable<object[]> Sample(int count)
    {
        var step = Math.Max(1, All.Count / count);

        return Enumerable.Range(0, count)
            .Select(i => i * step)
            .Where(index => index < All.Count)
            .Select(index => new object[] { index });
    }

    public static IReadOnlyList<GeneratedPuzzle> Take(int count) =>
        [.. Sample(count).Select(data => All[(int)data[0]])];

    private static IReadOnlyList<GeneratedPuzzle> Load()
    {
        var path = Path.Combine(AppContext.BaseDirectory, FileName);

        if (!File.Exists(path))
            throw new FileNotFoundException($"Puzzle corpus '{FileName}' was not copied to the test output.", path);

        var options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
        var puzzles = JsonSerializer.Deserialize<List<GeneratedPuzzle>>(File.ReadAllText(path), options);

        if (puzzles is null || puzzles.Count == 0)
            throw new InvalidOperationException($"Puzzle corpus '{FileName}' is empty.");

        return puzzles;
    }
}
