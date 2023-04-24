using Discord.WebSocket;

namespace James;

static class Program
{
    public static void Main(string[] args)
    {
        var host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, config) =>
            {
                config
                    .AddJsonFile("appsettings.local.json", true, true)
                    .AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.local.json", true, true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices(services =>
            {
                // Build Services
                services.AddHostedService<Worker>();
                services.AddSingleton<DiscordSocketClient>();
                services.RegisterServicesByConvention();

                // Configure Services
                using var scope = services.BuildServiceProvider().CreateScope();
                scope.ServiceProvider.ConfigureStartupServices();
            })
            .Build();
        host.Run();
    }
}
