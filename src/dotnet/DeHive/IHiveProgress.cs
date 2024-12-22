namespace DeHive;

public interface IHiveProgress
{
    ulong Total { get; set; }
    void IncAndReport();
}