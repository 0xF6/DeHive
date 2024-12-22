namespace DeHive.Abstractions;

using Collections;
using Hives;
using Creation;
using System.Diagnostics;

public unsafe struct HiveEntityHeader : IHiveSerialization<HiveEntityHeader>
{
    public HiveEntityId Id;
    public ulong Length;
    public HiveString Name;
    public HiveString Extensions;
    public HiveString RelativePath;
    public HiveArray<HiveString> Tags;
    public ulong Crc64;
    public ReservedValues reserved1;
    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveEntityHeader data)
    {
        data.Id = reader.Read<HiveEntityId>();
        data.Length = reader.ReadNumeric<ulong>();
        data.Name = reader.Read<HiveString>();
        data.RelativePath = reader.Read<HiveString>();
        data.Extensions = reader.Read<HiveString>();
        data.Tags = reader.Read<HiveArray<HiveString>>();
        data.Crc64 = reader.ReadNumeric<ulong>();
        data.reserved1 = reader.ReadStruct<ReservedValues>();
        Debug.Assert(reader.ReadNumeric<byte>() == 0xA);
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveEntityHeader data)
    {
        writer.Write(in data.Id);
        writer.WriteNumeric(data.Length);
        writer.Write(in data.Name);
        writer.Write(in data.RelativePath);
        writer.Write(in data.Extensions);
        writer.Write(in data.Tags);
        writer.WriteNumeric(data.Crc64);
        writer.WriteStruct(data.reserved1);
        writer.WriteNumeric((byte)0xA);
    }
}