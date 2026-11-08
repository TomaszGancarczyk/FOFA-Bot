using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Data;

namespace FOFA_Bot.Bot
{
    internal class BotHandler
    {
        private static DiscordSocketClient? Discord;
        private static int RofaSignupMessageRunningCount = 0;
        private static int BofaSignupMessageRunningCount = 0;
        internal static int SignupHour = 8;

        internal static async Task Run(DiscordSocketClient discord)
        {
            Logger.LogInformation("Starting program...");
            Discord = discord;
            await Backup.ReadBackup();
            Logger.LogInformation("Program is running");
            while (true)
            {
                CheckSignupMessage();
                Task.Delay(60000).Wait();
            }
        }
        private static void CheckSignupMessage()
        {
            if (RofaSignupMessageRunningCount == 0 && DateTime.Now.Hour == SignupHour && DateTime.Now.DayOfWeek == DayOfWeek.Monday && SettingsHandler.GetAutomnaticSignupMessage("ROFA"))
                AttendanceHandler.CreateAttendanceEvent("ROFA");
            if (RofaSignupMessageRunningCount == 0 && DateTime.Now.Hour == SignupHour && DateTime.Now.DayOfWeek == DayOfWeek.Monday && SettingsHandler.GetAutomnaticSignupMessage("BOFA"))
                AttendanceHandler.CreateAttendanceEvent("BOFA");
        }
        internal static void ChangeSignupMessageRunning(int changeCount, string clan)
        {
            if (clan == "ROFA")
                RofaSignupMessageRunningCount += changeCount;
            else if (clan == "BOFA")
                BofaSignupMessageRunningCount += changeCount;
            Logger.LogInformation($"Changed Signup Message count with count {changeCount}, current count: {RofaSignupMessageRunningCount}");
        }
        internal static DiscordSocketClient? GetDiscord() => Discord;
    }
}