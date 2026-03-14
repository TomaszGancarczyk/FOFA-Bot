using Discord;
using FOFA_Bot.Data;
using Newtonsoft.Json;

namespace FOFA_Bot.PlayerStats
{
    internal class APIAccessToken
    {
        private static ApiToken? Token;
        private static DateTime TokenExpirationTime = DateTime.Now;
        internal static async Task<ApiToken?> GetApiAcessToken()
        {
            Logger.LogInformation($"    Getting API Access token");
            if (Token != null && TokenExpirationTime > DateTime.Now)
            {
                Logger.LogInformation($"    Token expires in {TokenExpirationTime - DateTime.Now}");
                return Token;
            }
            Logger.LogInformation($"    Token Expired, creating new API Access token");
            string clientId = BotData.GetExboClientID();
            string clientSecret = BotData.GetExboClientSecret();
            HttpClient client = new()
            {
                BaseAddress = new Uri("https://exbo.net/oauth/token")
            };
            var data = new[]
            {
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", "")
            };
            var content = new FormUrlEncodedContent(data);
            HttpResponseMessage response = await client.PostAsync("token", content);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                Logger.LogInformation($"    Successfully obtained API Access token");
                string jsonContent = response.Content.ReadAsStringAsync().Result;
                ApiToken? responseBody = JsonConvert.DeserializeObject<ApiToken>(jsonContent);
                if (responseBody == null) { return null; }
                TokenExpirationTime = DateTime.Now.AddMilliseconds(responseBody.ExpiresIn - 60000);
                Logger.LogInformation($"    Token expires in {TokenExpirationTime - DateTime.Now}");
                Token = responseBody;
                return Token;
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                Logger.LogCritical($"    Unauthorized API response");
                return null;
            }
            else
            {
                Logger.LogWarning($"    Unsupported error in API call: \n{response.StatusCode}");
                return null;
            }
        }
    }
    [JsonObject]
    internal class ApiToken
    {
        [JsonProperty("token_type")]
        internal string TokenType = "";
        [JsonProperty("expires_in")]
        internal double ExpiresIn = 0;
        [JsonProperty("access_token")]
        internal string AccessToken = "";
    }
}
