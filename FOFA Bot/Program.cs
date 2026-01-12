using Discord;
using Discord.WebSocket;
using FOFA_Bot.Bot;

namespace FOFA_Bot
{
    internal class Program
    {
        private readonly DiscordSocketClient Discord;
        private static string Token;
        static async Task Main()
        {
            Logger.LogInformation($"Starting...");
            await BotData.LoadJson();
            Token = BotData.GetDiscordToken();
            Logger.LogInformation($"[FOFA] Bot is starting");
            new Program().StartBotAsync().GetAwaiter().GetResult();
            await Task.CompletedTask;
        }
        private Program()
        {
            DiscordSocketConfig? config = new()
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 100,
                GatewayIntents = GatewayIntents.All,
                UseInteractionSnowflakeDate = false
            };
            Discord = new DiscordSocketClient(config);
        }
        internal async Task StartBotAsync()
        {
            await Discord.LoginAsync(TokenType.Bot, Token);
            await Discord.StartAsync();
            Discord.Ready += DiscordReady;
            Discord.Ready += () =>
            {
                Discord.ButtonExecuted += ButtonEventHandler.Handler;
                Discord.SlashCommandExecuted += SlashCommandHandler.Handler;
                Logger.LogInformation($"[FOFA] Bot is running");
                return Task.CompletedTask;
            };
            await Task.Delay(3000);
            await BotHandler.Run(Discord);
            await Task.Delay(-1);
        }
        private async Task DiscordReady()
        {
            //commands
        }
    }
}