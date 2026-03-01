using Newtonsoft.Json;
using System.Net.Http.Headers;


namespace FOFA_Bot.PlayerStats
{
    internal class PlayerApiHandler
    {
        internal static async Task<PlayerStats?> GetPlayerStats(string playerName)
        {
            Logger.LogInformation($"    Calling API for {playerName}");
            var apitoken = await APIAccessToken.GetApiAcessToken();

            HttpClient client = new()
            {
                BaseAddress = new Uri("https://eapi.stalcraft.net/eu/character/by-name/")
            };
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", apitoken.AccessToken);
            client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpResponseMessage response = client.GetAsync($"{playerName}/profile").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Logger.LogInformation($"    Successfully got the request for {playerName}");
                string responseBody = await response.Content.ReadAsStringAsync();
                PlayerStats? playerStats = ConvertJsonStringToPlayerStats(responseBody);
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
        private static PlayerStats? ConvertJsonStringToPlayerStats(string jsonString)
        {
            try
            {
                PlayerStats stats = JsonConvert.DeserializeObject<PlayerStats>(jsonString);
                return stats;
            }
            catch (Exception e)
            {
                Logger.LogError($"Run into problem deserializing player stats:\n{e}");
                return null;
            }
        }
    }
}