namespace DeHive.UpdateSystem;

public record AppManifest
{
    public required Guid AppId { get; set; }
    public required DirectoryInfo OutputPath { get; set; }
    public required string Branch { get; set; }
    public required List<AppDepot> Depots { get; set; }
}

public record AppDepot
{
    public required Guid DepotId { get; set; }
    public required DirectoryInfo ContentPath { get; set; }
    public required List<string> FileExclusions { get; set; }
    public required AppPlatform Platform { get; set; }
}

public enum AppPlatform
{
    NONE,
    WINDOWS,
    LINUX,
    MACOS,
    XBOX,
    STEAM_OS,
    ANDROID,
    IOS,
}