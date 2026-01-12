using Discord;
using Discord.WebSocket;
using FOFA_Bot.Initializer;

namespace FOFA_Bot
{
    internal class Program
    {
        private readonly DiscordSocketClient Discord;
        private static readonly string Token = BotInitializer.GetDiscordToken();
        static Task Main()
        {
            Logger.LogInformation($"Starting...");

            //get data

            //start bot

            return Task.CompletedTask;
        }
    }
}