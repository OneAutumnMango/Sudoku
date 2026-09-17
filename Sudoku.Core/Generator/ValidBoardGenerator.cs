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
            cell.Value = 0;

            if (IsUniqueSolution(board, ruleSet))
            {
                anyRemoved = true;
                continue;
            }

            cell.Value = originalValue;
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

        var bestSelection = GetBestCellCandidates(board, ruleSet);
        if (bestSelection.IsNone)
            return 1;

        var (bestCell, bestCandidates) = bestSelection.Value;
        if (bestCandidates.Count == 0)
            return 0;

        var count = 0;
        foreach (var candidate in bestCandidates)
        {
            bestCell.Value = candidate;

            count += CountSolutions(board, ruleSet, limit - count);

            bestCell.Value = 0;

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

        var candidates = GetCandidates(board, ruleSet, row, col)
            .OrderBy(_ => rng.Next());

        foreach (byte val in candidates)
        {
            board[row, col].Value = val;

            if (Fill(board, ruleSet, index + 1, rng))
                return true;

            board[row, col].Value = 0;  // revert
        }

        return false;
    }

    private static Option<(Cell cell, List<byte> candidates)> GetBestCellCandidates(Board board, IRuleSet ruleSet)
    {
        Option<(Cell cell, List<byte> candidates)> bestSelection = Option<(Cell cell, List<byte> candidates)>.None;

        foreach (var (row, col, cell) in board.EnumerateEmptyCells())
        {
            var candidates = GetCandidates(board, ruleSet, row, col).ToList();
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

    private static IEnumerable<byte> GetCandidates(
        Board board,
        IRuleSet ruleSet,
        int row,
        int col)
    {
        return ruleSet is StandardRuleSet
            ? GetStandardCandidates(board, row, col)
            : GetRuleSetCandidates(board, ruleSet, row, col);
    }

    private static IEnumerable<byte> GetRuleSetCandidates(Board board, IRuleSet ruleSet, int row, int col)
    {
        for (byte i = 1; i <= board.Size; i++)
        {
            board[row, col].Value = i;

            if (ruleSet.FindFirstUnsatisfiedConstraint(board).IsNone)
                yield return i;

            board[row, col].Value = 0;
        }
    }

    private static IEnumerable<byte> GetStandardCandidates(Board board, int row, int col)
    {
        var used = new bool[10];

        for (var i = 0; i < board.Size; i++)
        {
            var rowValue = board[row, i].Value;
            if (rowValue != 0)
                used[rowValue] = true;

            var colValue = board[i, col].Value;
            if (colValue != 0)
                used[colValue] = true;
        }

        var boxSize = (int)Math.Sqrt(board.Size);
        var boxRowStart = (row / boxSize) * boxSize;
        var boxColStart = (col / boxSize) * boxSize;

        for (var r = boxRowStart; r < boxRowStart + boxSize; r++)
        {
            for (var c = boxColStart; c < boxColStart + boxSize; c++)
            {
                var value = board[r, c].Value;
                if (value != 0)
                    used[value] = true;
            }
        }

        for (byte value = 1; value <= board.Size; value++)
        {
            if (!used[value])
                yield return value;
        }
    }
}