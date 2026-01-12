using Newtonsoft.Json.Linq;

namespace FOFA_Bot
{
    internal class BotData
    {
        private static dynamic JsonBotData;
        internal static void LoadJson()
        {
            Logger.LogInformation($"Getting json data");
            using StreamReader reader = new("..\\..\\..\\Data.json");
            string json = reader.ReadToEnd();
            JsonBotData = JObject.Parse(json);
            Logger.LogInformation($"Read json data");
        }


        internal static string GetDiscordToken() {
            Logger.LogInformation($"Getting Discord Token");
            return JsonBotData.DiscordToken;
        }
        internal static ulong GetGuildId()
        {
            Logger.LogInformation($"Getting Guild Id");
            return JsonBotData.GuildId;
        }
        internal static ulong GetQuestionChannelId()
        {
            Logger.LogInformation($"Getting Question Channel Id");
            return JsonBotData.QuestionChannelId;
        }
        internal static ulong GetSignupsChannelId()
        {
            Logger.LogInformation($"Getting Signups Channel Id");
            return JsonBotData.SignupsChannelId;
        }
        internal static ulong GetNadeChannelId()
        {
            Logger.LogInformation($"Getting Nade Channel Id");
            return JsonBotData.NadeChannelId;
        }
    }
}
