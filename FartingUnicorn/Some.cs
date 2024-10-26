namespace FartingUnicorn;

public class Some<T> : Option<T>
{
    public T Value { get; }
    public Some(T value) => Value = value;
}
