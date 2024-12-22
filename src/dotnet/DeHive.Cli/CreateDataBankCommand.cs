namespace DeHive.Cli;

using Spectre.Console.Cli;
using System.ComponentModel;
using Spectre.Console;

public class CreateDataBankCommandSettings : CommandSettings
{
    [Description("Package name")]
    [CommandArgument(0, "[NAME]")]
    public required string PackageName { get; set; }

    [Description("Path to folder")]
    [CommandOption("--folder|-f")]
    public string InputFolder { get; set; }

    [Description("Path to output folder")]
    [CommandOption("--outputFolder|-o")]
    public string OutputFolder { get; set; }
}

public class CreateDataBankCommand : AsyncCommand<CreateDataBankCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, CreateDataBankCommandSettings settings)
    {
        await AnsiConsole.Progress()
            .Columns(
                new TaskDescriptionColumn(),
                new ProgressBarColumn(),
                new PercentageColumn(), 
                new RemainingTimeColumn(), 
                new SpinnerColumn())
            .StartAsync(async ctx => {
                // Define tasks
                var task = ctx.AddTask($"Generate databank [green]{settings.PackageName}.hb[/]...");

                var name = settings.PackageName;

                await HiveDataBank.CreateAsync(new HiveDatabankSettings(new DirectoryInfo(settings.InputFolder) ,
                    new DirectoryInfo(settings.OutputFolder), 1024 * 1024 * 1024, 128, name)
                {
                    EnableSharding = false
                }, builder =>
                {
                    builder.AddFolder(new DirectoryInfo(settings.InputFolder));
                }, new CommandProgress(task));
            });

        return 0;
    }
}


public class CommandProgress(ProgressTask task) : IHiveProgress
{
    public ulong Total { get; set; }
    public void IncAndReport()
    {
        task.MaxValue = Total;
        task.Value++;
    }
}