using Discord;
using Discord.WebSocket;
using FOFA_Bot.Bot;
using Newtonsoft.Json.Linq;

namespace FOFA_Bot.Data
{
    internal class BotData
    {
        private static dynamic JsonBotData = "";
        internal static void LoadJson()
        {
            Logger.LogInformation($"Getting json data");
            using StreamReader reader = new("..\\..\\..\\Data\\Data.json");
            string json = reader.ReadToEnd();
            JsonBotData = JObject.Parse(json);
            Logger.LogInformation($"Read json data");
        }

        internal static string GetDiscordToken()
        {
            Logger.LogInformation($"Getting Discord Token");
            return JsonBotData.DiscordToken;
        }
        internal static SocketGuild GetGuild()
        {
            ulong guildId = JsonBotData.GuildId;
            SocketGuild? guild = BotHandler.GetDiscord().GetGuild(guildId);
            return guild;
        }

        internal static IMessageChannel? GetSignupsChannel(string clan)
        {
            Logger.LogInformation($"    Getting {clan} Signups Channel");
            if (clan == "ROFA") return (IMessageChannel)GetGuild().GetChannel((ulong)JsonBotData.ROFASignupsChannelId);
            else if (clan == "BOFA") return (IMessageChannel)GetGuild().GetChannel((ulong)JsonBotData.BOFASignupsChannelId);
            return null;
        }
        internal static IMessageChannel? GetAnnouncementChannel(string clan)
        {
            Logger.LogInformation($"    Getting {clan} Announcement Channel");
            ulong channelId = 1;
            if (clan == "ROFA") channelId = (ulong)JsonBotData.ROFAAnnouncementChannelId;
            else if (clan == "BOFA") channelId = (ulong)JsonBotData.BOFAAnnouncementChannelId;
            if (channelId != 1) return (IMessageChannel)GetGuild().GetChannel(channelId);
            return null;
        }

        internal static ulong GetClanWarChannelId(string clan)
        {
            Logger.LogInformation($"    Getting {clan} Announcement Channel Id");
            if (clan == "ROFA") return JsonBotData.ROFAClanWarChannelId;
            else if (clan == "BOFA") return JsonBotData.BOFAClanWarChannelId;
            return 1;
        }
        internal static ulong GetStatsChannelId()
        {
            return JsonBotData.StatsChannelId;
        }
        internal static string? GetClanRoleName(string clan)
        {
            if (clan == "ROFA") return JsonBotData.ROFARoleName;
            else if (clan == "BOFA") return JsonBotData.BOFARoleName;
            return null;
        }
        internal static string[]? GetPrivilegedRoleNames(string clan)
        {
            if (clan == "ROFA")
                return JsonBotData.PrivilegedRoleNamesRofa.ToObject<string[]>();
            else if (clan == "BOFA")
                return JsonBotData.PrivilegedRoleNamesBofa.ToObject<string[]>();
            return null;
        }

        internal static string GetPlannerSheetId()
        {
            Logger.LogInformation($"    Getting Planner Sheet Id");
            return JsonBotData.PlannerSheetId;
        }

        internal static string GetExboClientID()
        {
            Logger.LogInformation($"    Getting Exbo Client ID");
            return JsonBotData.ExboClientID;
        }
        internal static string GetExboClientSecret()
        {
            Logger.LogInformation($"    Getting Exbo Client Secret");
            return JsonBotData.ExboClientSecret;
        }
    }
}
