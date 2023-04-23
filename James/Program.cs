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
                services.AddHostedService<Worker>();
                services.AddSingleton<DiscordSocketClient>();
            })
            .Build();
        host.Run();
    }
}
