namespace DeHive.Abstractions;

using DeHive.Creation;

public interface IHiveSerialization<T> where T : struct
{
    static abstract void OnDeserialize(ref HiveReader reader, scoped ref T data);
    static abstract void OnSerialize(ref HiveWriter writer, scoped ref readonly T data);
}