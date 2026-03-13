using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Data;

namespace FOFA_Bot.Bot
{
    internal class BotHandler
    {
        private static DiscordSocketClient? Discord;
        private static bool SignupMessageRunning = false;
        private static bool NadeMessageRunning = false;

        internal static async Task Run(DiscordSocketClient discord)
        {
            Discord = discord;
            await AttendanceBackup.ReadBackup();
            while (true)
            {
                _ = CheckSignupMessage();

                Task.Delay(60000).Wait();
            }
        }
        private static async Task CheckSignupMessage()
        {
            if (!SignupMessageRunning && DateTime.Now.Hour == 20 && SettingsHandler.GetAutomnaticSignupMessage())
            {
                SignupMessageRunning = true;
                await AttendanceHandler.StartQuestionAttendanceEvent();
                Logger.LogInformation($"    Attendance event finished");
            }
        }
        internal static EmbedBuilder ChangeAutomnaticSignupMessage(bool status)
        {
            EmbedBuilder embed;
            SettingsHandler.SetAutomnaticSignupMessage(status);
            if (SettingsHandler.GetAutomnaticSignupMessage() == status)
                embed = AttendanceMessageResponse.CreatePositiveStatusResponse(status);
            else embed = AttendanceMessageResponse.CreateNegativeStatusResponse();
            return embed;
        }
        internal static void SetSignupMessageRunning(bool status) => SignupMessageRunning = status;
        internal static DiscordSocketClient GetDiscord() => Discord;
    }
}
