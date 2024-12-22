namespace DeHive.Abstractions;

using DeHive;

internal class ZeroProgress : IHiveProgress
{
    public ulong Total { get; set; }
    public void IncAndReport()
    {
    }
}