using Discord;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceHandler
    {
        private static IMessage? CurrentMessage;
        private static IMessageChannel? SignupsChannel;
        private static IMessageChannel? QuestionChannel;
        internal static async Task StartAttendanceEvent()
        {
            Logger.LogInformation($"Starting attendance event");
            await CreateAttendanceEventData();
            ulong? localCurrentMessageId = null;
        }
        internal static async Task CreateAttendanceEventData()
        {
            Logger.LogInformation("Creating attendance data");
            CurrentMessage = null;
            SignupsChannel = BotData.GetSignupsChannel();
            QuestionChannel = BotData.GetQuestionChannel();
        }
    }
}
