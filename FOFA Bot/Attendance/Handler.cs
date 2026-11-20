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
        internal static void CreateAttendanceEvent(string clan)
        {
            Logger.LogInformation($"    Creating attendance event");
            BotHandler.ChangeSignupMessageRunning(1, clan);
            Message? tempCurrentMessage = MessageGenerator.CreateAttendanceMessage(clan);
            if (tempCurrentMessage == null)
            {
                Logger.LogError($"    Cloudn't create attendance message, returning");
                return;
            }
            Message? message = tempCurrentMessage;
            if (clan == "ROFA") message.SignupsChannel = BotData.GetSignupsChannel("ROFA");
            else if (clan == "BOFA") message.SignupsChannel = BotData.GetSignupsChannel("BOFA");
            _ = SendAttendanceMessage(message, clan);
            Logger.LogInformation($"    Attendance event finished");
            return;
        }
        internal static async Task SendAttendanceMessage(Message? currentMessage, string clan)
        {
            Logger.LogInformation($"    Sending attendance message to {currentMessage.SignupsChannel.Name}");
            string pingMessage = "";
            pingMessage = $"<@&{BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetClanRoleName(clan)).Id}>";
            try
            {
                IMessage localCurrentMessage = await currentMessage.SignupsChannel.SendMessageAsync(
                    pingMessage,
                    false, currentMessage.EmbedMessage.Build(), null, null, null, currentMessage.MessageButtons.Build());
                currentMessage.DiscordMessage = localCurrentMessage;
                Backup.SaveBuckup(currentMessage);
                CurrentMessages.Add(currentMessage);
                await HandleMessageRunning(localCurrentMessage.Id, currentMessage.Clan);
            }
            catch (Exception e)
            {
                Logger.LogError($"Error while handling attendance message:\n{e}");
            }
        }

        internal static async Task HandleMessageRunning(ulong messageId, string clan)
        {
            Message? currentMessage = CurrentMessages.First(m => m.DiscordMessage.Id == messageId);
            DateTime[] eventReminderTimes =
            [
                currentMessage.ThursdayDate.AddMinutes(-EventReminderMinutes),
                currentMessage.ThursdayDate.AddMinutes(-EventReminderMinutes).AddDays(1),
                currentMessage.ThursdayDate.AddMinutes(-EventReminderMinutes).AddDays(2),
                currentMessage.ThursdayDate.AddMinutes(-EventReminderMinutes).AddDays(3).AddHours(-1),
            ];
            foreach (DateTime reminderTime in eventReminderTimes)
            {
                if (DateTime.Now < reminderTime)
                {
                    Logger.LogInformation($"    Next signup reminder on {reminderTime} in {reminderTime - DateTime.Now}");
                    while (DateTime.Now < reminderTime)
                        Task.Delay(60000).Wait();
                    if (CurrentMessages.Count > 0 && messageId == currentMessage.DiscordMessage.Id && SettingsHandler.GetAutomaticReminderRofa(clan) && !await CheckIfMessageIsDeleted(currentMessage.DiscordMessage.Id, clan))
                    {
                        string? reminderMessage = CreateReminderMessage(Array.IndexOf(eventReminderTimes, reminderTime), clan);
                        if (reminderMessage != string.Empty && reminderMessage != null)
                            try
                            {
                                await currentMessage.SignupsChannel.SendMessageAsync(reminderMessage);
                            }
                            catch (Exception e)
                            {
                                Logger.LogError($"Error when sending reminder message:\n{e}");
                            }
                    }
                }
                if (DateTime.Now < reminderTime.AddHours(2))
                {
                    Logger.LogInformation($"    Next clan war reminder on {reminderTime.AddHours(2)} in {reminderTime.AddHours(2) - DateTime.Now}");
                    while (DateTime.Now < reminderTime.AddHours(2))
                        Task.Delay(60000).Wait();
                    if (CurrentMessages.Count > 0 && messageId == currentMessage.DiscordMessage.Id && SettingsHandler.GetAutomaticReminderRofa(clan) && !await CheckIfMessageIsDeleted(currentMessage.DiscordMessage.Id, clan))
                    {
                        string AnnouncementMessage = CreateAnnouncementMessage(reminderTime.DayOfWeek, clan);
                        if (AnnouncementMessage != string.Empty && AnnouncementMessage != null)
                            try
                            {
                                await BotData.GetAnnouncementChannel(clan).SendMessageAsync(AnnouncementMessage);
                            }
                            catch (Exception e)
                            {
                                Logger.LogError($"Error when sending announcement message:\n{e}");

                            }
                    }
                }
            }
            if (CurrentMessages.Count > 0 && currentMessage.DiscordMessage != null && messageId == currentMessage.DiscordMessage.Id)
                CurrentMessages.Remove(CurrentMessages.First(m => m.DiscordMessage.Id == messageId));
            Backup.DeleteBackup(currentMessage.DiscordMessage.Id.ToString());
            BotHandler.ChangeSignupMessageRunning(-1, clan);
        }

        internal static async Task<bool> CheckIfMessageIsDeleted(ulong messageId, string clan)
        {
            IMessageChannel? channel = null;
            if (clan == "ROFA") channel = BotData.GetSignupsChannel("ROFA");
            else if (clan == "BOFA") channel = BotData.GetSignupsChannel("BOFA");
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

        internal static EmbedBuilder? RefreshSignupMessage(ulong? messageId, string clan)
        {
            Message? currentMessage = CurrentMessages.First(m => m.DiscordMessage.Id == messageId);
            if (CurrentMessages.Count <= 0) return null;
            Logger.LogInformation($"    Refreshing attendance message members");
            MemberHandler.RefreshMemberSquads(clan);
            Logger.LogInformation($"    Refreshing attendance message fields");
            currentMessage.EmbedMessage = MessageGenerator.AddMessageFields(currentMessage.EmbedMessage, clan);
            currentMessage.EmbedMessage = MessageGenerator.AddFooterMessage(currentMessage.EmbedMessage, clan);
            Backup.SaveBuckup(currentMessage);
            return currentMessage.EmbedMessage;
        }

        private static string? CreateReminderMessage(int dayOfSignup, string clan)
        {
            string reminderMessage = "## Don't forget to signup!";
            List<Member>? members = MemberHandler.GetMembers(clan);
            bool nullmembers = false;
            foreach (var member in members) if (member.status[dayOfSignup] == null && member.discordUser != null)
                {
                    reminderMessage += "\n" + MentionUtils.MentionUser(member.discordUser.Id);
                    nullmembers = true;
                }
            if (nullmembers) return reminderMessage;
            return null;
        }
        private static string CreateAnnouncementMessage(DayOfWeek annoucmentDay, string clan)
        {
            string annoucmentDayStrting = $"{annoucmentDay} Tournament";
            if (annoucmentDay == DayOfWeek.Sunday) annoucmentDayStrting = $"{annoucmentDay} Base Capture";
            ulong clanRoleId = BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetClanRoleName(clan)).Id;
            string AnnouncementMessage = $"{MentionUtils.MentionRole(clanRoleId)} Gather up for the {annoucmentDayStrting} in {MentionUtils.MentionChannel(BotData.GetClanWarChannelId(clan))}";
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
        internal static Embed ChangeAutomnaticSettings(IReadOnlyCollection<SocketSlashCommandDataOption> options, string clan)
        {
            Logger.LogInformation($"    Changing settings for {clan}");
            string message = "";
            Dictionary<string, bool>? oldSettings = SettingsHandler.GetAutomaticSettings(clan);
            Dictionary<string, bool>? settings = [];
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
            SettingsHandler.SetAutomaticSettings(settings, clan);

            EmbedBuilder embed = new()
            {
                Color = Color.Green,
                Title = message
            };
            return embed.Build();
        }
    }
}