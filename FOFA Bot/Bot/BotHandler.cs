using Discord;
using Discord.WebSocket;
using FOFA_Bot.Nades;
using FOFA_Bot.Attendance;

namespace FOFA_Bot.Bot
{
    internal class BotHandler
    {
        private static SocketGuild? Guild;
        private static IMessageChannel? QuestionChannel;
        private static IMessageChannel? SignupsChannel;
        private static IMessageChannel? NadeChannel;

        private static bool NadeMessageRunning = false;
        private static bool AutomnaticNadeMessage = true;

        private static bool SignupMessageRunning = false;
        private static bool AutomnaticSignupMessage = true;

        internal static async Task Run(DiscordSocketClient discord)
        {
            Guild = discord.GetGuild(BotData.GetGuildId());
            Logger.LogInformation($"Found Guild: {Guild.Name}");
            QuestionChannel = (IMessageChannel)Guild.GetChannel(BotData.GetQuestionChannelId());
            Logger.LogInformation($"Found Nade Channel: {QuestionChannel.Name}");
            SignupsChannel = (IMessageChannel)Guild.GetChannel(BotData.GetSignupsChannelId());
            Logger.LogInformation($"Found Nade Channel: {SignupsChannel.Name}");
            NadeChannel = (IMessageChannel)Guild.GetChannel(BotData.GetNadeChannelId());
            Logger.LogInformation($"Found Nade Channel: {NadeChannel.Name}");

            while (true)
            {
                CheckNadeMessage();

                CheckSignupMessage();

                Task.Delay(60000).Wait();
            }
        }
        private static async Task CheckNadeMessage()
        {
            if (!NadeMessageRunning && AutomnaticNadeMessage)
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Monday && DateTime.Now.Hour == 20)
                {
                    NadeMessageRunning = true;
                    await NadeHandler.StartNadeEvent();
                    NadeMessageRunning = false;
                }
            }
        }
        private static async Task CheckSignupMessage()
        {
            if (!SignupMessageRunning && AutomnaticSignupMessage)
            {
                SignupMessageRunning = true;
                await AttendanceHandler.StartAttendanceEvent();
                SignupMessageRunning = false;
            }
        }
    }
}
