namespace FartingUnicorn;

public class None<T> : Option<T> {
    public override bool Equals(object? obj) => obj is None<T>;
    public override int GetHashCode() => typeof(T).GetHashCode();
}