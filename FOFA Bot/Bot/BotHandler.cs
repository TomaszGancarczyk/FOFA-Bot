using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Data;
using FOFA_Bot.Nades;

namespace FOFA_Bot.Bot
{
    internal class BotHandler
    {
        private static DiscordSocketClient? Discord;
        private static bool SignupMessageRunning = false;
        private static bool NadeMessageRunning = false;

        internal static void Run(DiscordSocketClient discord)
        {
            Discord = discord;
            while (true)
            {
                //_ = CheckNadeMessage();

                _ = CheckSignupMessage();

                Task.Delay(60000).Wait();
            }
        }
        private static async Task CheckNadeMessage()
        {
            if (!NadeMessageRunning && SettingsHandler.GetAutomnaticNadeMessage())
            {
                if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.Hour == 22)
                {
                    NadeMessageRunning = true;
                    await NadeHandler.StartNadeEvent();
                    NadeMessageRunning = false;
                }
            }
        }
        private static async Task CheckSignupMessage()
        {
            if (!SignupMessageRunning && DateTime.Now.Hour == 22 && SettingsHandler.GetAutomnaticSignupMessage())
            {
                SignupMessageRunning = true;
                await AttendanceHandler.StartQuestionAttendanceEvent();
                Logger.LogInformation($"Attendance event finished");
                Task.Delay(3600000).Wait();
                SignupMessageRunning = false;
            }
        }
        internal static EmbedBuilder ChangeAutomnaticSignupMessage(bool status)
        {
            EmbedBuilder embed = new EmbedBuilder();
            SettingsHandler.SetAutomnaticSignupMessage(status);
            if (SettingsHandler.GetAutomnaticSignupMessage() == status)
                embed = AttendanceMessageResponse.CreatePositiveStatusResponse(status);
            else embed = AttendanceMessageResponse.CreateNegativeStatusResponse();
            return embed;
        }

        internal static DiscordSocketClient GetDiscord() => Discord;
    }
}
