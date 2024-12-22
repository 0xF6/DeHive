namespace DeHive.Cli;

using Spectre.Console.Cli;
using System.ComponentModel;
using Spectre.Console;
using static Spectre.Console.AnsiConsole;

public class ViewBankDetailsCommandSettings : CommandSettings
{
    [Description("Bank path")]
    [CommandArgument(0, "[PATH]")]
    public required string PackageName { get; set; }
}

public class ViewBankDetailsCommand : AsyncCommand<ViewBankDetailsCommandSettings>
{
    public override async Task<int> ExecuteAsync(CommandContext context, ViewBankDetailsCommandSettings settings)
    {
        var bank = await HiveDataBank.OpenAsync(new FileInfo(settings.PackageName));
        var rule = new Rule("[blue] @ [/]");

        MarkupLine($"HiveBank [green]{bank.bankName}[/]");
        Write(rule);
        var header = bank.Header;
        MarkupLine($"Size [green]{bank.bankStream.Length}[/] bytes");
        MarkupLine($"Items [green]{header.EntityCount + header.ArchetypeCount + header.ComponentCount}[/]");
        MarkupLine($"Set/Shard [green]{header.ShardHeader.SetId}/{header.ShardHeader.ShardId}[/]");
        MarkupLine($"HiveVersion [green]{header.Version}[/]");
        Write(rule);

        var root = new Tree($"[yellow]@{settings.PackageName}[/]");
        var entities = bank.GetEntities();
        var max = entities.Max(x => x.Key.relative.Length);

        foreach (var (path, entity) in entities) 
            root.AddNode($"[yellow]{path.relative}{new string(' ', max - path.relative.Length)}[/] - :[gray]{entity.Id.Id}[/], offset: [red]+0x{entity.position:X8}[/] crc64: [purple]0x{entity.header.Crc64:X}[/]"); 

        Write(root);
        Write(rule);
        return 0;
    }
}