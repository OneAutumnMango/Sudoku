namespace Sudoku.Core.Utils;

public readonly struct Option<T>
{
    private bool _exists { get; }
    public bool IsNone => !_exists;
    public bool IsSome => _exists;
    public T Value { get; }

    public Option(T value) { _exists = true; Value = value; }
    public static Option<T> None => default;

    public T GetValueOrDefault(T defaultValue) => _exists ? Value : defaultValue;

    public T GetValueOrThrow(string? message = null) =>
        _exists ? Value : throw new InvalidOperationException(message ?? "Option has no value");

    public Option<U> Map<U>(Func<T, U> mapper) =>
        _exists ? new Option<U>(mapper(Value)) : Option<U>.None;

    public Option<U> FlatMap<U>(Func<T, Option<U>> mapper) =>
        _exists ? mapper(Value) : Option<U>.None;

    public Option<T> Filter(Func<T, bool> predicate) =>
        _exists && predicate(Value) ? this : None;

    public override bool Equals(object? obj) =>
        obj is Option<T> opt &&
        _exists == opt._exists &&
        (!_exists || (Value?.Equals(opt.Value) ?? false));

    public override int GetHashCode() =>
        _exists ? Value?.GetHashCode() ?? 0 : 0;

    public override string ToString() =>
        _exists ? $"Some({Value})" : "None";
}