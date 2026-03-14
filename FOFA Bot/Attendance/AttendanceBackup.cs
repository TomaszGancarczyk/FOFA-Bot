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
                Logger.LogWarning($"    Message is incorrect. dropping");
                return;
            }
            Logger.LogInformation($"[backup message] Backup message is correct, handling");
            await HandleMessage(backupMessage);
        }
        private static async Task HandleMessage(AttendanceMessageBackup backupMessage)
        {
            BotHandler.SetSignupMessageRunning(true);
            await ConvertToAttendanceMessage(backupMessage);
            await AttendanceHandler.HandleMessageRunning(backupMessage.DiscordMessageId);
            BotHandler.SetSignupMessageRunning(false);
        }

        private static async Task<AttendanceMessage> ConvertToAttendanceMessage(AttendanceMessageBackup backupMessage)
        {
            Logger.LogInformation($"    Getting discord message from backup");
            IMessageChannel channel = BotData.GetSignupsChannel();
            IMessage discordMessage = await channel.GetMessageAsync(backupMessage.DiscordMessageId);
            Logger.LogInformation($"    Getting event title");
            string eventName = string.Join(" ", discordMessage.Embeds.First().Title.Split(" ").Skip(1));
            Logger.LogInformation($"    Converting members from backup");
            Logger.LogInformation($"    Creating attendance message");
            AttendanceMessage response = AttendanceMessageGenerator.CreateCustomAttendanceMessage(eventName, backupMessage.Date);
            response.Reminder = backupMessage.Reminder;
            response.DiscordMessage = discordMessage;
            MemberHandler.UpdateBackupMembers(backupMessage.Members);
            AttendanceHandler.UpdateBackupAttendanceMessage(response);
            Logger.LogInformation($"    Message converted");
            return response;
        }
    }
}
