using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;

namespace FOFA_Bot.Bot
{
    internal class ButtonEvent
    {
        public static async Task Handle(SocketMessageComponent component)
        {
            EmbedBuilder? responseMessage;
            EmbedBuilder updatedMessage;
            ulong? currentMessageId;

            switch (component.Data.CustomId)
            {
                case "presentButton":
                    currentMessageId = AttendanceHandler.GetCurrentMessageId();
                    if (currentMessageId == null)
                        await AttendanceMessageResponds.RespondWithOldSignupError(component);
                    else if (component.Message.Id == currentMessageId)
                    {
                        Logger.LogInformation($"{component.User.Username} clicked present on the signup");
                        await MemberHandler.UpdateMemberStatus(component.User, true);
                        updatedMessage = await AttendanceHandler.RefreshSignupMessageFields();
                        Logger.LogInformation($"Updating discord attendance message");
                        await component.UpdateAsync(attendanceMessage => attendanceMessage.Embed = updatedMessage.Build());
                        await AttendanceMessageResponds.RespondWithSignupStatus(component, true);
                    }
                    else
                        await AttendanceMessageResponds.RespondWithOldSignupError(component);
                    break;
                case "absentButton":
                    currentMessageId = AttendanceHandler.GetCurrentMessageId();
                    if (currentMessageId == null)
                        await AttendanceMessageResponds.RespondWithOldSignupError(component);
                    else if (component.Message.Id == currentMessageId)
                    {
                        Logger.LogInformation($"{component.User.Username} clicked absent on the signup");
                        await MemberHandler.UpdateMemberStatus(component.User, false);
                        updatedMessage = await AttendanceHandler.RefreshSignupMessageFields();
                        Logger.LogInformation($"Updating discord attendance message");
                        await component.UpdateAsync(attendanceMessage => attendanceMessage.Embed = updatedMessage.Build());
                        await AttendanceMessageResponds.RespondWithSignupStatus(component, false);
                    }
                    else
                        await AttendanceMessageResponds.RespondWithOldSignupError(component);
                    break;
            }
            //TODO button event
            // question buttons
        }
    }
}
