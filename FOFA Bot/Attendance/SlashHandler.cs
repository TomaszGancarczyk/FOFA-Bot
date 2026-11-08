using Discord;

namespace FOFA_Bot.Attendance
{
    internal class SlashHandler
    {
        internal static Task<Embed> CreateSignupFromSlashCommand(string clan)
        {
            AttendanceHandler.CreateAttendanceEvent(clan);
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
    }
}
