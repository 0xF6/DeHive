namespace DeHive.Abstractions.Hives;

using Abstractions;
using Creation;

public record struct HiveEntityId(Ulid Id, string Name) : IHiveSerialization<HiveEntityId>
{
    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveEntityId data)
    {
        data.Id = reader.ReadId();
        data.Name = reader.ReadString();
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveEntityId data)
    {
        writer.WriteId(data.Id);
        writer.WriteString(data.Name);
    }


    public static HiveEntityId New(string Name) => new HiveEntityId(Ulid.NewUlid(), Name);
}