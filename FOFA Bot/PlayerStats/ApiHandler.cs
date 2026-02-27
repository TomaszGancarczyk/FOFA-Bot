using FOFA_Bot.Data;
using Newtonsoft.Json;
using System.Net.Http.Headers;


namespace FOFA_Bot.PlayerStats
{
    internal class ApiHandler
    {
        internal static async Task<Stats?> GetPlayerStats(string playerName)
        {
            Logger.LogInformation($"    Calling API for {playerName}");
            string apitoken = BotData.GetApiToken();

            HttpClient client = new()
            {
                BaseAddress = new Uri("https://eapi.stalcraft.net/eu/character/by-name/")
            };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apitoken);
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync($"{playerName}/profile").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Logger.LogInformation($"    Successfully got the request for {playerName}");
                string responseBody = await response.Content.ReadAsStringAsync();
                Stats playerStats = await ConvertJsonStringToPlayerStats(responseBody);
                return playerStats;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Logger.LogCritical($"Unauthorized API response");
                return null;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                Logger.LogWarning($"Couldnt find player {playerName}");
                return null;
            }
            else
            {
                Logger.LogWarning($"Unsupported error in API call: {response.StatusCode}");
                return null;
            }
        }
        private static async Task<Stats> ConvertJsonStringToPlayerStats(string jsonString)
        {
            dynamic? dynamicStats = JsonConvert.DeserializeObject(jsonString);
            //Stats stats = new()
            //{
            //    Uuid = dynamicStats.uuid,
            //    Username = dynamicStats.username
            //};
            //return stats;
            return null;
        }
    }
}