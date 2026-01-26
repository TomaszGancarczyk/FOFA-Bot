using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Nades;

namespace FOFA_Bot.Bot
{
    internal class BotHandler
    {
        private static DiscordSocketClient? Discord;

        private static bool NadeMessageRunning = false;
        private static bool AutomnaticNadeMessage = true;

        private static bool SignupMessageRunning = false;
        private static bool AutomnaticSignupMessage = true;

        internal static void Run(DiscordSocketClient discord)
        {
            Discord = discord;
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
            if (!SignupMessageRunning && AutomnaticSignupMessage)
            {
                //TODO   DateTime.Now.Hour == 22 for checksignups
                //if (DateTime.Now.Hour == 22)
                {
                    SignupMessageRunning = true;
                    await AttendanceHandler.StartQuestionAttendanceEvent();
                    Task.Delay(3600000).Wait();
                    SignupMessageRunning = false;
                }
            }
        }

        internal static DiscordSocketClient GetDiscord() => Discord;
    }
}
