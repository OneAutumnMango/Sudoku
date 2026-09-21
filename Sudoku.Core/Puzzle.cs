using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Core;

public class Puzzle
{
    public IRuleSet RuleSet { get; }
    public Board Board => RuleSet.Board;

    public Puzzle(IRuleSet ruleset)
    {
        ArgumentNullException.ThrowIfNull(ruleset);
        RuleSet = ruleset;
        RuleSet.ComputeAndFillCandidates();
    }

    public Puzzle(IRuleSet ruleset, int[,] values):
        this(ruleset)
    {
        ArgumentNullException.ThrowIfNull(values);

        if (values.GetLength(0) != Board.Size || values.GetLength(1) != Board.Size)
            throw new ArgumentException($"Values must be a {Board.Size}x{Board.Size} matrix.", nameof(values));

        for (var row = 0; row < Board.Size; row++)
        {
            for (var col = 0; col < Board.Size; col++)
                Board[row, col].Value = (byte)values[row, col];
        }

        RuleSet.ComputeAndFillCandidates();
    }

    public bool UpdateCell(int row, int col, byte value)
    {
        return UpdateCell(Board[row, col], value);
    }

    internal bool UpdateCell(Cell cell, byte value)
    {
        ArgumentNullException.ThrowIfNull(cell);

        if (cell.Value == value)
            return false;

        cell.ResetRuleCandidates();
        cell.Value = value;
        RuleSet.UpdateCandidates(cell);
        return true;
    }

    public void SetCell(int row, int col, byte value)
    {
        UpdateCell(row, col, value);
    }

    public void ClearCell(int row, int col)
    {
        SetCell(row, col, 0b0);
    }

    public void RemoveCandidate(int row, int col, byte value)
    {
        Board[row, col].RemoveCandidate(value);
    }

    public void RemoveCandidate(Cell cell, byte value)
    {
        cell.RemoveCandidate(value);
    }

    public void RemoveCandidates(Cell cell, ushort candidates)
    {
        cell.RemoveCandidates(candidates);
    }
}