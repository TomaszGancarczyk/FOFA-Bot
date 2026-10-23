using Discord;
using Discord.Net;
using Discord.WebSocket;
using FOFA_Bot.Attendance;

namespace FOFA_Bot.Bot
{
    internal class ButtonEventHandler
    {
        private static Embed CreatePositiveEmbed(DayOfWeek dayOfWeek)
        {
            return new EmbedBuilder
            {
                Color = Color.Green,
                Title = $"Registered for event on {dayOfWeek}"
            }.Build();
        }
        private static Embed CreateNegativeEmbed(DayOfWeek dayOfWeek)
        {
            return new EmbedBuilder
            {
                Color = Color.Red,
                Title = $"Unregistered for event on {dayOfWeek}"
            }.Build();
        }
        private static List<string> signupActionNames = [];
        public static async Task Handle(SocketMessageComponent component)
        {
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

                case "presentButton_Thursday":
                    signupActionNames.Add(component.User.GlobalName);
                    await Task.WhenAny(
                        HandleAttendanceButton(component, true, 0),
                        HandleTimeoutDelay(TimeSpan.FromSeconds(5), component.User.GlobalName)
                        );
                    break;
                case "absentButton_Thursday":
                    await Task.WhenAny(
                        HandleAttendanceButton(component, false, 0),
                        HandleTimeoutDelay(TimeSpan.FromSeconds(5), component.User.GlobalName)
                        );
                    break;
                case "presentButton_Friday":
                    signupActionNames.Add(component.User.GlobalName);
                    await Task.WhenAny(
                        HandleAttendanceButton(component, true, 1),
                        HandleTimeoutDelay(TimeSpan.FromSeconds(5), component.User.GlobalName)
                        );
                    break;
                case "absentButton_Friday":
                    await Task.WhenAny(
                        HandleAttendanceButton(component, false, 1),
                        HandleTimeoutDelay(TimeSpan.FromSeconds(5), component.User.GlobalName)
                        );
                    break;
                case "presentButton_Saturday":
                    signupActionNames.Add(component.User.GlobalName);
                    await Task.WhenAny(
                        HandleAttendanceButton(component, true, 2),
                        HandleTimeoutDelay(TimeSpan.FromSeconds(5), component.User.GlobalName)
                        );
                    break;
                case "absentButton_Saturday":
                    await Task.WhenAny(
                        HandleAttendanceButton(component, false, 2),
                        HandleTimeoutDelay(TimeSpan.FromSeconds(5), component.User.GlobalName)
                        );
                    break;
                case "presentButton_Sunday":
                    signupActionNames.Add(component.User.GlobalName);
                    await Task.WhenAny(
                        HandleAttendanceButton(component, true, 3),
                        HandleTimeoutDelay(TimeSpan.FromSeconds(5), component.User.GlobalName)
                        );
                    break;
                case "absentButton_Sunday":
                    await Task.WhenAny(
                        HandleAttendanceButton(component, false, 3),
                        HandleTimeoutDelay(TimeSpan.FromSeconds(5), component.User.GlobalName)
                        );
                    break;
            }
        }
        private static async Task HandleAttendanceButton(SocketMessageComponent component, bool status, int dayOfWeek)
        {
            DayOfWeek signupDayOfWeek = DayOfWeek.Thursday + dayOfWeek;
            DateTime closedCheckTime = new
            (
                DateTime.Today.Year,
                DateTime.Today.Month,
                DateTime.Today.Day,
                19,
                30,
                0
            );
            if (dayOfWeek == 3)
            {
                signupDayOfWeek = DayOfWeek.Sunday;
                if (signupDayOfWeek == closedCheckTime.DayOfWeek && DateTime.Now > closedCheckTime.AddHours(-1))
                {
                    await RespondWithOldSignupError(component);
                    return;
                }
            }
            if (signupDayOfWeek != DayOfWeek.Sunday && signupDayOfWeek < DateTime.Now.DayOfWeek && DateTime.Now > closedCheckTime)
            {
                await RespondWithOldSignupError(component);
                return;
            }
            if (signupDayOfWeek == closedCheckTime.DayOfWeek && DateTime.Now.Hour > 19)
                Logger.LogInformation($"[button] {component.User.Username} clicked {status} on the signup for {signupDayOfWeek}");
            List<ulong?>? currentMessageIds = AttendanceHandler.GetCurrentMessagesIds();
            if (currentMessageIds != null && currentMessageIds.Contains(component.Message.Id))
            {
                MemberHandler.UpdateMemberStatus(component.User, status, dayOfWeek);
                EmbedBuilder? updatedMessage = AttendanceHandler.RefreshSignupMessage(component.Message.Id);
                if (updatedMessage == null)
                {
                    Logger.LogWarning($"    Run into error when updating attendance message:\nupdatedMessage == null");
                    return;
                }
                try
                {
                    Logger.LogInformation($"    Updating discord attendance message");
                    await component.UpdateAsync(attendanceMessage => attendanceMessage.Embed = updatedMessage.Build());
                    signupActionNames.Remove(component.User.GlobalName);
                    Logger.LogInformation($"    Attendance message updated");
                }
                catch (HttpException e)
                {
                    if (e.DiscordCode == DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged)
                    {
                        Logger.LogWarning($"    Run into error when updating attendance message:\nInteractionHasAlreadyBeenAcknowledged");
                        return;
                    }
                    Logger.LogError($"    Run into error when updating attendance message:\n{e}");
                }
                if (status)
                    await component.FollowupAsync(embed: CreatePositiveEmbed(signupDayOfWeek), ephemeral: true);
                else
                    await component.FollowupAsync(embed: CreateNegativeEmbed(signupDayOfWeek), ephemeral: true);
            }
            else
                await RespondWithOldSignupError(component);

        }
        private static async Task HandleTimeoutDelay(TimeSpan timeout, string nameId)
        {
            await Task.Delay(timeout);
            if (signupActionNames.Contains(nameId))
            {
                signupActionNames.Remove(nameId);
                Logger.LogWarning($"    Timeout reached for {nameId}");
            }
        }
        private static async Task RespondWithOldSignupError(SocketMessageComponent component)
        {
            Logger.LogWarning($"    {component.User.Username} interacted with old signup");
            EmbedBuilder embed = new()
            {
                Color = Color.DarkerGrey,
                Title = $"This is signup is closed"
            };
            await component.RespondAsync(embed: embed.Build(), ephemeral: true);
        }
    }
}
