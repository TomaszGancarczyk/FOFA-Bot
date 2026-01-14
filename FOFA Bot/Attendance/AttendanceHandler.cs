using Discord;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceHandler
    {
        private static IMessage? CurrentMessage;
        private static IMessageChannel? SignupsChannel;
        private static IMessageChannel? QuestionChannel;
        internal static async Task StartQuestionAttendanceEvent()
        {
            Logger.LogInformation($"Starting attendance question event");
            await CreateAttendanceEventData();
            ulong? localCurrentMessageId = null;
            Logger.LogInformation($"HandlingEventQuestion");
            string template = await Question.Handle();
        }
        private static async Task CreateAttendanceEvent(string? EventName, DateTime? eventDate, string? template)
        {
            Logger.LogInformation($"Creating attendance event");
            AttendanceMessage attendanceMessage;
            if (template != null)
            {
                attendanceMessage = await AttendanceMessageGenerator.CreateAttendanceMessageFromTemplate(template);
            }
            else
            {
                attendanceMessage = await AttendanceMessageGenerator.CreateCustomAttendanceMessage(EventName, eventDate.Value);
            }

        }


        private static async Task CreateAttendanceEventData()
        {
            Logger.LogInformation("Creating attendance data");
            CurrentMessage = null;
            SignupsChannel = BotData.GetSignupsChannel();
            QuestionChannel = BotData.GetQuestionChannel();
        }

    }
}
