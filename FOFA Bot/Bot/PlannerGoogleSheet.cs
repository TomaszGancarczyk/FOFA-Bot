using FOFA_Bot.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Util.Store;

namespace FOFA_Bot.Bot
{
    internal class PlannerGoogleSheet
    {
        internal static List<PlannerData> GetPlannerData()
        {
            List<PlannerData> data = [];
            string sheetId = BotData.GetPlannerSheetId();
            SheetsService? service = GetSheetService();
            string range = "A2:C";
            var request = service.Spreadsheets.Values.Get(sheetId, range);
            var requestResponse = request.Execute().Values;
            foreach (var value in requestResponse) if (value != null && value[0] != null && value[1] != null)
                {
                    PlannerData newData = new(value[0].ToString(), value[1].ToString());
                    try
                    {
                        if (value[2] != null && value[2].ToString() != string.Empty)
                        {
                            newData.priority = Int32.Parse(value[2].ToString());
                        }
                    }
                    catch (Exception) { newData.priority = 0; }
                    data.Add(newData);
                }
            return data;
        }
        private static SheetsService? GetSheetService()
        {
            Logger.LogInformation($"    Getting signup sheet service");
            string clientId = BotData.GetSheetClientId();
            string clientSecret = BotData.GetSheetClientSecret();
            string[] scopes = [SheetsService.Scope.Spreadsheets];
            UserCredential? credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                new ClientSecrets
                {
                    ClientId = clientId,
                    ClientSecret = clientSecret
                },
                scopes,
                "FOFA Bot",
                CancellationToken.None,
                new FileDataStore("GoogleToken"))
                .Result;
            SheetsService? service = new(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "FOFA Bot"
            });
            return service;
        }
    }
}
