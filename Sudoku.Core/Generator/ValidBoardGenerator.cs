namespace Sudoku.Core.Generator;

using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Utils;

public class ValidBoardGenerator
{
    public static Puzzle GenerateMinimalBoard(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        Generate(puzzle);
        Minimise(puzzle);
        return puzzle;
    }

    public static Puzzle Generate(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        var rng = Random.Shared;

        if (!Fill(puzzle, 0, rng))
        {
            throw new InvalidOperationException("Failed to generate a valid board.");
        }

        return puzzle;
    }

    public static Puzzle Minimise(Puzzle puzzle)
    {
        ArgumentNullException.ThrowIfNull(puzzle);

        var board = puzzle.Board;
        var ruleSet = puzzle.RuleSet;

        if (board.EnumerateEmptyCells().Any())
            throw new InvalidOperationException("Provided board contains empty cells.");
        if (ruleSet.FindFirstUnsatisfiedConstraint(board).IsSome)
            throw new InvalidOperationException("Provided board does not satisfy the rule set.");

        var rng = Random.Shared;
        GreedyRemoval(puzzle, rng);

        return puzzle;
    }

    private static bool GreedyRemoval(Puzzle puzzle, Random rng)
    {
        var anyRemoved = false;
        var board = puzzle.Board;
        var ruleSet = puzzle.RuleSet;

        var cells = board.EnumerateFilledCells()
            .Select(_ => _.cell)
            .OrderBy(_ => rng.Next())
            .ToList();

        foreach (var cell in cells)
        {
            var originalValue = cell.Value;
            SetCellValue(puzzle, cell, 0);

            if (IsUniqueSolution(puzzle))
            {
                anyRemoved = true;
                continue;
            }

            SetCellValue(puzzle, cell, originalValue);
        }

        return anyRemoved;
    }

    private static bool IsUniqueSolution(Puzzle puzzle)
    {
        return CountSolutions(puzzle, limit: 2) == 1;
    }

    private static int CountSolutions(Puzzle puzzle, int limit)
    {
        if (limit <= 0)
            return 0;

        var board = puzzle.Board;
        var ruleSet = puzzle.RuleSet;

        ruleSet.ComputeAndFillCandidates(board);

        var bestSelection = GetBestCellCandidates(board);
        if (bestSelection.IsNone)
            return 1;

        var (bestCell, bestCandidates) = bestSelection.Value;
        if (bestCandidates.Count == 0)
            return 0;

        var count = 0;
        foreach (var candidate in bestCandidates)
        {
            SetCellValue(puzzle, bestCell, candidate);

            count += CountSolutions(puzzle, limit - count);

            SetCellValue(puzzle, bestCell, 0);

            if (count >= limit)
                return count;
        }

        return count;
    }

    private static bool Fill(Puzzle puzzle, int index, Random rng)
    {
        var board = puzzle.Board;
        var ruleSet = puzzle.RuleSet;

        if (index >= board.Size * board.Size)  // geq for safety?
            return true;

        var row = index / board.Size;
        var col = index % board.Size;

        // safeguard and allows if i ever want to set cells in the future
        if (board[row, col].Value != 0)
            return Fill(puzzle, index + 1, rng);

        ruleSet.ComputeAndFillCandidates(board);
        var candidates = board[row, col].GetCandidates()
            .OrderBy(_ => rng.Next())
            .ToList();

        foreach (byte value in candidates)
        {
            SetCellValue(puzzle, board[row, col], value);

            if (Fill(puzzle, index + 1, rng))
                return true;

            SetCellValue(puzzle, board[row, col], 0);
        }

        return false;
    }

    private static Option<(Cell cell, List<byte> candidates)> GetBestCellCandidates(Board board)
    {
        Option<(Cell cell, List<byte> candidates)> bestSelection = Option<(Cell cell, List<byte> candidates)>.None;

        foreach (var (_, _, cell) in board.EnumerateEmptyCells())
        {
            var candidates = cell.GetCandidates().ToList();
            var candidateCount = candidates.Count;

            if (candidateCount == 0)
            {
                return new Option<(Cell cell, List<byte> candidates)>((cell, candidates));
            }

            if (bestSelection.IsNone || candidateCount < bestSelection.Value.candidates.Count)
            {
                bestSelection = new Option<(Cell cell, List<byte> candidates)>((cell, candidates));

                if (candidateCount == 1)
                    break;
            }
        }

        return bestSelection;
    }

    private static void SetCellValue(Puzzle puzzle, Cell cell, byte value)
    {
        cell.Value = value;
        puzzle.RuleSet
            .ComputeAndFillCandidates(puzzle.Board);
    }
}