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
        internal static IMessageChannel GetROFASignupsChannel()
        {
            Logger.LogInformation($"    Getting ROFA Signups Channel");
            IMessageChannel channel = (IMessageChannel)GetGuild().GetChannel((ulong)JsonBotData.ROFASignupsChannelId);
            Logger.LogInformation($"    Found Signups Channel: {channel.Name}");
            return channel;
        }
        internal static IMessageChannel GetBOFASignupsChannel()
        {
            Logger.LogInformation($"    Getting BOFA Signups Channel");
            IMessageChannel channel = (IMessageChannel)GetGuild().GetChannel((ulong)JsonBotData.BOFASignupsChannelId);
            Logger.LogInformation($"    Found Signups Channel: {channel.Name}");
            return channel;
        }
        internal static IMessageChannel GetROFAAnnouncementChannel()
        {
            Logger.LogInformation($"    Getting ROFA Announcement Channel");
            IMessageChannel channel = (IMessageChannel)GetGuild().GetChannel(JsonBotData.ROFAAnnouncementChannelId);
            Logger.LogInformation($"    Found Announcement Channel: {channel.Name}");
            return channel;
        }
        internal static IMessageChannel GetBOFAAnnouncementChannel()
        {
            Logger.LogInformation($"    Getting BOFA Announcement Channel");
            IMessageChannel channel = (IMessageChannel)GetGuild().GetChannel(JsonBotData.BOFAAnnouncementChannelId);
            Logger.LogInformation($"    Found Announcement Channel: {channel.Name}");
            return channel;
        }
        internal static ulong GetROFAClanWarChannelId()
        {
            Logger.LogInformation($"    Getting ROFA Announcement Channel Id");
            ulong channelId = JsonBotData.ROFAClanWarChannelId;
            return channelId;
        }
        internal static ulong GetBOFAClanWarChannelId()
        {
            Logger.LogInformation($"    Getting BOFA Announcement Channel Id");
            ulong channelId = JsonBotData.BOFAClanWarChannelId;
            return channelId;
        }
        internal static ulong GetStatsChannelId()
        {
            return JsonBotData.StatsChannelId;
        }

        internal static string GetROFARoleName()
        {
            return JsonBotData.ROFARoleName;
        }
        internal static string GetBOFARoleName()
        {
            return JsonBotData.BOFARoleName;
        }
        internal static string[] GetPrivilegedRoleNames(string clan)
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
