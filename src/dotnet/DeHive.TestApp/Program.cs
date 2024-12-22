using DeHive;
using DeHive.Abstractions;
using DeHive.Abstractions.Hives;

var builder = new HiveDatabankBuilder(new HiveDatabankSettings(
    new DirectoryInfo("Z:\\!argon\\client\\src\\ui\\dist"),
    new DirectoryInfo("./out"), 1024 * 1024 * 512, 0, "front")
{
    EnableSharding = false
});



builder.AddFolder(new DirectoryInfo("Z:\\!argon\\client\\src\\ui\\dist"));

await builder.BuildToAsync(new Reporter());

//var d = await HiveDataBank.OpenAsync(new FileInfo("./out/front.hb"));

public class Reporter : IHiveProgress
{
    public ulong Total { get; set; }
    public long Current;
    public void IncAndReport()
    {
        var current = Interlocked.Add(ref Current, 1);
        var percent = (double)current / Total * 100;

        if (percent % 2 == 0)
            Console.WriteLine($"{percent:0.00}%");
    }
}

