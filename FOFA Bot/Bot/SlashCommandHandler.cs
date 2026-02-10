using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;

namespace FOFA_Bot.Bot
{
    internal class SlashCommandHandler
    {
        public static async Task Handle(SocketSlashCommand command)
        {
            EmbedBuilder? embed;
            switch (command.Data.Name)
            {
                case "create-signup-template":
                    Logger.LogInformation($"User {command.User.Username} used create-signup-template");
                    embed = null;
                    await command.DeferAsync(ephemeral: true);
                    embed = await SlashAttendanceHandler.CreateSignupTemplate((Int64)command.Data.Options.First().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;
                case "create-signup-custom":
                    Logger.LogInformation($"User {command.User.Username} used create-signup-custom");
                    embed = null;
                    await command.DeferAsync(ephemeral: true);
                    embed = await SlashAttendanceHandler.CreateSignupCustom((string)command.Data.Options.First().Value, (string)command.Data.Options.Last().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;
                case "automatic-signups-question":
                    Logger.LogInformation($"User {command.User.Username} used automatic-signups-question");
                    embed = null;
                    await command.DeferAsync(ephemeral: true);
                    embed = BotHandler.ChangeAutomnaticSignupMessage((bool)command.Data.Options.First().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;
                case "automatic-signups-reminder":
                    Logger.LogInformation($"User {command.User.Username} used automatic-signups-reminder");
                    embed = null;
                    await command.DeferAsync(ephemeral: true);
                    embed = AttendanceHandler.ChangeAutomaticReminder((bool)command.Data.Options.First().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;
            }
        }
    }
}
