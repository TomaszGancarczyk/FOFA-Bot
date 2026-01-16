using Discord;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceButton
    {
        internal async static Task<AttendanceMessage> AddAttendanceButtons(AttendanceMessage message)
        {
            Logger.LogInformation($"Adding attendance buttons");
            ComponentBuilder buttons = new ComponentBuilder()
                .WithButton("Present", "presentButton", ButtonStyle.Success)
                .WithButton("Absent", "absentButton", ButtonStyle.Danger);
            Logger.LogInformation($"Created buttons");
            message.messageButtons = buttons;
            return message;
        }
    }
}
