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
            SocketGuild guild = BotHandler.GetDiscord().GetGuild(guildId);
            return guild;
        }
        internal static IMessageChannel GetQuestionChannel()
        {
            Logger.LogInformation($"Getting Question Channel Id");
            ulong channelId = JsonBotData.QuestionChannelId;
            Logger.LogInformation($"Getting Question Channel");
            IMessageChannel channel = (IMessageChannel)GetGuild().GetChannel(channelId);
            Logger.LogInformation($"Found Question Channel: {channel.Name}");
            return channel;
        }
        internal static IMessageChannel GetSignupsChannel()
        {
            Logger.LogInformation($"Getting Signups Channel Id");
            ulong channelId = JsonBotData.SignupsChannelId;
            Logger.LogInformation($"Getting Signups Channel");
            IMessageChannel channel = (IMessageChannel)GetGuild().GetChannel(channelId);
            Logger.LogInformation($"Found Signups Channel: {channel.Name}");
            return channel;
        }
        internal static IMessageChannel GetNadeChannel()
        {
            Logger.LogInformation($"Getting Nade Channel Id");
            ulong channelId = JsonBotData.NadeChannelId;
            Logger.LogInformation($"Getting Nade Channel");
            IMessageChannel channel = (IMessageChannel)GetGuild().GetChannel(channelId);
            Logger.LogInformation($"Found Nade Channel: {channel.Name}");
            return channel;
        }

        internal static double GetTournamentHour()
        {
            Logger.LogInformation($"Getting Tournament Hour");
            return JsonBotData.TournamentHour;
        }
        internal static double GetBrawlHour()
        {
            Logger.LogInformation($"Getting Brawl Hour");
            return JsonBotData.BrawlHour;
        }
        internal static double GetBaseCaptureHour()
        {
            Logger.LogInformation($"Getting Base Capture Hour");
            return JsonBotData.BaseCaptureHour;
        }
        
        internal static double GetGoldenDropHour()
        {
            Logger.LogInformation($"Getting Golden Drop Hour");
            return JsonBotData.GoldenDropHour;
        }
        internal static double GetStillwatersHour()
        {
            Logger.LogInformation($"Getting Stillwaters Hour");
            return JsonBotData.StillwatersHour;
        }
        internal static string GetRofaRoleName()
        {
            return JsonBotData.RofaRoleName;
        }

        internal static string GetSheetClientId()
        {
            Logger.LogInformation($"Getting Sheet Client Id");
            return JsonBotData.SheetClientId;
        }
        internal static string GetSheetClientSecret()
        {
            Logger.LogInformation($"Getting Sheet Client Secret");
            return JsonBotData.SheetClientSecret;
        }
        internal static string GetSignupSheetId()
        {
            Logger.LogInformation($"Getting Signup Sheet Id");
            return JsonBotData.SignupSheetId;
        }
        internal static string GetNadeSheetId()
        {
            Logger.LogInformation($"Getting Nade Sheet Id");
            return JsonBotData.NadeSheetId;
        }

        internal static string GetApiToken()
        {
            Logger.LogInformation($"API Token");
            return JsonBotData.ApiToken;
        }
    }
}
