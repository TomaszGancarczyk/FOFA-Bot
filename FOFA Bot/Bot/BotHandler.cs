using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Data;

namespace FOFA_Bot.Bot
{
    internal class BotHandler
    {
        private static DiscordSocketClient? Discord;
        private static int SignupMessageRunningCount = 0;

        internal static async Task Run(DiscordSocketClient discord)
        {
            Discord = discord;
            await Backup.ReadBackup();
            while (true)
            {
                _ = CheckSignupMessage();

                //_ = CheckActivityMessage();
                //TODO auto daily events
                //wekend 3h before cw golden drop + wild north
                //weekdays 2h before cw wild north
                //daily posting

                Task.Delay(60000).Wait();
            }
        }

        //private static async Task CheckActivityMessage()
        //{
        //    if (!ActivityMessageRunning && SettingsHandler.GetAutomnaticSignupMessage())
        //    {
        //        ActivityMessageRunning = true;
        //        //TODO auto daily events
        //        await AttendanceHandler.StartQuestionAttendanceEvent();
        //        Logger.LogInformation($"    Attendance event finished");
        //    }
        //}

        private static async Task CheckSignupMessage()
        {
            if (SignupMessageRunningCount == 0 && DateTime.Now.Hour == 21 && SettingsHandler.GetAutomnaticSignupMessage())
            {
                await AttendanceHandler.StartQuestionAttendanceEvent();
                Logger.LogInformation($"    Attendance event finished");
            }
        }
        internal static EmbedBuilder ChangeAutomnaticSignupMessage(bool status)
        {
            EmbedBuilder embed;
            SettingsHandler.SetAutomnaticSignupMessage(status);
            if (SettingsHandler.GetAutomnaticSignupMessage() == status)
                embed = MessageResponse.CreatePositiveStatusResponse(status);
            else embed = MessageResponse.CreateNegativeStatusResponse();
            return embed;
        }
        internal static void ChangeSignupMessageRunning(int changeCount)
        {
            SignupMessageRunningCount += changeCount;
            Logger.LogInformation($"Changed Signup Message count with count {changeCount}, current count: {SignupMessageRunningCount}");
        }
        internal static DiscordSocketClient? GetDiscord() => Discord;
    }
}
