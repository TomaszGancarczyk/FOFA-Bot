using Discord;
using Discord.WebSocket;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceMessageResponds
    {
        internal static async Task RespondWithOldSignupError(SocketMessageComponent component)
        {
            Logger.LogWarning($"{component.User.Username} interacted with old signup");
            EmbedBuilder embed = new();
            embed
                .WithColor(Color.DarkGrey)
                .WithTitle($"This is signup is closed");
            await component.RespondAsync(embed: embed.Build(), ephemeral: true);
        }

        internal static async Task RespondWithSignupStatus(SocketMessageComponent component, bool status)
        {
            Logger.LogInformation($"Responding to {component.User.Username} button event"); EmbedBuilder embed = new();
            Color color;
            string message;
            if (status)
            {
                color = Color.Green;
                message = $"Registered for event";
            }
            else
            {
                color = Color.Red;
                message = $"Unregistered up for event";
            }
            embed
                .WithColor(color)
                .WithTitle(message);
            await component.FollowupAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}
