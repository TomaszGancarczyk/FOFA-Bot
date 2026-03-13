using Discord;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceBackup
    {
        private static JsonSerializerOptions Options = new JsonSerializerOptions { IncludeFields = true, ReferenceHandler = ReferenceHandler.Preserve };
        internal static void SaveBuckup(AttendanceMessage message)
        {
            Logger.LogInformation($"    Saving backup message");
            string jsonMessage = "";
            AttendanceMessageBackup backupMessage = new()
            {
                Date = message.Date,
                discordMessageId = message.discordMessage.Id,
                Reminder = message.Reminder
            };
            try
            {
                jsonMessage = JsonSerializer.Serialize(backupMessage, Options);
            }
            catch (Exception e)
            {
                Logger.LogError($"    Run into error when serializing class:\n{e}");
                return;
            }
            File.WriteAllText("..\\..\\..\\Data\\BackupMessage.json", jsonMessage);
            Logger.LogInformation($"    Backup message saved");
        }
        internal static async Task ReadBackup()
        {
            string json = "";
            try
            {
                using StreamReader reader = new("..\\..\\..\\Data\\BackupMessage.json");
                json = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                Logger.LogError($"    Couldnt find the backup message\n{e}");
                return;
            }
            AttendanceMessageBackup? backupMessage = null;
            try
            {
                backupMessage = JsonSerializer.Deserialize<AttendanceMessageBackup?>(json, Options);
                Logger.LogInformation($"    Successfully read backup message");
            }
            catch (Exception e)
            {
                Logger.LogError($"    Couldnt read the backup message:\n{e}");
                return;
            }
            if (backupMessage == null || backupMessage.discordMessageId == null || backupMessage.Date < DateTime.Now || await AttendanceHandler.CheckIfMessageIsDeleted(backupMessage.discordMessageId))
            {
                Logger.LogWarning($"    Message is incorrect. dropping");
                return;
            }
            Logger.LogInformation($"[backup message] Backup message is correct, handling");
            await HandleMessage(backupMessage);
        }
        private static async Task HandleMessage(AttendanceMessageBackup backupMessage)
        {
            BotHandler.SetSignupMessageRunning(true);
            IMessageChannel channel = BotData.GetSignupsChannel();
            AttendanceMessage message = await ConvertToAttendanceMessage(backupMessage);
            //TODO handle
            BotHandler.SetSignupMessageRunning(false);
        }
        private static async Task<AttendanceMessage> ConvertToAttendanceMessage(AttendanceMessageBackup backupMessage)
        {
            Logger.LogInformation($"    Converting backup message into attendance message");
            IMessageChannel channel = BotData.GetSignupsChannel();
            IMessage discordMessage = await channel.GetMessageAsync(backupMessage.discordMessageId);
            AttendanceMessage response = new()
            {
                //TODO embed message
                embedMessage = null,
                messageButtons = null,
                Date = backupMessage.Date,
                discordMessage = discordMessage,
                Reminder = backupMessage.Reminder
            };
            response = AttendanceMessageGenerator.AddMessageButtons(response);
            Logger.LogInformation($"    Message converted");
            return response;
        }
    }
}
