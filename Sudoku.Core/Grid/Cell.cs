namespace Sudoku.Core.Grid;

public class Cell
{
    public static readonly ushort AllCandidates = 0b111111111;  // bitmask of all candidates
    private byte _value;
    public byte Value
    {
        get => _value;
        set
        {
            if (value < 0 || value > 9)
                throw new ArgumentOutOfRangeException(nameof(value));

            _value = value;

            if (value != 0)
                RemoveCandidate(value);
        }
    }

    public bool IsGiven { get; set; }
    public ushort Candidates { get; set; } = AllCandidates;

    public Cell()
    {
        _value = 0;
        IsGiven = false;
    }

    public Cell(byte value)
    {
        Value = value;
        IsGiven = true;
    }

    private void ValidateCandidate(byte cand)
    {
        if (cand < 1 || cand > 9)
            throw new ArgumentOutOfRangeException(nameof(cand));
    }

    public void AddCandidate(byte cand)
    {
        ValidateCandidate(cand);
        byte bitIndex = (byte)(cand - 1);
        Candidates |= (ushort)(1 << bitIndex);
    }

    public void RemoveCandidate(byte cand)
    {
        ValidateCandidate(cand);
        byte bitIndex = (byte)(cand - 1);
        Candidates &= (ushort)~(1 << bitIndex);
    }

    public bool HasCandidate(byte cand)
    {
        ValidateCandidate(cand);
        byte bitIndex = (byte)(cand - 1);
        return (Candidates & (ushort)(1 << bitIndex)) != 0;
    }

    public IEnumerable<byte> GetCandidates()
    {
        for (byte i = 1; i <= 9; i++)
        {
            if (HasCandidate(i))
                yield return i;
        }
    }

    public ushort GetCandidatesMask()
    {
        return Candidates;
    }

    public void IntersectCandidates(ushort other)
    {
        Candidates &= other;
    }
}
