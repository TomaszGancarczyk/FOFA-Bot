using Discord;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceHandler
    {
        private static AttendanceMessage? CurrentMessage;
        private readonly static int EventReminderMinutes = 90;
        private readonly static int EventCloseMinutes = 30;
        internal static async Task StartQuestionAttendanceEvent()
        {
            Logger.LogInformation($"    Starting attendance question event");
            Logger.LogInformation($"    HandlingEventQuestion");
            string template = await AttendanceQuestion.Handle();
            if (template == "Day Off")
            {
                Task.Delay(3600000).Wait();
                BotHandler.SetSignupMessageRunning(false);
                return;
            }
            if (template == "Next Message")
            {
                BotHandler.SetSignupMessageRunning(false);
                return;
            }
            CreateAttendanceEvent(null, null, template);
            await SendAttendanceMessage();
        }
        internal static void CreateAttendanceEvent(string? EventName, DateTime? eventDate, string? template)
        {
            Logger.LogInformation($"    Creating attendance event");
            AttendanceMessage? tempCurrentMessage;
            if (template != null)
                tempCurrentMessage = AttendanceMessageGenerator.CreateAttendanceMessageFromTemplate(template);
            else if (EventName != null && eventDate != null)
                tempCurrentMessage = AttendanceMessageGenerator.CreateCustomAttendanceMessage(EventName, eventDate.Value);
            else
            {
                Logger.LogError($"    Wrong data for message creation, returning");
                return;
            }
            if (tempCurrentMessage == null)
            {
                Logger.LogError($"    Cloudn't create attendance message, returning");
                return;
            }
            CurrentMessage = tempCurrentMessage;
        }
        internal static async Task SendAttendanceMessage()
        {
            if (CurrentMessage == null)
            {
                Logger.LogError($"    Attendance message not found, cannot send it");
                return;
            }
            Logger.LogInformation($"    Sending attendance message to {CurrentMessage.SignupsChannel.Name}");
            ulong pingMessage = BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetRofaRoleName()).Id;
            IMessage localCurrentMessage = await CurrentMessage.SignupsChannel.SendMessageAsync(
                $"<@&{pingMessage}>"
                , false, CurrentMessage.EmbedMessage.Build(), null, null, null, CurrentMessage.MessageButtons.Build());
            CurrentMessage.DiscordMessage = localCurrentMessage;
            AttendanceBackup.SaveBuckup(CurrentMessage);
            await HandleMessageRunning(localCurrentMessage.Id);
        }

        internal static async Task HandleMessageRunning(ulong messageId)
        {
            DateTime eventReminderTime = CurrentMessage.Date.AddMinutes(-EventReminderMinutes);
            if (DateTime.Now < eventReminderTime)
            {
                while (DateTime.Now < eventReminderTime)
                    Task.Delay(60000).Wait();
                bool isMessageDeleted = await CheckIfMessageIsDeleted(CurrentMessage.DiscordMessage.Id);
                if (CurrentMessage != null && messageId == CurrentMessage.DiscordMessage.Id && SettingsHandler.GetAutomaticReminder() && !isMessageDeleted)
                {
                    if (CurrentMessage.Reminder)
                    {
                        string reminderMessage = CreateReminderMessage();
                        if (reminderMessage != string.Empty)
                            await CurrentMessage.SignupsChannel.SendMessageAsync(reminderMessage);
                    }
                }
            }

            DateTime eventCloseTime = CurrentMessage.Date.AddMinutes(-EventCloseMinutes);
            if (DateTime.Now < eventCloseTime)
            {
                while (DateTime.Now < eventCloseTime)
                    Task.Delay(60000).Wait();
                bool isMessageDeleted = await CheckIfMessageIsDeleted(CurrentMessage.DiscordMessage.Id);
                if (CurrentMessage != null && messageId == CurrentMessage.DiscordMessage.Id && SettingsHandler.GetAutomaticReminder() && !isMessageDeleted)
                {
                    string annoucmentMessage = CreateAnnoucmentMessage();
                    if (annoucmentMessage != string.Empty)
                        await BotData.GetAnnoucmentChannel().SendMessageAsync(annoucmentMessage);
                }
            }

            if (CurrentMessage != null && CurrentMessage.DiscordMessage != null && messageId == CurrentMessage.DiscordMessage.Id && MemberHandler.GetMembers().Any(m => m.status == null))
                AttendanceGoogleSheet.HandleUnsignedUsers([.. MemberHandler.GetMembers().Where(m => m.status == null)]);
            if (CurrentMessage != null && CurrentMessage.DiscordMessage != null && messageId == CurrentMessage.DiscordMessage.Id)
                BotHandler.SetSignupMessageRunning(false);
            CurrentMessage = null;
        }

        internal static async Task<bool> CheckIfMessageIsDeleted(ulong messageId)
        {
            IMessageChannel channel = BotData.GetSignupsChannel();
            try
            {
                IMessage msg = await channel.GetMessageAsync(messageId);
                return false;
            }
            catch (Exception)
            {
                Logger.LogInformation($"    Message got deleted, skipping");
                return true;
            }
        }

        internal static EmbedBuilder? RefreshSignupMessage()
        {
            if (CurrentMessage == null) return null;
            Logger.LogInformation($"    Refreshing attendance message members");
            MemberHandler.RefreshMemberSquads();
            Logger.LogInformation($"    Refreshing attendance message fields");
            CurrentMessage.EmbedMessage = AttendanceMessageGenerator.AddMessageFields(CurrentMessage.EmbedMessage);
            CurrentMessage.EmbedMessage = AttendanceMessageGenerator.AddFooterMessage(CurrentMessage.EmbedMessage);
            AttendanceBackup.SaveBuckup(CurrentMessage);
            return CurrentMessage.EmbedMessage;
        }

        private static string CreateReminderMessage()
        {
            string reminderMessage = "## Don't forget to signup!";
            List<Member> members = MemberHandler.GetMembers();
            foreach (var member in members) if (member.status == null && member.discordUser != null)
                    reminderMessage += "\n" + MentionUtils.MentionUser(member.discordUser.Id);
            return reminderMessage;
        }
        private static string CreateAnnoucmentMessage()
        {
            ulong rofaRoleId = BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetRofaRoleName()).Id;
            string[] eventParts = CurrentMessage.DiscordMessage.Embeds.First().Title.Split(" ");
            string annoucmentMessage = $"{MentionUtils.MentionRole(rofaRoleId)} Gather up for the {CurrentMessage.DiscordMessage.Embeds.First().Title} in {MentionUtils.MentionChannel(BotData.GetClanWarChannelId())}";
            return annoucmentMessage;
        }

        internal static ulong? GetCurrentMessageId()
        {
            if (CurrentMessage != null) if (CurrentMessage.DiscordMessage != null) return CurrentMessage.DiscordMessage.Id;
            return null;
        }
        internal static EmbedBuilder ChangeAutomaticReminder(bool status)
        {
            EmbedBuilder embed;
            SettingsHandler.SetAutomaticReminder(status);
            if (SettingsHandler.GetAutomaticReminder() == status)
                embed = AttendanceMessageResponse.CreatePositiveStatusResponse(status);
            else embed = AttendanceMessageResponse.CreateNegativeStatusResponse();
            return embed;
        }
        internal static void UpdateBackupAttendanceMessage(AttendanceMessage messsage) => CurrentMessage = messsage;
    }
}