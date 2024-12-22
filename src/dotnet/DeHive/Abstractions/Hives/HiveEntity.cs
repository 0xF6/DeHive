namespace DeHive.Abstractions;

using Hives;

public record HiveEntity(HiveEntityHeader header, long position)
{
    public HiveEntityId Id => header.Id;
    public Stream Retrieve(Stream bankStream) => new SlicedStream(bankStream, position, (long)header.Length);
}