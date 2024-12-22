namespace DeHive.Cli;

using Spectre.Console.Cli;
using System.ComponentModel;
using Abstractions;
using Newtonsoft.Json;
using Spectre.Console;
using static Spectre.Console.AnsiConsole;

public class PublishDataBankCommandSettings : CommandSettings
{
    [Description("Manifest path")]
    [CommandArgument(0, "[PATH]")]
    public required string ManifestPatch { get; set; }
}
public class PublishDataBankCommand : AsyncCommand<PublishDataBankCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, PublishDataBankCommandSettings settings)
    {
        var fileEntity = settings.ManifestPatch.Equals(".")
            ? new DirectoryInfo(settings.ManifestPatch).File("dehive.manifest.json")
            : new FileInfo(settings.ManifestPatch);

        if (!fileEntity.Exists)
        {
            MarkupLine($"[red]Error[/] [gray]'{fileEntity.FullName}'[/] is not found");
            return -1;
        }

        MarkupLine($"Use [gray]'{fileEntity.FullName}'[/] manifest...");

        var manifest = JsonConvert.DeserializeObject<DeHiveManifest>(await File.ReadAllTextAsync(fileEntity.FullName));


        var packSettings = new DeHive.HiveDatabankSettings(new DirectoryInfo(manifest.Root),
            new DirectoryInfo(manifest.OutputPath), 1024 * 1024 * 1024, 1024, manifest.BankName)
        {
            EnableSharding = false,
        };

        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(),
                new RemainingTimeColumn(),
                new SpinnerColumn())
            .StartAsync(async ctx => {
                // Define tasks
                var task = ctx.AddTask($"Generate databank [green]{packSettings.bankName}.hb[/]...");

                await HiveDataBank.CreateAsync(packSettings, builder => {
                    builder.AddFolder(new DirectoryInfo(manifest.Root));
                }, new CommandProgress(task));
            });
        return 0;
    }
}


public record DeHiveManifest
{
    [JsonProperty("name")]
    public required string BankName { get; set; }
    [JsonProperty("root")]
    public required string Root { get; set; }
    [JsonProperty("output")]
    public required string OutputPath { get; set; }
}