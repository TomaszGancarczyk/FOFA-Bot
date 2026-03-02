using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Data;
using FOFA_Bot.Nades;
using FOFA_Bot.PlayerStats;

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
            //TODO remove after testing
            await StatsMessage.SendStatsMessage("Stagnant_Water");
            await StatsMessage.SendStatsMessage("miki_mus");
            await StatsMessage.SendStatsMessage("YmMingg");
            await StatsMessage.SendStatsMessage("monker");
            await StatsMessage.SendStatsMessage("JasperG");
            await StatsMessage.SendStatsMessage("Monker_ZXC");
            await StatsMessage.SendStatsMessage("asdasdasdasd");
            //TODO remove after testing
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
            if (!SignupMessageRunning && DateTime.Now.Hour == 20 && DateTime.Now.Minute >= 25 && DateTime.Now.Minute <= 30 && SettingsHandler.GetAutomnaticSignupMessage())
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
