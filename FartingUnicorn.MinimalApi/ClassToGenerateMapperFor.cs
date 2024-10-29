namespace MapperGenerator;

public readonly record struct ClassToGenerateMapperFor
{
    public readonly string Name;
    public readonly string Ns;

    public ClassToGenerateMapperFor(string name, string ns)
    {
        Name = name;
        Ns = ns;
    }
}
