namespace DeHive.Abstractions;

using DeHive.Creation;

public unsafe struct HiveShardHeader : IHiveSerialization<HiveShardHeader>
{
    public Ulid ShardId;
    public Ulid SetId;


    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveShardHeader data)
    {
        data.ShardId = reader.ReadId();
        data.SetId = reader.ReadId();
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveShardHeader data)
    {
        writer.WriteId(data.ShardId);
        writer.WriteId(data.SetId);
    }
}