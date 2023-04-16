using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace James;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly DiscordSocketClient _client;
    private readonly IConfiguration _configuration;

    public Worker(ILogger<Worker> logger, DiscordSocketClient client, IConfiguration configuration)
    {
        _logger = logger;
        _client = client;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _client.LoginAsync(TokenType.Bot, _configuration.GetValue<string>("Discord:Bot:Token"));
        await _client.StartAsync();

        _client.Log += Log;
        _client.Ready += OnClientReady;
        _client.SlashCommandExecuted += OnSlashCommand;
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }

        await _client.LogoutAsync();
        await _client.StopAsync();
    }

    private async Task OnSlashCommand(SocketSlashCommand arg)
    {
        if (arg.CommandName.Equals("first-command"))
        {
            await arg.RespondAsync(arg.User.Username + " => (╯°□°)╯︵ ┻━┻");
        }

        if (arg.CommandName.Equals("hurensohn"))
        {
            await arg.RespondAsync(arg.User.Username + " ist ein HURENSOHN");
        }
    }

    private Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }

    private async Task OnClientReady()
    {
        var targetGuild = _client.Guilds.First().Id;
        var guild = _client.GetGuild(targetGuild);
        
        var firstCommand = new SlashCommandBuilder()
            .WithName("first-command")
            .WithDescription("This is my first guild slash command!");

        var insultMeCommand = new SlashCommandBuilder()
            .WithName("hurensohn")
            .WithDescription("Sagt du wärst ein ...");
        
        try
        {
            await guild.CreateApplicationCommandAsync(firstCommand.Build());
            await guild.CreateApplicationCommandAsync(insultMeCommand.Build());
        }
        catch (HttpException exception)
        {
            var json = JsonConvert.SerializeObject(exception.Reason, Formatting.Indented);
            Console.WriteLine(json);
        }
    }
}