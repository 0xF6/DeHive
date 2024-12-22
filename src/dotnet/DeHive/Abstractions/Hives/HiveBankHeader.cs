namespace DeHive.Abstractions.Hives;

using Abstractions;
using Creation;
using System.Diagnostics;
using System.Text;

public unsafe struct HiveBankHeader : IHiveSerialization<HiveBankHeader>
{
    public const ushort VERSION = 1;

    public string MagicName; // "HIVE.BANK"
    public ushort Version;
    public fixed byte Signature[256];

    public HiveShardHeader ShardHeader;
    public HiveBankFlags Flags;

    public long EntityCount;
    public long ComponentCount;
    public long ArchetypeCount;

    public ReservedValues reserved1;


    public static HiveBankHeader Create()
    {
        var header = new HiveBankHeader
        {
            MagicName = "HIVE.BANK",
            Version = VERSION
        };
        return header;
    }


    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveBankHeader data)
    {
        data.MagicName = reader.ReadRaw(9, Encoding.ASCII);
        data.Version = reader.ReadNumeric<ushort>();
        _ = reader.ReadRaw(256, Encoding.ASCII);
        Debug.Assert(reader.ReadNumeric<byte>() == 0xA);
        data.ShardHeader = reader.Read<HiveShardHeader>();
        data.Flags = (HiveBankFlags)reader.ReadNumeric<ulong>();
        data.EntityCount = reader.ReadNumeric<long>();
        data.ComponentCount = reader.ReadNumeric<long>();
        data.ArchetypeCount = reader.ReadNumeric<long>();
        data.reserved1 = reader.ReadStruct<ReservedValues>();
        Debug.Assert(reader.ReadNumeric<byte>() == 0xA);
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveBankHeader data)
    {
        writer.WriteRaw(data.MagicName, Encoding.ASCII);
        writer.WriteNumeric(data.Version);
        fixed (byte* sig = data.Signature)
            writer.WriteRaw(sig, 256);
        writer.WriteNumeric((byte)0xA);
        writer.Write(in data.ShardHeader);
        writer.WriteNumeric((ulong)data.Flags);
        writer.WriteNumeric(data.EntityCount);
        writer.WriteNumeric(data.ComponentCount);
        writer.WriteNumeric(data.ArchetypeCount);
        writer.WriteStruct(data.reserved1);
        writer.WriteNumeric((byte)0xA);
    }
}