namespace DeHive.Abstractions;

using DeHive.Creation;

public readonly record struct HiveStringTable(Dictionary<ulong, string> table) : IHiveSerialization<HiveStringTable>
{
    public const uint MagicHash = 0x81F3_A1EF;

    public static void OnDeserialize(ref HiveReader reader, scoped ref HiveStringTable data)
    {
        throw new NotImplementedException();
    }

    public static void OnSerialize(ref HiveWriter writer, scoped ref readonly HiveStringTable data)
    {
        writer.WriteASCIIString(".string_table");
        writer.WriteNumeric(MagicHash);
        writer.WriteNumeric(data.table.Count);
        foreach (var (index, @string) in data.table)
        {
            writer.WriteNumeric(index);
            writer.WriteString(@string);
        }
    }
}