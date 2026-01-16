using Discord;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceHandler
    {
        private static IMessage CurrentMessage = null;
        internal static async Task StartQuestionAttendanceEvent()
        {
            Logger.LogInformation($"Starting attendance question event");
            ulong? localCurrentMessageId = null;
            Logger.LogInformation($"HandlingEventQuestion");
            string template = await Question.Handle(BotData.GetQuestionChannel());
            template = "Brawl";//TODO   temporary question response - template
            Logger.LogInformation($"Got response from question: {template}");
            AttendanceMessage message = await CreateAttendanceEvent(null, null, template);
            message = await AttendanceButton.AddAttendanceButtons(message);
            await SendAttendanceMessage(message);
        }
        private static async Task<AttendanceMessage> CreateAttendanceEvent(string? EventName, DateTime? eventDate, string? template)
        {
            Logger.LogInformation($"Creating attendance event");
            AttendanceMessage attendanceMessage = new();
            if (template != null)
                attendanceMessage = await AttendanceMessageGenerator.CreateAttendanceMessageFromTemplate(template);
            else
                attendanceMessage = await AttendanceMessageGenerator.CreateCustomAttendanceMessage(EventName, eventDate.Value);
            return attendanceMessage;
        }
        private static async Task SendAttendanceMessage(AttendanceMessage message)
        {
            Logger.LogInformation($"Sending attendance message to {message.signupsChannel.Name}");
        }

    }
}