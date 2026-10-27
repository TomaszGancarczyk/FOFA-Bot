using Discord;
using Discord.WebSocket;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceHandler
    {
        private static List<Message?> CurrentMessages = [];
        private static readonly int EventReminderMinutes = 150;
        internal static void CreateAttendanceEvent()
        {
            Logger.LogInformation($"    Creating attendance event");
            BotHandler.ChangeSignupMessageRunning(1);
            Message? tempCurrentMessage = MessageGenerator.CreateAttendanceMessage();
            if (tempCurrentMessage == null)
            {
                Logger.LogError($"    Cloudn't create attendance message, returning");
                return;
            }
            Message? message = tempCurrentMessage;
            _ = SendAttendanceMessage(message);
            Logger.LogInformation($"    Attendance event finished");
            return;
        }
        internal static async Task SendAttendanceMessage(Message? currentMessage)
        {
            Logger.LogInformation($"    Sending attendance message to {currentMessage.SignupsChannel.Name}");
            ulong pingMessage = BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetROFARoleName()).Id;
            try
            {
                IMessage localCurrentMessage = await currentMessage.SignupsChannel.SendMessageAsync(
                    $"<@&{pingMessage}>"
                    , false, currentMessage.EmbedMessage.Build(), null, null, null, currentMessage.MessageButtons.Build());
                currentMessage.DiscordMessage = localCurrentMessage;
                Backup.SaveBuckup(currentMessage);
                CurrentMessages.Add(currentMessage);
                await HandleMessageRunning(localCurrentMessage.Id);
            }
            catch (Exception e)
            {
                Logger.LogError($"Error while handling attendance message:\n{e}");
            }
        }

        internal static async Task HandleMessageRunning(ulong messageId)
        {
            Message? currentMessage = CurrentMessages.First(m => m.DiscordMessage.Id == messageId);
            DateTime[] tempEventReminderTimes =
            [
                currentMessage.ThursdayDate.AddMinutes(-EventReminderMinutes),
                currentMessage.ThursdayDate.AddMinutes(-EventReminderMinutes).AddDays(1),
                currentMessage.ThursdayDate.AddMinutes(-EventReminderMinutes).AddDays(2),
                currentMessage.ThursdayDate.AddMinutes(-EventReminderMinutes).AddDays(3).AddHours(-1),
            ];
            DateTime[] eventReminderTimes = [];
            foreach (DateTime tempReminderTime in tempEventReminderTimes)
            {
                if (tempReminderTime > DateTime.Now) eventReminderTimes = [.. eventReminderTimes, tempReminderTime];
            }
            foreach (DateTime reminderTime in eventReminderTimes)
            {
                while (DateTime.Now < reminderTime)
                    Task.Delay(60000).Wait();
                bool isMessageDeleted = await CheckIfMessageIsDeleted(currentMessage.DiscordMessage.Id);
                if (CurrentMessages.Count > 0 && messageId == currentMessage.DiscordMessage.Id && SettingsHandler.GetAutomaticReminder() && !isMessageDeleted)
                {
                    string? reminderMessage = CreateReminderMessage();
                    if (reminderMessage != string.Empty)
                        await currentMessage.SignupsChannel.SendMessageAsync(reminderMessage);
                }
                while (DateTime.Now < reminderTime.AddHours(1))
                    Task.Delay(60000).Wait();
                isMessageDeleted = await CheckIfMessageIsDeleted(currentMessage.DiscordMessage.Id);
                if (CurrentMessages.Count > 0 && messageId == currentMessage.DiscordMessage.Id && SettingsHandler.GetAutomaticReminder() && !isMessageDeleted)
                {
                    string AnnouncementMessage = CreateAnnouncementMessage(messageId);
                    if (AnnouncementMessage != string.Empty && AnnouncementMessage != null)
                        try
                        {
                            await BotData.GetROFAAnnouncementChannel().SendMessageAsync(AnnouncementMessage);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError($"Error when sending announcement message:\n{e}");
                        }
                }
            }
            if (CurrentMessages.Count > 0 && currentMessage.DiscordMessage != null && messageId == currentMessage.DiscordMessage.Id)
                CurrentMessages.Remove(CurrentMessages.First(m => m.DiscordMessage.Id == messageId));
            BotHandler.ChangeSignupMessageRunning(-1);
        }

        internal static async Task<bool> CheckIfMessageIsDeleted(ulong messageId)
        {
            IMessageChannel channel = BotData.GetROFASignupsChannel();
            try
            {
                if (await channel.GetMessageAsync(messageId) == null)
                {
                    Logger.LogInformation($"    Message got deleted, skipping");
                    return true;
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"Error when getting message to check if it's deleted:\n{e}");
                return true;
            }
            return false;
        }

        internal static EmbedBuilder? RefreshSignupMessage(ulong? messageId)
        {
            Message? currentMessage = CurrentMessages.First(m => m.DiscordMessage.Id == messageId);
            if (CurrentMessages.Count <= 0) return null;
            Logger.LogInformation($"    Refreshing attendance message members");
            MemberHandler.RefreshMemberSquads();
            Logger.LogInformation($"    Refreshing attendance message fields");
            currentMessage.EmbedMessage = MessageGenerator.AddMessageFields(currentMessage.EmbedMessage);
            currentMessage.EmbedMessage = MessageGenerator.AddFooterMessage(currentMessage.EmbedMessage);
            Backup.SaveBuckup(currentMessage);
            return currentMessage.EmbedMessage;
        }

        private static string? CreateReminderMessage()
        {
            string reminderMessage = "## Don't forget to signup!";
            List<Member> members = MemberHandler.GetMembers();
            int nullmbmers = 0;
            foreach (var member in members) if (member.status == null && member.discordUser != null)
                {
                    reminderMessage += "\n" + MentionUtils.MentionUser(member.discordUser.Id);
                    nullmbmers++;
                }
            if (nullmbmers == 0) return null;
            return reminderMessage;
        }
        private static string CreateAnnouncementMessage(ulong? messageId)
        {
            Message? currentMessage = CurrentMessages.First(m => m.DiscordMessage.Id == messageId);
            ulong rofaRoleId = BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetROFARoleName()).Id;
            string[] eventParts = currentMessage.DiscordMessage.Embeds.First().Title.Split(" ");
            string AnnouncementMessage = $"{MentionUtils.MentionRole(rofaRoleId)} Gather up for the {currentMessage.DiscordMessage.Embeds.First().Title} in {MentionUtils.MentionChannel(BotData.GetROFAClanWarChannelId())}";
            return AnnouncementMessage;
        }

        internal static List<ulong?>? GetCurrentMessagesIds()
        {
            List<ulong?> response = [];
            if (CurrentMessages.Count > 0) foreach (Message? message in CurrentMessages) response.Add(message.DiscordMessage.Id);
            if (response.Count > 0) return response;
            return null;
        }
        internal static void UpdateBackupAttendanceMessage(Message message)
        {
            CurrentMessages.Add(message);
            Logger.LogInformation($"    Sucessfully added backup message for {message.ThursdayDate}: {message.DiscordMessage.Id}");
        }
        internal static Embed ChangeRofaAutomnaticSettings(IReadOnlyCollection<SocketSlashCommandDataOption> options)
        {
            Logger.LogInformation("    Changing settings");
            string message = "";

            Dictionary<string, bool> oldSettings = SettingsHandler.GetAutomaticSettingsRofa();
            Dictionary<string, bool> settings = [];
            foreach (SocketSlashCommandDataOption option in options)
            {
                settings[option.Name] = (bool)option.Value;
                message += $"{option.Name[0].ToString().ToUpper()}{option.Name.AsSpan(1)}: {oldSettings[option.Name]} -> {option.Value}\n";
                oldSettings.Remove(option.Name);
            }
            foreach (KeyValuePair<string, bool> setting in oldSettings)
            {
                message += $"{setting.Key[0].ToString().ToUpper()}{setting.Key.AsSpan(1)}: {setting.Value}\n";
                settings.Add(setting.Key, setting.Value);
            }
            SettingsHandler.SetAutomaticSettingsRofa(settings);

            EmbedBuilder embed = new()
            {
                Color = Color.Green,
                Title = message
            };
            return embed.Build();
        }
    }
}