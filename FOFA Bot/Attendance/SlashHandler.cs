using Discord;

namespace FOFA_Bot.Attendance
{
    internal class SlashHandler
    {
        internal static Task<Embed> CreateSignupFromSlashCommand()
        {
            AttendanceHandler.CreateAttendanceEvent();
            return Task.FromResult(SuccessSignupMessage());
        }

        private static Embed SuccessSignupMessage()
        {
            EmbedBuilder embed = new()
            {
                Color = Color.Green,
                Title = "Successfully created signup message"
            };
            return embed.Build();
        }
        private static Embed DateErrorMessage(string message)
        {
            EmbedBuilder embed = new()
            {
                Color = Color.Red,
                Title = message
            };
            return embed.Build();
        }
    }
}
