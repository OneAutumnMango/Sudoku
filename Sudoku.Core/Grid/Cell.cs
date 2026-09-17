namespace Sudoku.Core.Grid;

public class Cell
{
    private byte _value;
    public byte Value
    {
        get => _value;
        set
        {
            if (value < 0 || value > 9)
                throw new ArgumentOutOfRangeException(nameof(value));
            _value = value;
        }
    }

    public bool IsGiven { get; set; }
    private ushort _candidates = 0;  // bitmask of candidates

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
        if (cand < 0 || cand > 8)
            throw new ArgumentOutOfRangeException(nameof(cand));
    }

    public void AddCandidate(byte cand)
    {
        ValidateCandidate(cand);
        _candidates |= (ushort)(1 << cand);
    }

    public void RemoveCandidate(byte cand)
    {
        ValidateCandidate(cand);
        _candidates &= (ushort)~(1 << cand);
    }

    public bool HasCandidate(byte cand)
    {
        ValidateCandidate(cand);
        return (_candidates & (ushort)(1 << cand)) != 0;
    }

    public IEnumerable<byte> GetCandidates()
    {
        for (byte i = 0; i < 9; i++)
        {
            if (HasCandidate(i))
                yield return i;
        }
    }
}
