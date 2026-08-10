using Discord;
using Discord.Net;
using Discord.WebSocket;
using FOFA_Bot.Attendance;

namespace FOFA_Bot.Bot
{
    internal class ButtonEventHandler
    {
        private static readonly Embed PositiveEmbed = new EmbedBuilder
        {
            Color = Color.Green,
            Title = "Registered for event"
        }.Build();
        private static readonly Embed NegativeEmbed = new EmbedBuilder
        {
            Color = Color.Red,
            Title = "Unregistered for event"
        }.Build();
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
                case "presentButton":
                    await Task.WhenAny(
                        HandleAttendanceButton(component, true),
                        Task.Delay(TimeSpan.FromSeconds(5))
                        );
                    break;
                case "absentButton":
                    await Task.WhenAny(
                        HandleAttendanceButton(component, false),
                        Task.Delay(TimeSpan.FromSeconds(5))
                        );
                    break;
            }
        }
        private static async Task HandleAttendanceButton(SocketMessageComponent component, bool status)
        {
            Logger.LogInformation($"[button] {component.User.Username} clicked {status} on the signup");
            List<ulong?>? currentMessageIds = AttendanceHandler.GetCurrentMessagesIds();
            if (currentMessageIds != null && currentMessageIds.Contains(component.Message.Id))
            {
                MemberHandler.UpdateMemberStatus(component.User, status);
                EmbedBuilder? updatedMessage = AttendanceHandler.RefreshSignupMessage(component.Message.Id);
                try
                {
                    Logger.LogInformation($"    Updating discord attendance message");
                    await component.UpdateAsync(attendanceMessage => attendanceMessage.Embed = updatedMessage.Build());
                    Logger.LogInformation($"    Attendance message updated");
                }
                catch (HttpException e)
                {
                    if (e.DiscordCode == DiscordErrorCode.InteractionHasAlreadyBeenAcknowledged) return;
                    Logger.LogError($"    Run into error when updating attendance message:\n{e}");
                }
                if (status)
                    await component.FollowupAsync(embed: PositiveEmbed, ephemeral: true);
                else
                    await component.FollowupAsync(embed: NegativeEmbed, ephemeral: true);
            }
            else
                await RespondWithOldSignupError(component);

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
