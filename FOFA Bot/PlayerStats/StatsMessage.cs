using Discord;
using FOFA_Bot.Data;

namespace FOFA_Bot.PlayerStats
{
    internal class StatsMessage
    {
        internal static async Task SendStatsMessage(string playername)
        {
            var stats = await PlayerApiHandler.GetPlayerStats("Stagnant_Water");
            EmbedBuilder message = StatsMessageGenerator.CreateStatsMessage(stats);
            string image = GetFactionImage(stats.alliance);
            await BotData.GetSignupsChannel().SendFileAsync(image, embed: message.Build());
        }
        internal static string GetFactionImage(string faction)
        {
            string factionImage = $"attachment://..\\..\\..\\..\\Images\\{faction.ToLower()}.webp";
            return factionImage;
        }
    }
}
