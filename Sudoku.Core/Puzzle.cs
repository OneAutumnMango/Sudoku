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
    }

    public Puzzle(IRuleSet ruleset, Board board)
    {
        RuleSet = ruleset;
        Board = board;
    }

    public Puzzle(IRuleSet ruleset, int[,] values): 
        this(ruleset, new Board(values)) {}

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