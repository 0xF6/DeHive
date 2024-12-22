using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using DeHive.Cli;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Spectre.Console.Cli;
using static Spectre.Console.AnsiConsole;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    System.Console.OutputEncoding = Encoding.Unicode;


await Host.CreateDefaultBuilder(args)
    .ConfigureLogging(x => x.SetMinimumLevel(LogLevel.None))
    .UseConsoleLifetime()
    .UseSpectreConsole(config => {
        config.SetApplicationCulture(CultureInfo.InvariantCulture);

        config.AddCommand<CreateDataBankCommand>("pack")
            .WithDescription("Create databank");
        config.AddCommand<PublishDataBankCommand>("publish")
            .WithDescription("Create databank by manifest");
        config.AddCommand<ViewBankDetailsCommand>("view")
            .WithDescription("View details about bank");


        config.SetExceptionHandler((ex, resolver) => {
            if (ex is CommandParseException exc)
            {
                MarkupLine($"[red]{exc.Message}[/]");
                return;
            }

            MarkupLine($"([orange1]{ex.GetType().FullName!.Split('.').Last().ToLowerInvariant().Replace("exception", "")}[/]) [red]{ex.Message}[/]");

            WriteException(ex);
        });
    })
    .RunConsoleAsync();


return Environment.ExitCode;