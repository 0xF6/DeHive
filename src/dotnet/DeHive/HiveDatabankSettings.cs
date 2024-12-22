namespace DeHive;

public record HiveDatabankSettings(DirectoryInfo root, DirectoryInfo outputPath, long shardMaximumSize, ulong blockAlign, string bankName)
{
    public bool EnableSharding { get; set; } = true;
}