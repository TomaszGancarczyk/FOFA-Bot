using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;

namespace FOFA_Bot.Bot
{
    internal class ButtonEventHandler
    {
        public static async Task Handle(SocketMessageComponent component)
        {
            EmbedBuilder? updatedMessage;
            ulong? currentMessageId;
            switch (component.Data.CustomId)
            {
                case "tournamentButton":
                    Logger.LogInformation($"Got tournament response to event question");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Tournament");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "baseCaptureButton":
                    Logger.LogInformation($"Got base capture response to event question");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Base Capture");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "brawlButton":
                    Logger.LogInformation($"Got brawl response to event question");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Brawl");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "goldenDropButton":
                    Logger.LogInformation($"Got golden drop response to event question");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Golden Drop");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "stillwatersButton":
                    Logger.LogInformation($"Got stillwaters response to event question");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Stillwaters Chrono/Pulpe/Drops");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "dayOffButton":
                    Logger.LogInformation($"Got day off response to event question");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Day Off");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "presentButton":
                    currentMessageId = AttendanceHandler.GetCurrentMessageId();
                    if (currentMessageId == null)
                        await AttendanceMessageResponds.RespondWithOldSignupError(component);
                    else if (component.Message.Id == currentMessageId)
                    {
                        Logger.LogInformation($"{component.User.Username} clicked present on the signup");
                        MemberHandler.UpdateMemberStatus(component.User, true);
                        updatedMessage = AttendanceHandler.RefreshSignupMessage();
                        if (updatedMessage != null)
                        {
                            Logger.LogInformation($"Updating discord attendance message");
                            await component.UpdateAsync(attendanceMessage => attendanceMessage.Embed = updatedMessage.Build());
                            await AttendanceMessageResponds.RespondWithSignupStatus(component, true);
                        }
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
                        MemberHandler.UpdateMemberStatus(component.User, false);
                        updatedMessage = AttendanceHandler.RefreshSignupMessage();
                        if (updatedMessage != null)
                        {
                            Logger.LogInformation($"Updating discord attendance message");
                            await component.UpdateAsync(attendanceMessage => attendanceMessage.Embed = updatedMessage.Build());
                            await AttendanceMessageResponds.RespondWithSignupStatus(component, false);
                        }
                    }
                    else
                        await AttendanceMessageResponds.RespondWithOldSignupError(component);
                    break;
            }
        }
    }
}
