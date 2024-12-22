namespace DeHive.Abstractions;

using DeHive.Creation;

public record struct HiveString(string str) : IHiveSerialization<HiveString>
{
    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveString data)
        => data.str = reader.ReadString();

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveString data) 
        => writer.WriteString(data.str);
}



public record struct HiveFixedBuffer(byte[] array) : IHiveSerialization<HiveFixedBuffer>
{
    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveFixedBuffer data)
    {
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveFixedBuffer data)
    {

    }
}
