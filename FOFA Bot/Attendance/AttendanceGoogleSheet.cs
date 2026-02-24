using FOFA_Bot.Bot;
using FOFA_Bot.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceGoogleSheet
    {
        internal static void HandleUnsignedUsers(List<Member> members)
        {
            Logger.LogInformation($"Exporting unassigned users");
            string sheetId = BotData.GetSignupSheetId();
            SheetsService? service = GetSheetService();
            string range = GetRange(members.Count + 1);
            List<string> userNames = [.. members.Select(m => m.discordUser.Username)];
            IList<IList<Object>> objRecords = GenerateData(userNames);
            if (service != null)
                UpdateGoogleSheet(objRecords, sheetId, range, service);
            Logger.LogInformation($"Finished updating attendance sheet");
        }

        private static void UpdateGoogleSheet(IList<IList<Object>> objRecords, string sheetId, string range, SheetsService service)
        {
            Logger.LogInformation($"Updating signup google sheet...");
            var request = service.Spreadsheets.Values.Append(new ValueRange() { Values = objRecords }, sheetId, range);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            Logger.LogInformation($"Executing signup google sheet update...");
            try
            {
                request.Execute();
            }
            catch(Exception e)
            {
                Logger.LogCritical($"Run into error when execuring sheet update:\n{e}");
            }
            Logger.LogInformation($"Signup google sheet updated");
        }

        private static List<IList<object>> GenerateData(List<string> userNames)
        {
            Logger.LogInformation($"Generating data for the signup sheet");
            List<IList<Object>> fullObject = [];
            IList<Object> objectLine = [];
            objectLine.Add(DateTime.Now.ToString("dd/MM/yyyy"));
            foreach (string userName in userNames)
                objectLine.Add(userName);
            fullObject.Add(objectLine);
            return fullObject;
        }

        private static SheetsService? GetSheetService()
        {
            Logger.LogInformation($"Getting signup sheet service");
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
        private static string GetRange(int numberOfCollumns)
        {
            char lastCollumnChar = (char)('A' + numberOfCollumns);
            string range = $"A3:{lastCollumnChar}3";
            Logger.LogInformation($"Got sheet range for {range}");
            return range;
        }
    }
}