using FOFA_Bot.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;

namespace FOFA_Bot.Bot
{
    internal class PlannerGoogleSheet
    {
        internal static List<PlannerData> GetPlannerData()
        {
            List<PlannerData> data = [];
            string sheetId = BotData.GetPlannerSheetId();
            SheetsService? service = GetSheetService();
            string range = "A2:D";
            var request = service.Spreadsheets.Values.Get(sheetId, range);
            var requestResponse = request.Execute().Values;
            foreach (var value in requestResponse)
            {
                try
                {
                    PlannerData newData = new(value[0].ToString(), value[1].ToString())
                    {
                        priority = Int32.TryParse(value[2].ToString(), out int outValue) ? outValue : 0
                    };
                    try
                    {
                        if (value[3] != null && value[3].ToString() != string.Empty)
                        {
                            newData.squadleader = Convert.ToBoolean(value[3]);
                        }
                    }
                    catch (Exception) { newData.squadleader = false; }
                    data.Add(newData);
                }
                catch (Exception) { }
            }
            return data;
        }
        private static SheetsService? GetSheetService()
        {

            using var stream = new FileStream("..\\..\\..\\Data\\fofa-bot-cred.json", FileMode.Open, FileAccess.Read);
            var credential = ServiceAccountCredential.FromServiceAccountData(stream);
            credential.Scopes =
            [
                SheetsService.Scope.Spreadsheets,
                SheetsService.Scope.Drive
            ];
            SheetsService? service = new(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "fofa-bot"
            });
            return service;
        }
    }
}
