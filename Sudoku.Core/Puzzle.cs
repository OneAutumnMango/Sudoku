using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Core;

public class Puzzle
{
    public IRuleSet RuleSet { get; }
    public Board Board { get; }

    public Puzzle(IRuleSet ruleset)
    {
        RuleSet = ruleset;
        Board = new Board(9);
        RuleSet.ComputeAndFillCandidates(Board);
    }

    public Puzzle(IRuleSet ruleset, Board board)
    {
        RuleSet = ruleset;
        Board = board;
        RuleSet.ComputeAndFillCandidates(Board);
    }

    public Puzzle(IRuleSet ruleset, int[,] values): 
        this(ruleset, new Board(values)) {}

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
        RuleSet.UpdateCandidates(Board, cell);
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

    public void AddCandidate(int row, int col, byte value)
    {
        Board[row, col].AddCandidate(value);
    }

    public void RemoveCandidate(int row, int col, byte value)
    {
        Board[row, col].RemoveCandidate(value);
    }
}