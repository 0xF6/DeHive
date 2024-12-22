namespace DeHive.Abstractions;

internal static class DirectoryEx
{
    public static FileInfo File(this DirectoryInfo info, string file) => new(Path.Combine(info.FullName, file));
}