using Discord;
using Discord.WebSocket;

namespace FOFA_Bot.PlayerStats
{
    internal class StatsMessage
    {
        internal static async Task SendStatsMessage(SocketSlashCommand command)
        {
            string playername = command.Data.Options.First().Value.ToString();
            if (playername == null)
            {
                await command.RespondAsync(embed: GetErrorMessage("").Build());
                return;
            }
            var stats = await PlayerApiHandler.GetPlayerStats(playername);
            EmbedBuilder message;
            if (stats == null)
            {
                await command.RespondAsync(embed: GetErrorMessage(playername).Build());
            }
            else
            {
                message = StatsMessageGenerator.CreateStatsMessage(stats);
                string image = GetFactionImage(stats.alliance);
                message.WithThumbnailUrl($"attachment://{stats.alliance.ToLower()}.webp");
                await command.RespondWithFileAsync(image, embed: message.Build());
            }
        }
        private static string GetFactionImage(string faction)
        {
            string factionImage = $"attachment://..\\..\\..\\..\\Images\\{faction.ToLower()}.webp";
            return factionImage;
        }
        private static EmbedBuilder GetErrorMessage(string playername)
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red)
                .WithTitle($"Couldn't find player {playername}");
            return embed;
        }
    }
}
