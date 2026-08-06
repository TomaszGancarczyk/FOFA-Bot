using Discord;
using Discord.WebSocket;

namespace FOFA_Bot.Attendance
{
    internal class MessageResponse
    {
        internal static async Task RespondWithOldSignupError(SocketMessageComponent component)
        {
            Logger.LogWarning($"    {component.User.Username} interacted with old signup");
            EmbedBuilder embed = new();
            embed
                .WithColor(Color.DarkGrey)
                .WithTitle($"This is signup is closed");
            await component.RespondAsync(embed: embed.Build(), ephemeral: true);
        }

        internal static EmbedBuilder CreatePositiveStatusResponse(bool status)
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Green);
            embed.WithTitle($"    Status successfully changed to {status}");
            return embed;
        }
        internal static EmbedBuilder CreateNegativeStatusResponse()
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red);
            embed.WithTitle($"    Run into error when changing status");
            return embed;
        }
    }
}
