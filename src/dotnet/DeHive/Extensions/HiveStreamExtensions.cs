using DeHive.Abstractions;
using DeHive.Abstractions.Hives;
using DeHive.Creation;

namespace DeHive.Extensions;

using System.IO;
using System;
using System.Buffers;
using System.IO.Hashing;
using Abstractions.Collections;

public static class HiveStreamExtensions
{
    public static void WriteHiveHeader(this Stream stream, HiveBankHeader header)
    {
        var writer = new HiveWriter(stream);

        HiveBankHeader.OnSerialize(ref writer, in header);
    }

    public static void WriteHiveStringTable(this Stream stream, Dictionary<ulong, string> strings)
    {
        var writer = new HiveWriter(stream);
        var table = new HiveStringTable(strings);
        HiveStringTable.OnSerialize(ref writer, in table);
    }

    public static void WriteComponent(this Stream stream, HiveComponentBuilder builder)
    {
    }

    public static void WriteArchetype(this Stream stream, HiveArchetypeBuilder builder)
    {

    }

    public static async Task WriteFile(this Stream stream, HiveEntityBuilder builder, HiveRelativePath relativePath, CancellationToken ct = default)
    {
        var writer = new HiveWriter(stream);
        var header = new HiveEntityHeader()
        {
            Length = (ulong)builder.filePath.Length,
            Name = new HiveString(builder.filePath.Name),
            Extensions = new HiveString(builder.filePath.Extension),
            RelativePath = new HiveString(relativePath.relative),
            Id = builder.entityId,
            Tags = new HiveArray<HiveString>([]),
            Crc64 = await ComputeCrc64(builder.filePath, ct)
        };
        HiveEntityHeader.OnSerialize(ref writer, in header);
        writer.WriteASCIIString(".e_file");
        await using var reader = builder.filePath.OpenRead();
        await reader.CopyToAsync(stream, ct);
    }

    private static async ValueTask<ulong> ComputeCrc64(FileInfo file, CancellationToken ct = default)
    {
        var crc64 = new Crc64();
        await using var stream = file.OpenRead();
        var bytesRead = 0;
        var bytes = MemoryPool<byte>.Shared.Rent(8192);
        while ((bytesRead = await stream.ReadAsync(bytes.Memory, ct)) > 0) 
            crc64.Append(bytes.Memory.Span[..bytesRead]);
        return crc64.GetCurrentHashAsUInt64();
    }
}