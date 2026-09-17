using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Core;

public class Puzzle(IRuleSet ruleset)
{
    public IRuleSet RuleSet { get; } = ruleset;
    public Board Board { get; } = new Board(9);

    public void SetCell(int row, int col, byte value)
    {
        Board[row, col].Value = value;
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