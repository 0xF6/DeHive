namespace DeHive.Abstractions;

using DeHive.Creation;

public record HiveShard
{
    public Dictionary<HiveRelativePath, HiveEntityBuilder> entities { get; set; } = new();
    public Dictionary<HiveRelativePath, HiveComponentBuilder> components { get; set; } = new();
    public Dictionary<HiveRelativePath, HiveArchetypeBuilder> archetypes { get; set; } = new();
    public Ulid shardId { get; set; }
    public Ulid setId { get; set; }
}