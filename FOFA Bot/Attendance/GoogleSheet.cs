using FOFA_Bot.Bot;
using FOFA_Bot.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace FOFA_Bot.Attendance
{
    internal class GoogleSheet
    {
        internal static void HandleUnsignedUsers(string eventName, List<Member> members)
        {
            Logger.LogInformation($"[excel] Exporting unassigned users");
            string sheetId = BotData.GetSignupSheetId();
            SheetsService? service = GetSheetService();
            string range = GetRange(members.Count + 1);
            List<string> userNames = [.. members.Select(m => m.discordUser.Username)];
            IList<IList<Object>> objRecords = GenerateData(eventName, userNames);
            if (service != null)
                UpdateGoogleSheet(objRecords, sheetId, range, service);
            Logger.LogInformation($"    Finished updating attendance sheet");
        }

        private static void UpdateGoogleSheet(IList<IList<Object>> objRecords, string sheetId, string range, SheetsService service)
        {
            Logger.LogInformation($"    Updating signup google sheet...");
            var request = service.Spreadsheets.Values.Append(new ValueRange() { Values = objRecords }, sheetId, range);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            Logger.LogInformation($"    Executing signup google sheet update...");
            try
            {
                request.Execute();
            }
            catch (Exception e)
            {
                Logger.LogCritical($"    Run into error when execuring sheet update:\n{e}");
            }
            Logger.LogInformation($"    Signup google sheet updated");
        }

        private static List<IList<object>> GenerateData(string eventName, List<string> userNames)
        {
            Logger.LogInformation($"    Generating data for the signup sheet");
            List<IList<Object>> fullObject = [];
            IList<Object> objectLine = [];
            objectLine.Add(DateTime.Now.ToString("dd/MM/yyyy"));
            objectLine.Add(eventName);
            foreach (string userName in userNames)
                objectLine.Add(userName);
            fullObject.Add(objectLine);
            return fullObject;
        }

        internal static SheetsService? GetSheetService()
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

        private static string GetRange(int numberOfCollumns)
        {
            string isBigChar = "";
            if (numberOfCollumns > 25)
            {
                isBigChar = "A";
                numberOfCollumns -= 25;
            }
            char lastCollumnChar = (char)('A' + numberOfCollumns);
            string range = $"A3:{isBigChar}{lastCollumnChar}3";
            Logger.LogInformation($"    Got sheet range for {range}");
            return range;
        }
    }
}