using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Reflection;
using System.Threading.Tasks;

public class Program
{
    private DiscordSocketClient? _client;
    private CommandService? _commands;
    private IServiceProvider? _services;

    public static void Main(string[] args)
        => new Program().RunBotAsync().GetAwaiter().GetResult();

    public async Task RunBotAsync()
    {
        _client = new DiscordSocketClient();
        _commands = new CommandService();

        _client.Log += Log;

        await RegisterCommandsAsync();

        await _client.LoginAsync(TokenType.Bot, "TOKEN");

        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask;
    }

    public async Task RegisterCommandsAsync()
    {
        _client.MessageReceived += HandleCommandAsync;

        await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);
    }


    private async Task HandleCommandAsync(SocketMessage arg)
    {
        var message = arg as SocketUserMessage;
        var context = new SocketCommandContext(_client, message);

        if (message.Author.IsBot) return;

        int argPos = 0;
        if (message.HasStringPrefix("!", ref argPos))
        {
            var result = await _commands.ExecuteAsync(context, argPos, _services);
            if (!result.IsSuccess)
                Console.WriteLine(result.ErrorReason);
        }
    }
}

public class HelloCommandModule : ModuleBase<SocketCommandContext>
{
    [Command("hello")]
    public async Task HelloCommand()
    {
        await ReplyAsync("Hello! How can I assist you?");
    }

}

public class HelpCommandModule : ModuleBase<SocketCommandContext>
{
    [Command("help")]
    public async Task HelpCommand()
    {
        await ReplyAsync("`This is a help message that you can modify.`");
    }
}
