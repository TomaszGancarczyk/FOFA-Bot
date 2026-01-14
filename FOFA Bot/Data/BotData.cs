using Discord;
using Discord.WebSocket;
using FOFA_Bot.Bot;
using Newtonsoft.Json.Linq;

namespace FOFA_Bot.Data
{
    internal class BotData
    {
        private static dynamic JsonBotData;
        internal async static Task LoadJson()
        {
            Logger.LogInformation($"Getting json data");
            using (StreamReader reader = new StreamReader("..\\..\\..\\Data\\Data.json"))
            {
                string json = reader.ReadToEnd();
                JsonBotData = JObject.Parse(json);
                Logger.LogInformation($"Read json data");
            }
        }

        internal static string GetDiscordToken()
        {
            Logger.LogInformation($"Getting Discord Token");
            return JsonBotData.DiscordToken;
        }
        internal static SocketGuild GetGuild()
        {
            Logger.LogInformation($"Getting Guild Id");
            ulong guildId = JsonBotData.GuildId;
            Logger.LogInformation($"Getting Guild");
            SocketGuild guild = BotHandler.GetDiscord().GetGuild(guildId);
            Logger.LogInformation($"Found Guild: {guild.Name}");
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

        internal static int GetTournamentHour()
        {
            Logger.LogInformation($"Getting Tournament Hour");
            return JsonBotData.TournamentHour;
        }
        internal static int GetBrawlHour()
        {
            Logger.LogInformation($"Getting Brawl Hour");
            return JsonBotData.BrawlHour;
        }
        internal static int GetBaseCaptureHour()
        {
            Logger.LogInformation($"Getting Base Capture Hour");
            return JsonBotData.BaseCaptureHour;
        }
        internal static int GetGoldenDropHour()
        {
            Logger.LogInformation($"Getting Golden Drop Hour");
            return JsonBotData.GoldenDropHour;
        }

        internal static string GetRofaRoleName()
        {
            Logger.LogInformation($"Getting Rofa Role Name");
            return JsonBotData.RofaRoleName;
        }
    }
}
