namespace Sudoku.Core.Solver;

public enum Difficulty
{
    Unknown,
    Simple,         // basic elimination
    Easy,           // easy deductions with minimal tracking
    Intermediate,   // requires tracking and cognition of simple patterns
    Advanced,       // more deliberate scanning, less obvious patterns, and more complex deductions
    Expert,         // complex patterns, difficult to spot relationships and substatial candidate analysis
    Master          // complex deductions, often involving multiple interacting patterns/chains
}
