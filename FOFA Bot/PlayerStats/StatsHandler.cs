using Discord;
using Discord.WebSocket;


namespace FOFA_Bot.PlayerStats
{
    internal class StatsHandler
    {
        internal static EmbedBuilder CreateStatsMessage(Stats player)
        {
            EmbedBuilder embed = new();
            switch (player.Faction)
            {
                case "Rise":
                    embed.WithColor(Color.Green);
                    break;
                case "Frontier":
                    embed.WithColor(Color.Red);
                    break;
                case "Covenant":
                    embed.WithColor(Color.Purple);
                    break;
                case "Mercenaries":
                    embed.WithColor(Color.Blue);
                    break;
                case "Stalkers":
                    embed.WithColor(Color.Gold);
                    break;
                case "Bandits":
                    embed.WithColor(Color.LightGrey);
                    break;
            }
            string clanString = "";
            string clansJoinedString = "";
            EmbedFieldBuilder GameTimeField = new()
            {
                Name = $"Game Time",
                Value = $"- {player.PlaytimeHours} hours since {player.JoinedGame}\n" +
                $"- Last login: {player.LastLogin}",
                IsInline = false,
            };
            EmbedFieldBuilder PveField = new()
            {
                Name = $"PVE",
                Value = $"- NPC Kills: {player.NpcKills}\n" +
                $"- MutantKills: {player.MutantKills}",
                IsInline = false,
            };
            EmbedFieldBuilder PvpField = new()
            {
                Name = $"PVP",
                Value = $"- Kills: {player.Kills}\n" +
                $"- Deaths: {player.Deaths}\n" +
                $"- Assists: {player.Assists}\n" +
                $"- Suicides: {player.Suicides}\n" +
                $"- Total K/D: {player.TotalKD}\n" +
                $"- Session K/D: {player.SessionKD}",
                IsInline = false,
            };
            EmbedFieldBuilder OtherField = new()
            {
                Name = $"Other",
                Value = $"- Highest Money: {player.HighestMoney}\n" +
                $"- Artifacts Found: {player.ArtifactsFound}\n" +
                $"- Bolts Thrown: {player.BoltsThrown}\n" +
                $"- Deliveries Made: {player.DeliveriesMade}\n" +
                $"- Caches Found: {player.CachesFound}\n" +
                $"- Signals Found: {player.SignalsFound}",
                IsInline = false,
            };

            if (player.Clan != null)
                clanString = $"**{player.ClanRank} of {player.ClanTag} {player.Clan}**\n";
            if (player.TimesJoinedClan > 0)
                clansJoinedString = $"and member of {player.TimesJoinedClan - 1} clans before that\n";

            embed
                .WithTitle($"{player.Faction} **member** {player.Username}")
                .WithDescription($"{clanString}" + $"{clansJoinedString}")
                .WithFields(GameTimeField, PvpField, PveField, OtherField);

            return embed;
        }
        internal static string GetFactionImage(string faction)
        {
            string factionImage = $"..\\..\\..\\Images\\{faction.ToLower()}.webp";
            return factionImage;
        }
        internal static EmbedBuilder CreateStatsErrorMessage(SocketSlashCommand command)
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red)
                .WithTitle($"Couldn't find player {command.Data.Options.First().Value.ToString}");
            return embed;
        }
    }
}
