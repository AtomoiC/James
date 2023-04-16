using James;

internal class Program
{
    public static void Main(string[] args)
    {
        IHost host = Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((ctx, config) =>
            {
                config.AddJsonFile($"appsettings.{ctx.HostingEnvironment.EnvironmentName}.local.json", true, true);
            })
            .ConfigureServices(services => { services.AddHostedService<Worker>(); })
            .Build();

        host.Run();
    }
}


