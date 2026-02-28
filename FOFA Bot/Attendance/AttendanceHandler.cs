using Discord;
using Discord.WebSocket;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceHandler
    {
        private static AttendanceMessage? CurrentMessage;
        private readonly static int EventReminderMinutes = 90;
        private readonly static int EventCloseMinutes = 15;
        internal static async Task StartQuestionAttendanceEvent()
        {
            Logger.LogInformation($"    Starting attendance question event");
            Logger.LogInformation($"    HandlingEventQuestion");
            string template = await AttendanceQuestion.Handle();
            if (template == "Day Off")
            {
                Task.Delay(3600000).Wait();
                return;
            }
            if (template == "Next Message")
                return;
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
            Logger.LogInformation($"    Sending attendance message to {CurrentMessage.signupsChannel.Name}");
            ulong pingMessage = BotData.GetGuild().Roles.FirstOrDefault(role => role.Name == BotData.GetRofaRoleName()).Id;
            IMessage localCurrentMessage = await CurrentMessage.signupsChannel.SendMessageAsync(
                $"<@&{pingMessage}>"
                , false, CurrentMessage.embedMessage.Build(), null, null, null, CurrentMessage.messageButtons.Build());
            CurrentMessage.discordMessage = localCurrentMessage;
            if (CurrentMessage.Date == null)
                return;

            DateTime eventReminderTime = CurrentMessage.Date.Value.AddMinutes(-EventReminderMinutes);
            while (DateTime.Now < eventReminderTime)
                Task.Delay(60000).Wait();
            bool isMessageDeleted = await CheckIfMessageIsDeleted(CurrentMessage.discordMessage.Id);
            if (localCurrentMessage != null && CurrentMessage != null && localCurrentMessage.Id == CurrentMessage.discordMessage.Id && SettingsHandler.GetAutomaticReminder() && !isMessageDeleted)
            {
                string reminderMessage = CreateReminderMessage();
                if (reminderMessage != string.Empty)
                    await CurrentMessage.signupsChannel.SendMessageAsync(reminderMessage);
            }

            DateTime eventCloseTime = CurrentMessage.Date.Value.AddMinutes(-EventCloseMinutes);
            while (DateTime.Now < eventCloseTime)
                Task.Delay(60000).Wait();
            if (localCurrentMessage != null && localCurrentMessage.Id == CurrentMessage.discordMessage.Id && MemberHandler.GetMembers().Any(m => m.status == null))
                AttendanceGoogleSheet.HandleUnsignedUsers([.. MemberHandler.GetMembers().Where(m => m.status == null)]);
            if (localCurrentMessage != null && localCurrentMessage.Id == CurrentMessage.discordMessage.Id)
                BotHandler.SetSignupMessageRunning(false);
            CurrentMessage = null;
        }

        private static async Task<bool> CheckIfMessageIsDeleted(ulong messageId)
        {
            IMessageChannel channel = BotData.GetSignupsChannel();
            IMessage msg = await channel.GetMessageAsync(messageId);
            if (msg != null) return false;
            Logger.LogInformation($"    Signup message got deleted, skipping reminder");
            return true;
        }

        internal static EmbedBuilder? RefreshSignupMessage()
        {
            if (CurrentMessage == null) return null;
            Logger.LogInformation($"    Refreshing attendance message members");
            MemberHandler.RefreshMemberSquads();
            Logger.LogInformation($"    Refreshing attendance message fields");
            CurrentMessage.embedMessage = AttendanceMessageGenerator.AddMessageFields(CurrentMessage.embedMessage);
            CurrentMessage.embedMessage = AttendanceMessageGenerator.AddFooterMessage(CurrentMessage.embedMessage);
            return CurrentMessage.embedMessage;
        }

        private static string CreateReminderMessage()
        {
            string reminderMessage = "## Don't forget to signup!";
            List<Member> members = MemberHandler.GetMembers();
            foreach (var member in members) if (member.status == null && member.discordUser != null)
                    reminderMessage += "\n" + MentionUtils.MentionUser(member.discordUser.Id);
            return reminderMessage;
        }

        internal static ulong? GetCurrentMessageId()
        {
            if (CurrentMessage != null) if (CurrentMessage.discordMessage != null) return CurrentMessage.discordMessage.Id;
                else return null;
            else return null;

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
    }
}