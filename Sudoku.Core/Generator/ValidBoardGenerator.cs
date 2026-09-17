namespace Sudoku.Core.Generator;

using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

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

        while (GreedyRemoval(board, ruleSet))
            continue;
    }

    private static bool GreedyRemoval(Board board, IRuleSet ruleSet)
    {
        var anyRemoved = false;
        var rng = Random.Shared;
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

        var emptyCells = board.EnumerateEmptyCells();
        if (!emptyCells.Any())
            return 1;

        var count = 0;
        var (row, col, cell) =  emptyCells.First();
        var candidates = GetCandidates(board, ruleSet, row, col);

        foreach (byte val in candidates)
        {
            cell.Value = val;

            count += CountSolutions(board, ruleSet, limit - count);

            cell.Value = 0;

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

    private static IEnumerable<byte> GetCandidates(Board board, IRuleSet ruleSet, int row, int col)
    {
        for (byte i = 1; i <= board.Size; i++)
        {
            board[row, col].Value = i;

            if (ruleSet.FindFirstUnsatisfiedConstraint(board).IsNone)
                yield return i;

            board[row, col].Value = 0;
        }
    }
}