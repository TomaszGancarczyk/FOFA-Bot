using Discord;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceQuestion
    {
        private static IMessage? CurrentQuestionMessage = null;
        private static bool WaitingForQuestionResponse;
        private static string? QuestionResponse;
        private static DateTime EventDateTime;

        private static readonly List<DayOfWeek> TournamentDays =
        [
            DayOfWeek.Thursday,
            DayOfWeek.Friday,
            DayOfWeek.Saturday
        ];
        private static readonly List<DayOfWeek> BaseCaptureDays =
        [
            DayOfWeek.Sunday
        ];
        private static readonly List<DayOfWeek> BrawlDays =
        [
            DayOfWeek.Monday,
            DayOfWeek.Tuesday,
            DayOfWeek.Wednesday,
        ];
        internal static async Task<string> Handle(IMessageChannel questionChannel)
        {
            CurrentQuestionMessage = null;
            WaitingForQuestionResponse = true;
            EventDateTime = AttendanceMessageGenerator.GetEventDateTime(20);
            Logger.LogInformation($"Creating attendance event question");

            string questionMessageContent;
            if (EventDateTime.DayOfWeek == DateTime.Now.DayOfWeek)
                questionMessageContent = $"## What do we play today?";
            else questionMessageContent = $"## What do we play tomorrow?";

            ComponentBuilder component = CreateQuestionButtons();
            Logger.LogInformation($"Sending attendance question to {questionChannel.Name}");
            IMessage? localCurrentQuestionMessage = await questionChannel.SendMessageAsync(questionMessageContent, components: component.Build());
            CurrentQuestionMessage = localCurrentQuestionMessage;
            while ((DateTime.Now - localCurrentQuestionMessage.CreatedAt).Hours < 24 && WaitingForQuestionResponse)
            {
                Console.WriteLine((DateTime.Now - localCurrentQuestionMessage.CreatedAt).Hours);
                await Task.Delay(1000);
            }
            if (CurrentQuestionMessage != null && QuestionResponse != null && localCurrentQuestionMessage.Id == CurrentQuestionMessage.Id)
            {
                Logger.LogInformation($"Got response from question: {QuestionResponse}");
                return QuestionResponse;
            }
            else
                return "Next Message";
        }

        internal static void SetQuestionAnswear(ulong questionMessageId, string questionResponse)
        {
            if (CurrentQuestionMessage != null)
                if (questionMessageId == CurrentQuestionMessage.Id)
                {
                    QuestionResponse = questionResponse;
                    WaitingForQuestionResponse = false;
                }
                else Logger.LogError($"Got response from question message that has different ID than CurrentQuestionMessage");
            else Logger.LogError($"CurrentQuestionMessage is null");
        }

        private static ComponentBuilder CreateQuestionButtons()
        {
            Logger.LogInformation($"Creating attendance event question buttons");
            DayOfWeek eventDayOfWeek = EventDateTime.DayOfWeek;
            ComponentBuilder component = new();
            if (TournamentDays.Contains(eventDayOfWeek))
                component.WithButton("Tournament", "tournamentButton", emote: new Emoji("⚔️"));
            if (BaseCaptureDays.Contains(eventDayOfWeek))
                component.WithButton("Base Capture", "baseCaptureButton", emote: new Emoji("👑"));
            if (BrawlDays.Contains(eventDayOfWeek))
                component.WithButton("Brawl", "brawlButton", emote: new Emoji("💵"));
            component.WithButton("Golden Drop", "goldenDropButton", emote: new Emoji("💵"));
            component.WithButton("Day Off", "dayOffButton", emote: new Emoji("🏖️"));
            return component;
        }
    }
}
