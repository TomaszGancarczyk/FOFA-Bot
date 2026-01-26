using Discord;
using FOFA_Bot.Data;
using FOFA_Bot.Bot;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceHandler
    {
        private static AttendanceMessage CurrentMessage;
        private readonly static int EventCloseMinutes = 15;
        internal static async Task StartQuestionAttendanceEvent()
        {
            Logger.LogInformation($"Starting attendance question event");
            Logger.LogInformation($"HandlingEventQuestion");
            string template = await AttendanceQuestion.Handle(BotData.GetQuestionChannel());
            if (template == "Day Off")
            {
                DateTime nextEventTime = AttendanceMessageGenerator.GetEventDateTime(20);
                await Task.Delay(nextEventTime - DateTime.Now);
                return;
            }
            if (template == "Next Message")
                return;
            await CreateAttendanceEvent(null, null, template);
        }
        private static async Task CreateAttendanceEvent(string? EventName, DateTime? eventDate, string? template) //create custom attendance as well
        {
            Logger.LogInformation($"Creating attendance event");
            if (template != null)
                CurrentMessage = AttendanceMessageGenerator.CreateAttendanceMessageFromTemplate(template);
            else
                CurrentMessage = AttendanceMessageGenerator.CreateCustomAttendanceMessage(EventName, eventDate.Value);
            await SendAttendanceMessage();
        }
        private static async Task SendAttendanceMessage()
        {
            Logger.LogInformation($"Sending attendance message to {CurrentMessage.signupsChannel.Name}");
            IMessage localCurrentMessage = await CurrentMessage.signupsChannel.SendMessageAsync("", false, CurrentMessage.embedMessage.Build(), null, null, null, CurrentMessage.messageButtons.Build());
            CurrentMessage.discordMessage = localCurrentMessage;
            DateTime eventCloseTime = CurrentMessage.Date.Value.AddMinutes(-EventCloseMinutes);
            while (DateTime.Now < eventCloseTime)
                Task.Delay(60000).Wait();
            if (localCurrentMessage.Id == CurrentMessage.discordMessage.Id && MemberHandler.GetMembers().Any(m => m.status == null))
                AttendanceGoogleSheet.HandleUnsignedUsers([.. MemberHandler.GetMembers().Where(m => m.status == null)]);
        }
        internal static async Task<EmbedBuilder> RefreshSignupMessage()
        {
            Logger.LogInformation($"Refreshing attendance message fields");
            CurrentMessage.embedMessage = AttendanceMessageGenerator.AddMessageFields(CurrentMessage.embedMessage);
            CurrentMessage.embedMessage = AttendanceMessageGenerator.AddFooterMessage(CurrentMessage.embedMessage);
            return CurrentMessage.embedMessage;
        }


        internal static ulong? GetCurrentMessageId()
        {
            if (CurrentMessage != null) return CurrentMessage.discordMessage.Id;
            else return null;
                
        }
    }
}