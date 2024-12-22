namespace DeHive.Abstractions;

using DeHive.Creation;

public record struct HiveVector2<Quantum>(Quantum x, Quantum y) : IHiveSerialization<HiveVector2<Quantum>> where Quantum : struct, INumber<Quantum>
{
    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveVector2<Quantum> data)
    {
        data.x = reader.ReadNumeric<Quantum>();
        data.y = reader.ReadNumeric<Quantum>();
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveVector2<Quantum> data)
    {
        writer.WriteNumeric(data.x);
        writer.WriteNumeric(data.y);
    }
}