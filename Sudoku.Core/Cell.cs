namespace Sudoku.Core;

public class Cell
{
    public byte Value { get; set; }
    public bool IsGiven { get; }
    private ushort Candidates = 0;  // bitmask of candidates

    public Cell() {
        Value = 0;
        IsGiven = false;
    }

    public Cell(byte Value) {
        this.Value = Value;
        this.IsGiven = true;
    }

    public void addCandidate(byte cand) {
        if (cand > 8)
            throw new ArgumentOutOfRangeException(nameof(cand));

        Candidates |= (ushort)(1 << cand);
    }

    public bool HasCandidate(byte cand)
    {
        if (cand > 8)
            throw new ArgumentOutOfRangeException(nameof(cand));

        return (Candidates & (ushort)(1 << cand)) != 0;
    }

    public IEnumerable<byte> GetCandidates() {
        for (byte i = 0; i < 9; i++) {
            if (HasCandidate(i))
                yield return i;
        }
    }
}
