using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Data;

namespace FOFA_Bot.Bot
{
    internal class BotHandler
    {
        private static DiscordSocketClient? Discord;
        private static int SignupMessageRunningCount = 0;
        internal static int SignupQuestionHour = 21;

        internal static async Task Run(DiscordSocketClient discord)
        {
            Logger.LogInformation("Starting program...");
            Discord = discord;
            await Backup.ReadBackup();
            while (true)
            {
                _ = CheckSignupMessage();
                Task.Delay(60000).Wait();
            }
        }
        private static async Task CheckSignupMessage()
        {
            if (SignupMessageRunningCount == 0 && DateTime.Now.Hour == SignupQuestionHour && SettingsHandler.GetAutomnaticSignupMessage())
            {
                await AttendanceHandler.StartQuestionAttendanceEvent();
                Logger.LogInformation($"    Attendance event finished");
            }
        }
        internal static void ChangeSignupMessageRunning(int changeCount)
        {
            SignupMessageRunningCount += changeCount;
            Logger.LogInformation($"Changed Signup Message count with count {changeCount}, current count: {SignupMessageRunningCount}");
        }
        internal static DiscordSocketClient? GetDiscord() => Discord;
    }
}
