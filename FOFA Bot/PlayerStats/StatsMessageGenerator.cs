using Discord;
using Discord.WebSocket;


namespace FOFA_Bot.PlayerStats
{
    internal class StatsMessageGenerator
    {
        internal static EmbedBuilder CreateStatsMessage(PlayerStats stats)
        {
            string faction = "";
            EmbedBuilder embed = new();
            switch (stats.alliance)
            {
                case "freedom":
                    embed.WithColor(Color.Green);
                    faction = "Rise";
                    break;
                case "duty":
                    embed.WithColor(Color.Red);
                    faction = "Frontier";
                    break;
                case "covenant":
                    embed.WithColor(Color.Purple);
                    break;
                case "mercenaries":
                    embed.WithColor(Color.Blue);
                    break;
                case "stalkers":
                    embed.WithColor(Color.Gold);
                    break;
                case "bandits":
                    embed.WithColor(Color.LightGrey);
                    break;
            }
            string clanString = "";
            string clansJoinedString = "";
            long playtime = ((long)stats.stats.First(stat => stat.id == "pla-tim").value);
            int playtimeHours = (int)(playtime / (1000 * 60 * 60));
            EmbedFieldBuilder GameTimeField = new()
            {
                Name = $"Game Time",
                Value = $"- {playtimeHours} hours since {DateOnly.FromDateTime((DateTime)stats.stats.First(stat => stat.id == "reg-tim").value)}\n" +
                $"- Last login: {stats.lastLogin}",
                IsInline = false,
            };
            EmbedFieldBuilder PveField = new()
            {
                Name = $"PVE",
                Value = $"- NPC Kills: {stats.stats.First(stat => stat.id == "npc-kil").value.ToString()}\n" +
                $"- MutantKills: {stats.stats.First(stat => stat.id == "mut-kil").value.ToString()}",
                IsInline = false,
            };
            long kills = (long)stats.stats.First(stat => stat.id == "kil").value;
            long deaths = (long)stats.stats.First(stat => stat.id == "dea").value;
            double KD = (double)kills / (double)deaths;
            long sessionKills = (long)stats.stats.First(stat => stat.id == "kills-bf").value;
            long sessionDeaths = (long)stats.stats.First(stat => stat.id == "deaths-bf").value;
            double sessionKD = (double)sessionKills / (double)sessionDeaths;
            EmbedFieldBuilder PvpField = new()
            {
                Name = $"PVP",
                Value = $"- Kills: {kills}\n" +
                $"- Deaths: {deaths}\n" +
                $"- Assists: {stats.stats.First(stat => stat.id == "ast").value.ToString()}\n" +
                $"- Suicides: {stats.stats.First(stat => stat.id == "suicides").value.ToString()}\n" +
                $"- Total K/D: {Math.Round(KD, 2)}\n" +
                $"- Session Kills: {sessionKills}\n" +
                $"- Session Deaths: {sessionDeaths}\n" +
                $"- Session K/D: {Math.Round(sessionKD, 2)}",
                IsInline = false,
            };
            EmbedFieldBuilder OtherField = new()
            {
                Name = $"Other",
                Value = $"- Highest Money: {stats.stats.First(stat => stat.id == "max-mon-amo").value}\n" +
                $"- Artifacts Found: {stats.stats.First(stat => stat.id == "art-col").value.ToString()}\n" +
                $"- Bolts Thrown: {stats.stats.First(stat => stat.id == "scr-thr").value.ToString()}\n" +
                $"- Deliveries Made: {stats.stats.First(stat => stat.id == "tpacks-delivered").value.ToString()}\n" +
                $"- Caches Found: {stats.stats.First(stat => stat.id == "mining-count").value.ToString()}\n" +
                $"- Signals Found: {stats.stats.First(stat => stat.id == "sgn-fnd").value.ToString()}",
                IsInline = false,
            };

            if (stats.clan != null)
                clanString = $"**{stats.clan.member.rank} of [{stats.clan.info.tag}] {stats.clan.info.name}**\n";
            //if (stats.TimesJoinedClan > 0)
            //    clansJoinedString = $"and member of {stats.TimesJoinedClan - 1} clans before that\n";

            embed
                .WithTitle($"{faction} **member** {stats.username}")
                .WithDescription($"{clanString}" + $"{clansJoinedString}")
                .WithFields(GameTimeField, PvpField, PveField, OtherField)
                .WithImageUrl($"attachment://{stats.alliance.ToLower()}.webp");

            return embed;
        }
        internal static EmbedBuilder CreateStatsErrorMessage(SocketSlashCommand command)
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red)
                .WithTitle($"Couldn't find player {command.Data.Options.First().Value.ToString()}");
            return embed;
        }
    }
}

//TODO message
// K/D/A maybe
// inline fields
// operations field
// fix image placement
// check faction names