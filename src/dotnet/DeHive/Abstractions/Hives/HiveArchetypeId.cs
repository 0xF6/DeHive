namespace DeHive.Abstractions;

using Creation;

public record struct HiveArchetypeId(Ulid Id, string Name) : IHiveSerialization<HiveArchetypeId>
{
    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveArchetypeId data)
    {
        data.Id = reader.ReadId();
        data.Name = reader.ReadString();
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveArchetypeId data)
    {
        writer.WriteId(data.Id);
        writer.WriteString(data.Name);
    }

    public static HiveArchetypeId New(string Name) => new(Ulid.NewUlid(), Name);
}