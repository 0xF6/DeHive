namespace DeHive.Creation;

using Abstractions;
using Abstractions.Hives;

public record HiveArchetypeBuilder(HiveDatabankBuilder databankBuilder, HiveArchetypeId objectId)
{
    public long Size { get; }

    public HiveArchetypeBuilder AddComponent(HiveComponentId componentId, Action<HiveComponentBuilder> builder)
    {
        return this;
    }
}