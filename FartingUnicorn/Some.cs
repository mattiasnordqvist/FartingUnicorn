namespace FartingUnicorn;

public class Some<T> : Option<T>
{
    public T Value { get; }
    public Some(T value) => Value = value;

    public override bool Equals(object? obj)
        => obj is Some<T> some && Value.Equals(some.Value);

    public override int GetHashCode() => Value.GetHashCode();

}
