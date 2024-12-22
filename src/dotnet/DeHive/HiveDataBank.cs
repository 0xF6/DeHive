namespace DeHive;

using Abstractions;
using Abstractions.Hives;
using Creation;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;

public record HiveDataBank(Stream bankStream, string bankName) : IDisposable, IAsyncDisposable
{
    public HiveBankHeader Header { get; private set; }
    private readonly Dictionary<HiveRelativePath, HiveEntity> entities = new();
    private readonly Dictionary<HiveRelativePath, HiveComponentBuilder> components = new();
    private readonly Dictionary<HiveRelativePath, HiveArchetypeBuilder> archetypes = new();
    private bool isDisposed { get; set; }

    public Stream? FindFile(HiveEntityId entityId) => entities.FirstOrDefault(x => x.Value.Id == entityId).Value?.Retrieve(bankStream);

    public Stream? FindFile(HiveRelativePath path)
    {
        if (entities.TryGetValue(path, out var e))
            return e.Retrieve(bankStream);
        return null;
    }
     
    public Dictionary<HiveRelativePath, HiveEntity> GetEntities() => entities;

    public static ValueTask<HiveDataBank> OpenAsync(FileInfo file)
        => OpenAsync(file.OpenRead(), Path.GetFileNameWithoutExtension(file.Name));

    private static ValueTask<HiveDataBank> OpenAsync(Stream stream, string name)
    {
        var bank = new HiveDataBank(stream, name);
        var binary = new BinaryReader(stream);
        var reader = new HiveReader(binary);
        var header = new HiveBankHeader();
        HiveBankHeader.OnDeserialize(ref reader, ref header);
        bank.Header = header;
        foreach (var _ in ..(int)header.EntityCount)
        {
            var eHeader = new HiveEntityHeader();
            HiveEntityHeader.OnDeserialize(ref reader, ref eHeader);
            Debug.Assert(reader.ReadString(Encoding.ASCII) == ".e_file");
            bank.entities.Add(new HiveRelativePath(eHeader.RelativePath.str), new HiveEntity(eHeader, stream.Position));
            stream.Position += (long)eHeader.Length;
        }

        return new ValueTask<HiveDataBank>(bank);
    }

    public static async ValueTask CreateAsync(HiveDatabankSettings settings, Action<HiveDatabankBuilder> builder, IHiveProgress? progress = null)
    {
        var pipe = new HiveDatabankBuilder(settings);
        builder(pipe);
        await pipe.BuildToAsync(progress);
    }

    private void ThrowIfDisposed()
    {
        if (!isDisposed) return;
        throw new ObjectDisposedException("already disposed");
    }

    public void Dispose()
    {
        ThrowIfDisposed();
        entities.Clear();
        components.Clear();
        archetypes.Clear();
        bankStream.Dispose();
        isDisposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        ThrowIfDisposed();
        entities.Clear();
        components.Clear();
        archetypes.Clear();
        await bankStream.DisposeAsync();
        isDisposed = true;
    }
}