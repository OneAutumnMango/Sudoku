namespace Sudoku.Core.Generator;

using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

public class ValidBoardGenerator
{
    public static Board Generate(IRuleSet ruleSet)
    {
        var board = new Board(9);
        var rng = Random.Shared;

        if (!Fill(board, ruleSet, 0, rng))
        {
            throw new InvalidOperationException("Failed to generate a valid board.");
        }

        return board;
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

        var candidates = GetCandidates(board, ruleSet, row, col).ToList();
        Shuffle(candidates, rng);

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

    private static void Shuffle<T>(IList<T> items, Random rng)
    {
        for (int i = items.Count - 1; i > 0; i--)
        {
            int j = rng.Next(i + 1);
            (items[i], items[j]) = (items[j], items[i]);
        }
    }
}