using Discord;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceBackup
    {
        private static readonly JsonSerializerOptions Options = new() { IncludeFields = true, ReferenceHandler = ReferenceHandler.Preserve };
        internal static void SaveBuckup(AttendanceMessage message)
        {
            Logger.LogInformation($"    Saving backup message");
            string jsonMessage;
            Dictionary<ulong, bool?> members = [];
            foreach (Member member in MemberHandler.GetMembers())
            {
                members.Add(member.discordUser.Id, member.status);
            }

            AttendanceMessageBackup backupMessage = new()
            {
                Date = message.Date,
                DiscordMessageId = message.DiscordMessage.Id,
                Reminder = message.Reminder,
                Members = members
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
        internal static async Task ReadBackup()
        {
            foreach (string filePath in Directory.GetFiles("..\\..\\..\\Data\\BackupMessages"))
            {
                Console.WriteLine(filePath);
                string json = "";
                try
                {
                    using StreamReader reader = new(filePath);
                    json = reader.ReadToEnd();
                }
                catch (Exception e)
                {
                    Logger.LogError($"    Couldnt find the backup message\n{e}");
                    return;
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
                    return;
                }
                if (backupMessage == null || backupMessage.Date < DateTime.Now || await AttendanceHandler.CheckIfMessageIsDeleted(backupMessage.DiscordMessageId))
                {
                    Logger.LogWarning($"    Message is incorrect. Deleting and dropping");
                    File.Delete(filePath);
                    return;
                }
                Logger.LogInformation($"[backup message] Backup message is correct, handling");
                _ = HandleMessage(backupMessage);
            }
        }
        private static async Task HandleMessage(AttendanceMessageBackup backupMessage)
        {
            BotHandler.AddSignupMessageRunning();
            await ConvertToAttendanceMessage(backupMessage);
            AttendanceHandler.RefreshSignupMessage(backupMessage.DiscordMessageId);
            await AttendanceHandler.HandleMessageRunning(backupMessage.DiscordMessageId);
            BotHandler.RemoveSignupMessageRunning();
        }

        private static async Task<AttendanceMessage?> ConvertToAttendanceMessage(AttendanceMessageBackup backupMessage)
        {
            Logger.LogInformation($"    Getting discord message from backup");
            IMessageChannel channel = BotData.GetSignupsChannel();
            IMessage discordMessage = await channel.GetMessageAsync(backupMessage.DiscordMessageId);
            Logger.LogInformation($"    Getting event title");
            string eventName = string.Join(" ", discordMessage.Embeds.First().Title.Split(" ").Skip(1));
            Logger.LogInformation($"    Creating attendance message");
            AttendanceMessage? response = AttendanceMessageGenerator.CreateCustomAttendanceMessage(eventName, backupMessage.Date);
            if (response == null) return null;
            response.Reminder = backupMessage.Reminder;
            response.DiscordMessage = discordMessage;
            Logger.LogInformation($"    Converting members from backup");
            MemberHandler.UpdateBackupMembers(backupMessage.Members);
            AttendanceHandler.UpdateBackupAttendanceMessage(response);
            Logger.LogInformation($"    Message converted");
            return response;
        }
    }
}
