using Discord;
using Discord.WebSocket;

namespace FOFA_Bot.Bot
{
    internal class BotHandler
    {
        private static SocketGuild? Guild;
        private static IMessageChannel? QuestionChannel;
        private static IMessageChannel? SignupsChannel;
        private static IMessageChannel? NadeChannel;
        internal static void Run(DiscordSocketClient discord)
        {
            Guild = discord.GetGuild(DataInitializer.GetGuildId());
            Logger.LogInformation($"Found Guild: {Guild.Name}");

            QuestionChannel = (IMessageChannel)Guild.GetChannel(DataInitializer.GetQuestionChannelId());
            Logger.LogInformation($"Found Nade Channel: {QuestionChannel.Name}");

            SignupsChannel = (IMessageChannel)Guild.GetChannel(DataInitializer.GetSignupsChannelId());
            Logger.LogInformation($"Found Nade Channel: {SignupsChannel.Name}");

            NadeChannel = (IMessageChannel)Guild.GetChannel(DataInitializer.GetNadeChannelId());
            Logger.LogInformation($"Found Nade Channel: {NadeChannel.Name}");

            _ = StartEvents();
        }
        internal static async Task StartEvents()
        {
            Console.WriteLine("eee");
        }
    }
}
