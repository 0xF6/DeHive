namespace DeHive.Abstractions.Hives;

using Creation;

public unsafe struct HiveComponent : IHiveSerialization<HiveComponent>
{
    public HiveComponentId Id;
    public HiveString LayerName;
    public Memory<byte> Data;


    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveComponent data)
    {
        data.Id = reader.Read<HiveComponentId>();
        data.LayerName = reader.Read<HiveString>();
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveComponent data)
    {
        throw new NotImplementedException();
    }
}