namespace DeHive.Abstractions;

using DeHive;
using Hives;
using Creation;
using Extensions;

public class HiveDatabankBuilder(HiveDatabankSettings settings)
{
    private readonly Dictionary<HiveRelativePath, HiveEntityBuilder> entities = new();
    private readonly Dictionary<HiveRelativePath, HiveComponentBuilder> components = new();
    private readonly Dictionary<HiveRelativePath, HiveArchetypeBuilder> archetypes = new();

    public void AddFile(FileInfo path)
    {
        var relativePath = CreateRelativePath(path);
        var eb = new HiveEntityBuilder(HiveEntityId.New(Path.GetFileNameWithoutExtension(path.Name)), path);

        entities.Add(relativePath, eb);
    }

    public void AddFolder(DirectoryInfo info)
    {
        foreach (var file in info.EnumerateFiles("*", SearchOption.AllDirectories)) AddFile(file);
    }

    public void AddArchetype(HiveArchetypeId archetypeId, HiveRelativePath path, Action<HiveArchetypeBuilder> builder)
    {
        var e = new HiveArchetypeBuilder(this, archetypeId);
        builder(e);
        archetypes.Add(path, e);
    }



    public async Task BuildToAsync(IHiveProgress? progress)
    {
        progress ??= new ZeroProgress();
        if (!settings.outputPath.Exists)
            settings.outputPath.Create();
        var shards = GenerateHiveShards();
        progress.Total = (ulong)shards.Sum(x =>
            x.archetypes.Count + x.components.Count + x.entities.Count);

        await Task.WhenAll(
            BuildEntities(settings.outputPath, shards, progress), 
            BuildComponents(settings.outputPath, shards, progress),
            BuildArchetypes(settings.outputPath, shards, progress));
    }


    private List<HiveShard> GenerateHiveShards()
    {
        var shardSize = settings.shardMaximumSize;
        var shards = new List<HiveShard>();

        shards.AddRange(GenerateShards(entities.OrderByDescending(x => x.Value.filePath.Name),
            (shard, key, value) => shard.entities.Add(key, value),
            value => value.filePath.Length, shardSize));

        shards.AddRange(GenerateShards(components.OrderByDescending(x => x.Value.componentId.Name),
            (shard, key, value) => shard.components.Add(key, value),
            value => 100, shardSize));

        shards.AddRange(GenerateShards(archetypes.OrderByDescending(x => x.Value.objectId.Name),
            (shard, key, value) => shard.archetypes.Add(key, value),
            value => value.Size, shardSize));

        return shards;
    }

    private IEnumerable<HiveShard> GenerateShards<T>(
        IEnumerable<KeyValuePair<HiveRelativePath, T>> items,
        Action<HiveShard, HiveRelativePath, T> addToShard,
        Func<T, long> getSize,
        long shardSize)
    {
        var currentShard = new HiveShard()
        {
            setId = Ulid.NewUlid(),
            shardId = Ulid.NewUlid()
        };
        var currentSize = 0L;

        foreach (var (key, value) in items)
        {
            var itemSize = getSize(value);
            currentSize += itemSize;

            addToShard(currentShard, key, value);

            if (!settings.EnableSharding)
                continue;

            if (currentSize < shardSize)
                continue;
            yield return currentShard;
            currentShard = new HiveShard()
            {
                setId = Ulid.NewUlid(),
                shardId = Ulid.NewUlid()
            };
            currentSize = 0;
        }

        if (currentSize > 0)
            yield return currentShard;
    }

    private async Task BuildEntities(DirectoryInfo outputDirectory, List<HiveShard> shards, IHiveProgress progress)
    {
        foreach (var (shard, index) in shards.Where(x => x.entities.Count != 0).Select((x, y) => (x, y)).AsParallel())
        {
            var bankFile = settings.EnableSharding
                ? outputDirectory.File($"{settings.bankName}.{index:D4}.hb")
                : outputDirectory.File($"{settings.bankName}.hb");

            if (bankFile.Exists)
                bankFile.Delete();
            await using var stream = bankFile.OpenWrite();
            
            var header = HiveBankHeader.Create();
            header.EntityCount = shard.entities.Count;
            header.ShardHeader = new HiveShardHeader
            {
                SetId = shard.setId,
                ShardId = shard.shardId
            };
            stream.WriteHiveHeader(header);
            foreach (var entity in shard.entities)
            {
                progress.IncAndReport();
                await stream.WriteFile(entity.Value, CreateRelativePath(entity.Value.filePath));
            }
        }
    }

    private async Task BuildComponents(DirectoryInfo outputDirectory, List<HiveShard> shards, IHiveProgress progress)
    {
        foreach (var (shard, index) in shards.Where(x => x.components.Count != 0).Select((x, y) => (x, y)).AsParallel())
        {
            var bankFile = outputDirectory.File($"{settings.bankName}.{index:D4}.hb");
            if (bankFile.Exists)
                throw new Exception();
            await using var stream = bankFile.OpenWrite();

            var header = HiveBankHeader.Create();
            header.ComponentCount = shard.components.Count;
            header.ShardHeader = new HiveShardHeader
            {
                SetId = shard.setId,
                ShardId = shard.shardId
            };
            stream.WriteHiveHeader(header);
            foreach (var entity in shard.components)
            {
                progress.IncAndReport();
                stream.WriteComponent(entity.Value);
            }
        }
    }

    private async Task BuildArchetypes(DirectoryInfo outputDirectory, List<HiveShard> shards, IHiveProgress progress)
    {
        foreach (var (shard, index) in shards.Where(x => x.archetypes.Count != 0).Select((x, y) => (x, y)).AsParallel())
        {
            var bankFile = outputDirectory.File($"{settings.bankName}.{index:D4}.hb");

            if (bankFile.Exists)
                throw new Exception();

            await using var stream = bankFile.OpenWrite();

            var header = HiveBankHeader.Create();
            header.ArchetypeCount = shard.archetypes.Count;
            header.ShardHeader = new HiveShardHeader
            {
                SetId = shard.setId,
                ShardId = shard.shardId
            };
            stream.WriteHiveHeader(header);
            foreach (var entity in shard.archetypes)
            {
                progress.IncAndReport();
                stream.WriteArchetype(entity.Value);
            }
        }
    }

    private HiveRelativePath CreateRelativePath(FileInfo file)
    {
        if (!file.FullName.StartsWith(settings.root.FullName, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("File incorrect path");
        return new HiveRelativePath(file.FullName.Substring(settings.root.FullName.Length).TrimStart(Path.DirectorySeparatorChar));
    }
}