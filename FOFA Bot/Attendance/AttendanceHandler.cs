using Discord;
using Discord.WebSocket;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceHandler
    {
        private static AttendanceMessage CurrentMessage;
        internal static async Task StartQuestionAttendanceEvent()
        {
            Logger.LogInformation($"Starting attendance question event");
            Logger.LogInformation($"HandlingEventQuestion");
            string template = await Question.Handle(BotData.GetQuestionChannel());
            template = "Brawl";//TODO   temporary question response - template
            Logger.LogInformation($"Got response from question: {template}");
            await CreateAttendanceEvent(null, null, template);
            await SendAttendanceMessage();
        }
        private static async Task CreateAttendanceEvent(string? EventName, DateTime? eventDate, string? template) //create custom attendance as well
        {
            Logger.LogInformation($"Creating attendance event");
            if (template != null)
                CurrentMessage = await AttendanceMessageGenerator.CreateAttendanceMessageFromTemplate(template);
            else
                CurrentMessage = await AttendanceMessageGenerator.CreateCustomAttendanceMessage(EventName, eventDate.Value);
        }
        private static async Task SendAttendanceMessage()
        {
            Logger.LogInformation($"Sending attendance message to {CurrentMessage.signupsChannel.Name}");
            CurrentMessage.discordMessage = await CurrentMessage.signupsChannel.SendMessageAsync("", false, CurrentMessage.embedMessage.Build(), null, null, null, CurrentMessage.messageButtons.Build());
        }
        internal static async Task<EmbedBuilder> RefreshSignupMessageFields()
        {
            Logger.LogInformation($"Refreshing attendance message fields");
            CurrentMessage.embedMessage = await AttendanceMessageGenerator.AddMessageFields(CurrentMessage.embedMessage);
            return CurrentMessage.embedMessage;
        }


        internal static ulong? GetCurrentMessageId()
        {
            return CurrentMessage.discordMessage.Id;
        }
    }
}