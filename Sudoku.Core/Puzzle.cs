using Sudoku.Core.Grid;
using Sudoku.Core.RuleSets;

namespace Sudoku.Core;

public class Puzzle
{
    private readonly IRuleSet _ruleset;
    private readonly Board _board;

    public Puzzle(IRuleSet ruleset)
    {
        _ruleset = ruleset;
        _board = new Board(9);
    }

    public void SetCell(int row, int col, byte value)
    {
        _board[row, col].Value = value;
    }

    public void ClearCell(int row, int col)
    {
        SetCell(row, col, 0b0);
    }

    public void AddCandidate(int row, int col, byte value)
    {
        _board[row, col].AddCandidate(value);
    }

    public void RemoveCandidate(int row, int col, byte value)
    {
        _board[row, col].RemoveCandidate(value);
    }
}