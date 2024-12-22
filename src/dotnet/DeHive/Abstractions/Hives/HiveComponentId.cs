namespace DeHive.Abstractions.Hives;

using Abstractions;
using Creation;

public record struct HiveComponentId(Ulid Id, string Name) : IHiveSerialization<HiveComponentId>
{
    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveComponentId data)
    {
        data.Id = reader.ReadId();
        data.Name = reader.ReadString();
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveComponentId data)
    {
        writer.WriteId(data.Id);
        writer.WriteString(data.Name);
    }

    public static HiveComponentId New(string name) => new(Ulid.NewUlid(), name);
}