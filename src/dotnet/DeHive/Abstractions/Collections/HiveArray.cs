namespace DeHive.Abstractions.Collections;

using Abstractions;
using Creation;

public record struct HiveArray<T>(T[] array) : IHiveSerialization<HiveArray<T>> where T : struct, IHiveSerialization<T>
{
    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveArray<T> data)
    {
        var size = reader.ReadNumeric<int>();
        data.array = new T[size];
        foreach (var index in ..size)
            T.OnDeserialize(ref reader, ref data.array[index]);
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveArray<T> data)
    {
        writer.WriteNumeric(data.array.Length);
        foreach (var v in data.array)
            T.OnSerialize(ref writer, in v);
    }
}