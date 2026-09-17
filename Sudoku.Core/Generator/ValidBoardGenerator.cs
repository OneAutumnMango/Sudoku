namespace Sudoku.Core.Generator;

using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;
using Sudoku.Core.Utils;

public class ValidBoardGenerator
{
    public static Board GenerateMinimalBoard(IRuleSet ruleSet)
    {
        var board = Generate(ruleSet);
        Minimise(board, ruleSet);
        return board;
    }

    public static Board Generate(IRuleSet ruleSet)
    {
        ArgumentNullException.ThrowIfNull(ruleSet);

        var board = new Board(9);
        var rng = Random.Shared;

        if (!Fill(board, ruleSet, 0, rng))
        {
            throw new InvalidOperationException("Failed to generate a valid board.");
        }

        return board;
    }

    public static void Minimise(Board board, IRuleSet ruleSet)
    {
        ArgumentNullException.ThrowIfNull(board);
        ArgumentNullException.ThrowIfNull(ruleSet);

        if (board.EnumerateEmptyCells().Any())
            throw new InvalidOperationException("Provided board contains empty cells.");
        if (ruleSet.FindFirstUnsatisfiedConstraint(board).IsSome)
            throw new InvalidOperationException("Provided board does not satisfy the rule set.");

        var rng = Random.Shared;
        GreedyRemoval(board, ruleSet, rng);
    }

    private static bool GreedyRemoval(Board board, IRuleSet ruleSet, Random rng)
    {
        var anyRemoved = false;
        var cells = board.EnumerateFilledCells()
            .Select(_ => _.cell)
            .OrderBy(_ => rng.Next())
            .ToList();

        foreach (var cell in cells)
        {
            var originalValue = cell.Value;
            SetCellValue(board, ruleSet, cell, 0);

            if (IsUniqueSolution(board, ruleSet))
            {
                anyRemoved = true;
                continue;
            }

            SetCellValue(board, ruleSet, cell, originalValue);
        }

        return anyRemoved;
    }

    private static bool IsUniqueSolution(Board board, IRuleSet ruleSet)
    {
        return CountSolutions(board, ruleSet, limit: 2) == 1;
    }

    private static int CountSolutions(Board board, IRuleSet ruleSet, int limit)
    {
        if (limit <= 0)
            return 0;

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
            SetCellValue(board, ruleSet, bestCell, (byte)(candidate + 1));

            count += CountSolutions(board, ruleSet, limit - count);

            SetCellValue(board, ruleSet, bestCell, 0);

            if (count >= limit)
                return count;
        }

        return count;
    }

    private static bool Fill(Board board, IRuleSet ruleSet, int index, Random rng)
    {
        if (index >= board.Size * board.Size)  // geq for safety?
            return true;

        var row = index / board.Size;
        var col = index % board.Size;

        // safeguard and allows if i ever want to set cells in the future
        if (board[row, col].Value != 0)
            return Fill(board, ruleSet, index + 1, rng);

        ruleSet.ComputeAndFillCandidates(board);
        var candidates = board[row, col].GetCandidates()
            .OrderBy(_ => rng.Next())
            .ToList();

        foreach (byte value in candidates)
        {
            SetCellValue(board, ruleSet, board[row, col], (byte)(value + 1));

            if (Fill(board, ruleSet, index + 1, rng))
                return true;

            SetCellValue(board, ruleSet, board[row, col], 0);
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

    private static void SetCellValue(Board board, IRuleSet ruleSet, Cell cell, byte value)
    {
        cell.Value = value;
        ruleSet.ComputeAndFillCandidates(board);
    }
}