using Discord;
using FOFA_Bot.Bot;

namespace FOFA_Bot.Attendance
{
    internal class SlashAttendanceHandler
    {
        internal async static Task<EmbedBuilder?> CreateSignupTemplate(long templateOption)
        {
            BotHandler.SetSignupMessageRunning(true);
            string template = "";
            switch (templateOption)
            {
                case 0:
                    template = "Tournament";
                    break;
                case 1:
                    template = "Brawl";
                    break;
                case 2:
                    template = "Base Capture";
                    break;
                case 3:
                    template = "Golden Drop";
                    break;
                case 4:
                    template = "Stillwaters Chrono/Pulpe/Drops";
                    break;
            }
            AttendanceHandler.CreateAttendanceEvent(null, null, template);
            _ = AttendanceHandler.SendAttendanceMessage();
            return SuccessMessage();
        }
        internal async static Task<EmbedBuilder> CreateSignupCustom(string eventName, string date)
        {
            BotHandler.SetSignupMessageRunning(true);
            string[] dateParts = date.Split('.');
            DateTime? formatedDate;
            if (dateParts.Length != 2 && dateParts.Length != 5)
            {
                Logger.LogWarning($"Incorrect date used for custom signup: {date}");
                BotHandler.SetSignupMessageRunning(false);
                return DateErrorMessage($"Wrong date for event, please use [day.month.year.hour.minute] of the event or [hours.minutes] untill the event");
            }
            else formatedDate = FormatDate(dateParts);
            if (formatedDate == null)
            {
                Logger.LogWarning($"Incorrect numbers used for custom signup: {date}");
                return DateErrorMessage($"Wrong date for event, cannot create date from provided numbers");
            }
            if (formatedDate <= DateTime.Now)
            {
                Logger.LogWarning($"Incorrect numbers used for custom signup: {date}");
                return DateErrorMessage("The date provided for event already passed");
            }
            AttendanceHandler.CreateAttendanceEvent(eventName, formatedDate, null);
            _ = AttendanceHandler.SendAttendanceMessage();
            return SuccessMessage();
        }

        private static DateTime? FormatDate(string[] dateParts)
        {
            Logger.LogInformation($"Formatting date for custom signup");
            List<int> datePartsFormatted = [];
            foreach (string datePart in dateParts)
            {
                try { datePartsFormatted.Add(int.Parse(datePart)); }
                catch (Exception) { return null; }
            }
            if (datePartsFormatted.Count == 2) return FormatShortDate(datePartsFormatted);
            if (datePartsFormatted.Count == 5) return FormatLongDate(datePartsFormatted);
            return null;
        }
        private static DateTime? FormatShortDate(List<int> dateParts)
        {
            //[hours.minutes] untill the event
            Logger.LogInformation($"Formatting short date");
            DateTime eventDateTime = DateTime.Now;
            try
            {
                eventDateTime = eventDateTime.AddHours(dateParts[0]);
                eventDateTime = eventDateTime.AddMinutes(dateParts[1]);
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                return null;
            }
            Logger.LogInformation($"Formatting date to {eventDateTime}");
            return eventDateTime;
        }
        private static DateTime? FormatLongDate(List<int> dateParts)
        {
            //[day.month.year.hour.minute] of the event
            if (dateParts[2] < 2000) dateParts[2] = dateParts[2] + 2000;
            Logger.LogInformation($"Formatting long date");
            DateTime? eventDateTime;
            try
            {
                eventDateTime = new DateTime(
                    dateParts[2],
                    dateParts[1],
                    dateParts[0],
                    dateParts[3],
                    dateParts[4],
                    0, DateTimeKind.Local
                    );
            }
            catch (Exception e)
            {
                Logger.LogError(e.ToString());
                return null;
            }
            return eventDateTime;
        }

        private static EmbedBuilder SuccessMessage()
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Green);
            embed.WithTitle($"Successfully created signup message");
            return embed;
        }
        private static EmbedBuilder NameErrorMessage()
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red);
            embed.WithTitle($"Wrong name for event");
            return embed;
        }
        private static EmbedBuilder DateErrorMessage(string message)
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red);
            embed.WithTitle(message);
            return embed;
        }
    }
}
