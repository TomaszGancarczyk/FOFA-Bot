using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace FOFA_Bot.Attendance
{
    internal static class AttendanceGoogleSheetHelpers
    {

        private static void UpdateGoogleSheet(IList<IList<Object>> objRecords, string sheetId, string range, SheetsService service)
        {
            Logger.LogInformation($"Updating signup google sheet");
            var request = service.Spreadsheets.Values.Append(new ValueRange() { Values = objRecords }, sheetId, range);
            request.InsertDataOption = SpreadsheetsResource.ValuesResource.AppendRequest.InsertDataOptionEnum.INSERTROWS;
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.AppendRequest.ValueInputOptionEnum.RAW;
            request.Execute();
        }
    }
}