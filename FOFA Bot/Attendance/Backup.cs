using Discord;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FOFA_Bot.Attendance
{
    internal class Backup
    {
        private static readonly JsonSerializerOptions Options = new() { IncludeFields = true, ReferenceHandler = ReferenceHandler.Preserve };
        internal static void SaveBuckup(Message message)
        {
            Logger.LogInformation($"    Saving backup message");
            string jsonMessage;
            Dictionary<ulong, bool?[]> members = [];
            foreach (Member member in MemberHandler.GetMembers(message.Clan))
            {
                members.Add(member.discordUser.Id, member.status);
            }

            AttendanceMessageBackup backupMessage = new()
            {
                ThursdayDate = message.ThursdayDate,
                DiscordMessageId = message.DiscordMessage.Id,
                Members = members,
                Clan = message.Clan
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
            File.WriteAllText($"..\\..\\..\\Data\\BackupMessages\\BackupMessage_{backupMessage.DiscordMessageId}", jsonMessage);
            Logger.LogInformation($"    Backup message saved");
        }
        internal static void DeleteBackup(string messageId)
        {
            File.Delete($"..\\..\\..\\Data\\BackupMessages\\BackupMessage_{messageId}");
            Logger.LogInformation($"    Backup message deleted");
        }
        internal static async Task ReadBackup()
        {
            foreach (string filePath in Directory.GetFiles("..\\..\\..\\Data\\BackupMessages"))
            {
                string json = "";
                try
                {
                    using StreamReader reader = new(filePath);
                    json = reader.ReadToEnd();
                }
                catch (Exception e)
                {
                    Logger.LogError($"    Couldnt find the backup message\n{e}");
                    continue;
                }
                AttendanceMessageBackup? backupMessage;
                try
                {
                    backupMessage = JsonSerializer.Deserialize<AttendanceMessageBackup?>(json, Options);
                    Logger.LogInformation($"    Successfully read backup message");
                }
                catch (Exception e)
                {
                    Logger.LogError($"    Couldnt read the backup message:\n{e}");
                    continue;
                }
                if (backupMessage == null || backupMessage.ThursdayDate < DateTime.Now.AddDays(-3) || await AttendanceHandler.CheckIfMessageIsDeleted(backupMessage.DiscordMessageId, backupMessage.Clan))
                {
                    Logger.LogWarning($"    Message is incorrect. Deleting and dropping");
                    File.Delete(filePath);
                    continue;
                }
                Logger.LogInformation($"[backup message] Backup message is correct, handling");
                _ = HandleMessage(backupMessage);
            }
        }
        private static async Task HandleMessage(AttendanceMessageBackup backupMessage)
        {
            await ConvertToAttendanceMessage(backupMessage);
            AttendanceHandler.RefreshSignupMessage(backupMessage.DiscordMessageId, backupMessage.Clan);
            BotHandler.ChangeSignupMessageRunning(1, backupMessage.Clan);
            await AttendanceHandler.HandleMessageRunning(backupMessage.DiscordMessageId, backupMessage.Clan);
        }

        private static async Task ConvertToAttendanceMessage(AttendanceMessageBackup backupMessage)
        {
            Logger.LogInformation($"    Getting discord message from backup");
            IMessage discordMessage = await BotData.GetROFASignupsChannel().GetMessageAsync(backupMessage.DiscordMessageId);
            if (discordMessage == null) discordMessage = await BotData.GetBOFASignupsChannel().GetMessageAsync(backupMessage.DiscordMessageId);
            Logger.LogInformation($"    Creating attendance message");
            Message? response = MessageGenerator.CreateAttendanceMessage(backupMessage.Clan);
            response.ThursdayDate = backupMessage.ThursdayDate;
            if (response == null) return;
            response.DiscordMessage = discordMessage;
            Logger.LogInformation($"    Converting members from backup");
            MemberHandler.UpdateBackupMembers(backupMessage.Members, backupMessage.Clan);
            AttendanceHandler.UpdateBackupAttendanceMessage(response);
            Logger.LogInformation($"    Message converted");
            return;
        }
    }
}
