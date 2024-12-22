namespace DeHive.Abstractions;

using DeHive.Creation;

public record struct HiveVector3<Quantum>(Quantum x, Quantum y, Quantum z) : IHiveSerialization<HiveVector3<Quantum>> where Quantum : struct, INumber<Quantum>
{
    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveVector3<Quantum> data)
    {
        data.x = reader.ReadNumeric<Quantum>();
        data.y = reader.ReadNumeric<Quantum>();
        data.z = reader.ReadNumeric<Quantum>();
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveVector3<Quantum> data)
    {
        writer.WriteNumeric(data.x);
        writer.WriteNumeric(data.y);
        writer.WriteNumeric(data.z);
    }
}