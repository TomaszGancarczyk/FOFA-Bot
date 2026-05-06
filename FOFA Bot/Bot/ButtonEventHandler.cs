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
            List<ulong?>? currentMessageIds;
            switch (component.Data.CustomId)
            {
                case "tournamentButton":
                    Logger.LogInformation($"    Got tournament response to event question from {component.User.Username}");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Tournament");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "baseCaptureButton":
                    Logger.LogInformation($"    Got base capture response to event question from {component.User.Username}");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Base Capture");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "brawlButton":
                    Logger.LogInformation($"    Got brawl response to event question from {component.User.Username}");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Brawl");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "goldenDropButton":
                    Logger.LogInformation($"    Got golden drop response to event question from {component.User.Username}");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Golden Drop");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "stillwatersButton":
                    Logger.LogInformation($"    Got stillwaters response to event question from {component.User.Username}");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Wild North");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "dayOffButton":
                    Logger.LogInformation($"    Got day off response to event question from {component.User.Username}");
                    AttendanceQuestion.SetQuestionAnswear(component.Message.Id, "Day Off");
                    component.Message.DeleteAsync().Wait();
                    break;
                case "presentButton":
                    currentMessageIds = AttendanceHandler.GetCurrentMessagesIds();
                    if (currentMessageIds == null)
                        await MessageResponse.RespondWithOldSignupError(component);
                    else if (currentMessageIds.Contains(component.Message.Id))
                    {
                        Logger.LogInformation($"[button] {component.User.Username} clicked present on the signup");
                        MemberHandler.UpdateMemberStatus(component.User, true);
                        updatedMessage = AttendanceHandler.RefreshSignupMessage(component.Message.Id);
                        if (updatedMessage != null)
                        {
                            Logger.LogInformation($"    Updating discord attendance message");
                            await component.UpdateAsync(attendanceMessage => attendanceMessage.Embed = updatedMessage.Build());
                            await MessageResponse.RespondWithSignupStatus(component, true);
                        }
                    }
                    else
                        await MessageResponse.RespondWithOldSignupError(component);
                    break;
                case "absentButton":
                    currentMessageIds = AttendanceHandler.GetCurrentMessagesIds();
                    if (currentMessageIds == null)
                        await MessageResponse.RespondWithOldSignupError(component);
                    else if (currentMessageIds.Contains(component.Message.Id))
                    {
                        Logger.LogInformation($"[button] {component.User.Username} clicked absent on the signup");
                        MemberHandler.UpdateMemberStatus(component.User, false);
                        updatedMessage = AttendanceHandler.RefreshSignupMessage(component.Message.Id);
                        if (updatedMessage != null)
                        {
                            Logger.LogInformation($"    Updating discord attendance message");
                            await component.UpdateAsync(attendanceMessage => attendanceMessage.Embed = updatedMessage.Build());
                            await MessageResponse.RespondWithSignupStatus(component, false);
                        }
                    }
                    else
                        await MessageResponse.RespondWithOldSignupError(component);
                    break;
            }
        }
    }
}
