namespace DeHive.Creation;

using Abstractions.Hives;

public record HiveComponentBuilder(HiveArchetypeBuilder ArchetypeBuilder, HiveComponentId componentId)
{
    public Dictionary<string, object> Fields { get; } = new();
    public Dictionary<string, HiveComponentId> Refs { get; } = new();

    public HiveComponentBuilder AddField<T>(string key, T value)
    {
        if (value == null) throw new ArgumentNullException(nameof(value));
        Fields.Add(key, value);
        return this;
    }

    public HiveComponentBuilder AddReference(string key, HiveComponentId value)
    {
        Refs.Add(key, value);
        return this;
    }
}