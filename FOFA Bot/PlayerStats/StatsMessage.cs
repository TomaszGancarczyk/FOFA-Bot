using Discord;
using FOFA_Bot.Data;
using static System.Net.Mime.MediaTypeNames;

namespace FOFA_Bot.PlayerStats
{
    internal class StatsMessage
    {
        internal static async Task SendStatsMessage(string playername)
        {
            var stats = await PlayerApiHandler.GetPlayerStats(playername);
            EmbedBuilder message;
            if (stats == null)
            {
                message = GetErrorMessage(playername);
                await BotData.GetSignupsChannel().SendMessageAsync(embed: message.Build());
            }
            else
            {
                message = StatsMessageGenerator.CreateStatsMessage(stats);
                string image = GetFactionImage(stats.alliance);
                message.WithThumbnailUrl($"attachment://{stats.alliance.ToLower()}.webp");
                await BotData.GetSignupsChannel().SendFileAsync(image, embed: message.Build());
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
