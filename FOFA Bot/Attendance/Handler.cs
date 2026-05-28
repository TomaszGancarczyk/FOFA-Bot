using Discord;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceHandler
    {
        private static List<Message?> CurrentMessages = [];
        private readonly static int EventReminderMinutes = 150;
        private readonly static int EventCloseMinutes = 30;
        internal static async Task StartQuestionAttendanceEvent()
        {
            Logger.LogInformation($"    Starting attendance question event");
            BotHandler.ChangeSignupMessageRunning(1);
            Logger.LogInformation($"    HandlingEventQuestion");
            string template;
            template = await AttendanceQuestion.Handle();
            if (template == "Day Off")
            {
                Task.Delay(3600000).Wait();
                BotHandler.ChangeSignupMessageRunning(-1);
                return;
            }
            if (template == "Next Message")
            {
                BotHandler.ChangeSignupMessageRunning(-1);
                return;
            }
            Message? message = CreateAttendanceEvent(null, null, template);
            BotHandler.ChangeSignupMessageRunning(-1);
            await SendAttendanceMessage(message);
        }
        internal static Message? CreateAttendanceEvent(string? EventName, DateTime? eventDate, string? template)
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
                    string annoucmentMessage = CreateAnnoucmentMessage(messageId);
                    if (annoucmentMessage != string.Empty)
                        await BotData.GetAnnoucmentChannel().SendMessageAsync(annoucmentMessage);
                }
            }

            if (CurrentMessages.Count > 0 && currentMessage.DiscordMessage != null && messageId == currentMessage.DiscordMessage.Id && MemberHandler.GetMembers().Any(m => m.status == null))
                GoogleSheet.HandleUnsignedUsers([.. MemberHandler.GetMembers().Where(m => m.status == null)]);
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
        private static string CreateAnnoucmentMessage(ulong? messageId)
        {
            Message? currentMessage = CurrentMessages.First(m => m.DiscordMessage.Id == messageId);
            ulong rofaRoleId = BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetRofaRoleName()).Id;
            string[] eventParts = currentMessage.DiscordMessage.Embeds.First().Title.Split(" ");
            string annoucmentMessage = $"{MentionUtils.MentionRole(rofaRoleId)} Gather up for the {currentMessage.DiscordMessage.Embeds.First().Title} in {MentionUtils.MentionChannel(BotData.GetClanWarChannelId())}";
            return annoucmentMessage;
        }

        internal static List<ulong?>? GetCurrentMessagesIds()
        {
            List<ulong?> response = [];
            if (CurrentMessages.Count > 0) foreach (Message? message in CurrentMessages) response.Add(message.DiscordMessage.Id);
            if (response.Count > 0) return response;
            return null;
        }
        internal static EmbedBuilder ChangeAutomaticReminder(bool status)
        {
            EmbedBuilder embed;
            SettingsHandler.SetAutomaticReminder(status);
            if (SettingsHandler.GetAutomaticReminder() == status)
                embed = MessageResponse.CreatePositiveStatusResponse(status);
            else embed = MessageResponse.CreateNegativeStatusResponse();
            return embed;
        }
        internal static void UpdateBackupAttendanceMessage(Message messsage) => CurrentMessages.Add(messsage);
    }
}