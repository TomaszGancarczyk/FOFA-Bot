using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Data;

namespace FOFA_Bot.Bot
{
    internal class BotHandler
    {
        private static DiscordSocketClient? Discord;
        private static int SignupMessageRunningCount = 0;
        internal static int SignupHour = 8;

        internal static async Task Run(DiscordSocketClient discord)
        {
            Logger.LogInformation("Starting program...");
            Discord = discord;
            await Backup.ReadBackup();
            Logger.LogInformation("Program is running");
            while (true)
            {
                _ = CheckSignupMessage();
                Task.Delay(60000).Wait();
            }
        }
        private static async Task CheckSignupMessage()
        {
            if (SignupMessageRunningCount == 0 && DateTime.Now.Hour == SignupHour && DateTime.Now.DayOfWeek == DayOfWeek.Monday && SettingsHandler.GetAutomnaticSignupMessage())
                AttendanceHandler.CreateAttendanceEvent();
        }
        internal static void ChangeSignupMessageRunning(int changeCount)
        {
            SignupMessageRunningCount += changeCount;
            Logger.LogInformation($"Changed Signup Message count with count {changeCount}, current count: {SignupMessageRunningCount}");
        }
        internal static DiscordSocketClient? GetDiscord() => Discord;
    }
}