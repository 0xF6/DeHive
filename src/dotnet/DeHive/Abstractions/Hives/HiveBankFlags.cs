namespace DeHive.Abstractions.Hives;

[Flags]
public enum HiveBankFlags : ulong
{
    NONE = 0,
    COMPRESSION = 1 << 1,
    SIGNATURE = 1 << 2,
    ROOT_WORLD = 1 << 3,
}