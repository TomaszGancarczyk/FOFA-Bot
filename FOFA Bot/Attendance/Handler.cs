using Discord;
using Discord.WebSocket;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;
using Newtonsoft.Json.Converters;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceHandler
    {
        private static List<Message?> CurrentMessages = [];
        private static readonly int EventReminderMinutes = 150;
        private static readonly int EventCloseMinutes = 30;
        private static readonly Dictionary<DayOfWeek, string> AutomaticSignupPosts = new()
        {
            { DayOfWeek.Monday, "Brawl" },
            { DayOfWeek.Tuesday, "Brawl" },
            { DayOfWeek.Wednesday, "Brawl" },
            { DayOfWeek.Thursday, "Tournament" },
            { DayOfWeek.Friday, "Tournament" },
            { DayOfWeek.Saturday, "Tournament" },
            { DayOfWeek.Sunday, "Base Capture" },
        };
        internal static async Task StartQuestionAttendanceEvent()
        {
            Logger.LogInformation($"    Starting attendance question event");
            BotHandler.ChangeSignupMessageRunning(1);
            string template;

            if (SettingsHandler.GetAutomaticSignupQuestion())
            {
                Logger.LogInformation($"    HandlingEventQuestion");
                template = await AttendanceQuestion.Handle();
                if (template == "Day Off")
                {
                    while (DateTime.Now.Hour == BotHandler.SignupQuestionHour)
                        Task.Delay(60000).Wait();
                    BotHandler.ChangeSignupMessageRunning(-1);
                    return;
                }
                if (template == "Next Message")
                {
                    BotHandler.ChangeSignupMessageRunning(-1);
                    return;
                }
            }
            else template = AutomaticSignupPosts[DateTime.Now.DayOfWeek];

            Message? message = CreateAttendanceEvent(template: template);
            BotHandler.ChangeSignupMessageRunning(-1);
            await SendAttendanceMessage(message);
        }
        internal static Message? CreateAttendanceEvent(string? EventName = null, DateTime? eventDate = null, string? template = null)
        {
            Logger.LogInformation($"    Creating attendance event");
            Message? tempCurrentMessage;
            if (template != null)
                tempCurrentMessage = MessageGenerator.CreateAttendanceMessageFromTemplate(template);
            else if (EventName != null && eventDate != null)
                tempCurrentMessage = MessageGenerator.CreateCustomAttendanceMessage(EventName, eventDate.Value);
            else
            {
                Logger.LogError($"    Wrong data for message creation, returning");
                return null;
            }
            if (tempCurrentMessage == null)
            {
                Logger.LogError($"    Cloudn't create attendance message, returning");
                return null;
            }
            return tempCurrentMessage;
        }
        internal static async Task SendAttendanceMessage(Message? currentMessage)
        {
            BotHandler.ChangeSignupMessageRunning(1);
            Logger.LogInformation($"    Sending attendance message to {currentMessage.SignupsChannel.Name}");
            ulong pingMessage = BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetRofaRoleName()).Id;
            IMessage localCurrentMessage = await currentMessage.SignupsChannel.SendMessageAsync(
                $"<@&{pingMessage}>"
                , false, currentMessage.EmbedMessage.Build(), null, null, null, currentMessage.MessageButtons.Build());
            currentMessage.DiscordMessage = localCurrentMessage;
            Backup.SaveBuckup(currentMessage);
            CurrentMessages.Add(currentMessage);
            await HandleMessageRunning(localCurrentMessage.Id);
        }

        internal static async Task HandleMessageRunning(ulong messageId)
        {
            Message? currentMessage = CurrentMessages.First(m => m.DiscordMessage.Id == messageId);
            DateTime eventReminderTime = currentMessage.Date.AddMinutes(-EventReminderMinutes);
            if (DateTime.Now < eventReminderTime)
            {
                while (DateTime.Now < eventReminderTime)
                    Task.Delay(60000).Wait();
                bool isMessageDeleted = await CheckIfMessageIsDeleted(currentMessage.DiscordMessage.Id);
                if (CurrentMessages.Count > 0 && messageId == currentMessage.DiscordMessage.Id && SettingsHandler.GetAutomaticReminder() && !isMessageDeleted)
                {
                    if (currentMessage.Reminder)
                    {
                        string? reminderMessage = CreateReminderMessage();
                        if (reminderMessage != string.Empty)
                            await currentMessage.SignupsChannel.SendMessageAsync(reminderMessage);
                    }
                }
            }

            DateTime eventCloseTime = currentMessage.Date.AddMinutes(-EventCloseMinutes);
            if (DateTime.Now < eventCloseTime)
            {
                while (DateTime.Now < eventCloseTime)
                    Task.Delay(60000).Wait();
                bool isMessageDeleted = await CheckIfMessageIsDeleted(currentMessage.DiscordMessage.Id);
                if (CurrentMessages.Count > 0 && messageId == currentMessage.DiscordMessage.Id && SettingsHandler.GetAutomaticReminder() && !isMessageDeleted)
                {
                    string AnnouncementMessage = CreateAnnouncementMessage(messageId);
                    if (AnnouncementMessage != string.Empty && AnnouncementMessage != null)
                        try
                        {
                            await BotData.GetAnnouncementChannel().SendMessageAsync(AnnouncementMessage);
                        }
                        catch (Exception e)
                        {
                            Logger.LogError($"Error when sending announcement message:\n{e}");
                        }
                }
            }
            string eventName = string.Join(" ", currentMessage.EmbedMessage.Title.Split(" ").Skip(1));
            if (CurrentMessages.Count > 0 && currentMessage.DiscordMessage != null && messageId == currentMessage.DiscordMessage.Id)
                BotHandler.ChangeSignupMessageRunning(-1);
            CurrentMessages.Remove(CurrentMessages.First(m => m.DiscordMessage.Id == messageId));
        }

        internal static async Task<bool> CheckIfMessageIsDeleted(ulong messageId)
        {
            IMessageChannel channel = BotData.GetSignupsChannel();
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
            ulong rofaRoleId = BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetRofaRoleName()).Id;
            string[] eventParts = currentMessage.DiscordMessage.Embeds.First().Title.Split(" ");
            string AnnouncementMessage = $"{MentionUtils.MentionRole(rofaRoleId)} Gather up for the {currentMessage.DiscordMessage.Embeds.First().Title} in {MentionUtils.MentionChannel(BotData.GetClanWarChannelId())}";
            return AnnouncementMessage;
        }

        internal static List<ulong?>? GetCurrentMessagesIds()
        {
            List<ulong?> response = [];
            if (CurrentMessages.Count > 0) foreach (Message? message in CurrentMessages) response.Add(message.DiscordMessage.Id);
            if (response.Count > 0) return response;
            return null;
        }
        internal static void UpdateBackupAttendanceMessage(Message messsage) => CurrentMessages.Add(messsage);
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
                message += $"{setting.Key[0].ToString().ToUpper()}{setting.Key.AsSpan(1)}: {setting.Value}";
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