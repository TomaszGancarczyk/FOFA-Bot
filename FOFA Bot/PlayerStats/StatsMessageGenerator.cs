using Discord;


namespace FOFA_Bot.PlayerStats
{
    internal class StatsMessageGenerator
    {
        private static PlayerStats? Stats;
        internal static EmbedBuilder CreateStatsMessage(PlayerStats stats)
        {
            Stats = stats;
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
                    faction = "Covenant";
                    break;
                case "merc":
                    embed.WithColor(Color.Blue);
                    faction = "Mercenary";
                    break;
                case "stalkers":
                    embed.WithColor(Color.Gold);
                    faction = "Stalker";
                    break;
                case "bandits":
                    embed.WithColor(Color.LightGrey);
                    faction = "Bandit";
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
            long kills = (long)stats.stats.First(stat => stat.id == "kil").value;
            long deaths = (long)stats.stats.First(stat => stat.id == "dea").value;
            double KD = (double)kills / (double)deaths;
            EmbedFieldBuilder PveField = new()
            {
                Name = $"Player",
                Value =
                $"- Kills: {kills}\n" +
                $"- Deaths: {deaths}\n" +
                $"- K/D: {Math.Round(KD, 2)}\n" +
                GetStatLineFromId("Assists", "ast") +
                GetStatLineFromId("Suicides", "suicides") +
                GetStatLineFromId("NPC Kills", "npc-kil") +
                GetStatLineFromId("Mutant Kills", "mut-kil"),
                IsInline = true,
            };
            EmbedFieldBuilder SessionsField = new()
            {
                Name = $"Sessions",
                IsInline = true

            };
            try
            {
                long sessionKills = (long)stats.stats.First(stat => stat.id == "kills-bf").value;
                long sessionDeaths = (long)stats.stats.First(stat => stat.id == "deaths-bf").value;
                double sessionKD = (double)sessionKills / (double)sessionDeaths;
                double sessionWinrate = Math.Round((double)(long)stats.stats.First(stat => stat.id == "won-bf").value / (double)(long)stats.stats.First(stat => stat.id == "part-bf").value * 100);

                SessionsField.Value =
                    $"- Kills: {sessionKills}\n" +
                    $"- Deaths: {sessionDeaths}\n" +
                    $"- K/D: {Math.Round(sessionKD, 2)}\n" +
                    GetStatLineFromId("Sessions Played", "part-bf") +
                    GetStatLineFromId("Won Sessions", "won-bf") +
                    GetStatLineFromId("Lost Sessions", "lost-bf") +
                    $"- Win %: {sessionWinrate}%";
            }
            catch (Exception e)
            {
                Logger.LogWarning($"    Run into problem when creating sessions field:\n{e}");
                SessionsField.Value =
                    $"Player has not played any Session Battles";
            }
            string highestMoney = stats.stats.First(stat => stat.id == "max-mon-amo").value.ToString();
            string formattedHighestMoney = highestMoney.Substring(0, highestMoney.Length % 3);
            for (int i = 0; i < (highestMoney.Length / 3); i++)
            {
                formattedHighestMoney += "." + highestMoney.Substring(highestMoney.Length % 3 + i * 3, 3);
            }
            if (highestMoney.Length % 3 == 0)
                formattedHighestMoney = formattedHighestMoney.Substring(1);

            EmbedFieldBuilder OtherField = new()
            {
                Name = $"Other",
                Value = $"- Highest Money: {formattedHighestMoney}\n" +
                GetStatLineFromId("Artifacts Found", "art-col") +
                GetStatLineFromId("Bolts Thrown", "scr-thr") +
                GetStatLineFromId("Deliveries Made", "tpacks-delivered") +
                GetStatLineFromId("Caches Found", "mining-count") +
                GetStatLineFromId("Signals Found", "sgn-fnd"),
                IsInline = true,
            };
            EmbedFieldBuilder OpsField = new()
            {
                Name = $"Operations",
                IsInline = true
            };
            try
            {
                OpsField.Value =
                    $"- Operations Played: {stats.stats.First(stat => stat.id == "completed-ops").value.ToString()}\n" +
                    $"- Kills: {stats.stats.First(stat => stat.id == "kills-ops").value.ToString()}\n" +
                GetStatLineFromId("Big Cleanup Completed", "big-cleanup-completed-ops") +
                GetStatLineFromId("Big Cleanup Highest", "big-cleanup-max-key-ops") +
                GetStatLineFromId("Focus Completed", "focus-completed-ops") +
                GetStatLineFromId("Focus Highest", "focus-max-key-ops");
            }
            catch (Exception e)
            {
                Logger.LogWarning($"    Run into problem when creating ops field:\n{e}");
                OpsField.Value =
                    $"Player has not played any operations";
            }
            EmbedFieldBuilder Breakfield = new()
            {
                Name = "-------------------------------------------------",
                Value = "-------------------------------------------------",
                IsInline = false
            };
            if (stats.clan != null)
                clanString = $"**{stats.clan.member.rank} of [{stats.clan.info.tag}] {stats.clan.info.name}**\n";
            //if (stats.TimesJoinedClan > 0)
            //    clansJoinedString = $"and member of {stats.TimesJoinedClan - 1} clans before that\n";

            embed
                .WithTitle($"{faction} **member** {stats.username}")
                .WithDescription($"{clanString}" + $"{clansJoinedString}")
                .WithFields(GameTimeField, Breakfield, PveField, SessionsField, Breakfield, OtherField, OpsField);

            return embed;
        }

        private static string GetStatLineFromId(string message, string id)
        {
            try
            {
                return $"- {message}: {Stats.stats.First(stat => stat.id == id).value.ToString()}\n";
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}

//TODO message
// K/D/A maybe
// check faction names
// add command