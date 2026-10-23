using Discord;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceQuestion
    {
        private static IMessage? CurrentQuestionMessage = null;
        private static bool WaitingForQuestionResponse = true;
        private static string? QuestionResponse;
        private static DateTime EventDateTime;
        internal static async Task<string> Handle()
        {
            CurrentQuestionMessage = null;
            QuestionResponse = null;
            IMessageChannel questionChannel = BotData.GetROFAQuestionChannel();
            EventDateTime = MessageGenerator.GetEventDateTime(20);
            while (DateTime.Now < EventDateTime && WaitingForQuestionResponse)
            {
                await Task.Delay(1000);
            }
            if (CurrentQuestionMessage != null && QuestionResponse != null)
            {
                Logger.LogInformation($"    Got response from question: {QuestionResponse}");
                WaitingForQuestionResponse = true;
                CurrentQuestionMessage = null;
                return QuestionResponse;
            }
            else
            {
                if (CurrentQuestionMessage != null)
                    await CurrentQuestionMessage.DeleteAsync();
                return "Next Message";
            }
        }

        internal static void SetQuestionAnswear(ulong questionMessageId, string questionResponse)
        {
            if (CurrentQuestionMessage != null)
                if (questionMessageId == CurrentQuestionMessage.Id)
                {
                    QuestionResponse = questionResponse;
                    WaitingForQuestionResponse = false;
                }
                else Logger.LogError($"    Got response from question message that has different ID than CurrentQuestionMessage");
            else Logger.LogError($"    CurrentQuestionMessage is null");
        }
    }
}