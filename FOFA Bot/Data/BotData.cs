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
        internal static IMessageChannel GetROFAQuestionChannel()
        {
            Logger.LogInformation($"    Getting Question Channel Id");
            ulong channelId = JsonBotData.ROFAQuestionChannelId;
            Logger.LogInformation($"    Getting Question Channel");
            IMessageChannel channel = (IMessageChannel)GetGuild().GetChannel(channelId);
            Logger.LogInformation($"    Found Question Channel: {channel.Name}");
            return channel;
        }
        internal static IMessageChannel GetROFASignupsChannel()
        {
            Logger.LogInformation($"    Getting Signups Channel Id");
            ulong channelId = JsonBotData.ROFASignupsChannelId;
            Logger.LogInformation($"    Getting Signups Channel");
            IMessageChannel channel = (IMessageChannel)GetGuild().GetChannel(channelId);
            Logger.LogInformation($"    Found Signups Channel: {channel.Name}");
            return channel;
        }
        internal static IMessageChannel GetROFAAnnouncementChannel()
        {
            Logger.LogInformation($"    Getting Announcement Channel Id");
            ulong channelId = JsonBotData.ROFAAnnouncementChannelId;
            Logger.LogInformation($"    Getting Announcement Channel");
            IMessageChannel channel = (IMessageChannel)GetGuild().GetChannel(channelId);
            Logger.LogInformation($"    Found Announcement Channel: {channel.Name}");
            return channel;
        }
        internal static ulong GetROFAClanWarChannelId()
        {
            Logger.LogInformation($"    Getting Announcement Channel Id");
            ulong channelId = JsonBotData.ROFAClanWarChannelId;
            return channelId;
        }
        internal static ulong GetStatsChannelId()
        {
            return JsonBotData.StatsChannelId;
        }

        internal static double GetTournamentHour()
        {
            Logger.LogInformation($"    Getting Tournament Hour");
            return JsonBotData.TournamentHour;
        }
        internal static double GetBrawlHour()
        {
            Logger.LogInformation($"    Getting Brawl Hour");
            return JsonBotData.BrawlHour;
        }
        internal static double GetBaseCaptureHour()
        {
            Logger.LogInformation($"    Getting Base Capture Hour");
            return JsonBotData.BaseCaptureHour;
        }

        internal static double GetGoldenDropHour()
        {
            Logger.LogInformation($"    Getting Golden Drop Hour");
            return JsonBotData.GoldenDropHour;
        }
        internal static double GetStillwatersHour()
        {
            Logger.LogInformation($"    Getting Stillwaters Hour");
            return JsonBotData.StillwatersHour;
        }
        internal static string GetROFARoleName()
        {
            return JsonBotData.ROFARoleName;
        }
        internal static string[] GetPrivilegedRoleNames()
        {
            return JsonBotData.PrivilegedRoleNames.ToObject<string[]>();
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
