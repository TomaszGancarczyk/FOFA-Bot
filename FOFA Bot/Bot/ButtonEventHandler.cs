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
            DateTime closedCheckTime = new
            (
                DateTime.Today.Year,
                DateTime.Today.Month,
                DateTime.Today.Day,
                18,
                30,
                0
            );
            DayOfWeek signupDayOfWeek = DayOfWeek.Thursday + dayOfWeek;
            if (dayOfWeek == 3)
            {
                signupDayOfWeek = DayOfWeek.Sunday;

                Console.WriteLine(signupDayOfWeek);
                Console.WriteLine(closedCheckTime.DayOfWeek);
                Console.WriteLine(DateTime.Now.TimeOfDay);
                Console.WriteLine(closedCheckTime.AddHours(-1).TimeOfDay);

                if (signupDayOfWeek == closedCheckTime.DayOfWeek && DateTime.Now.TimeOfDay > closedCheckTime.AddHours(-1).TimeOfDay)
                {
                    await RespondWithOldSignupError(component);
                    return;
                }
            }
            else if (signupDayOfWeek < DateTime.Now.DayOfWeek && DateTime.Now > closedCheckTime)
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
