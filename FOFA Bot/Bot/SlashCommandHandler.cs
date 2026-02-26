using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Data;

namespace FOFA_Bot.Bot
{
    internal class SlashCommandHandler
    {
        public static async Task Handle(SocketSlashCommand command)
        {
            try
            {
                bool hasPermission = CheckForPermission(command.User.Id);
                if (!hasPermission)
                {
                    Logger.LogWarning($"User {command.User.Username} don't have permission to use {command.Data.Name}");
                    await command.RespondAsync(embed: GetPermissionErrorMessage().Build(), ephemeral: true);
                    return;
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            EmbedBuilder? embed;
            switch (command.Data.Name)
            {
                case "create-signup-template":
                    Logger.LogInformation($"User {command.User.Username} used create-signup-template");
                    await command.DeferAsync(ephemeral: true);
                    embed = await SlashAttendanceHandler.CreateSignupTemplate((Int64)command.Data.Options.First().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;
                case "create-signup-custom":
                    Logger.LogInformation($"User {command.User.Username} used create-signup-custom");
                    await command.DeferAsync(ephemeral: true);
                    embed = await SlashAttendanceHandler.CreateSignupCustom((string)command.Data.Options.First().Value, (string)command.Data.Options.Last().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;
                case "automatic-signups-question":
                    Logger.LogInformation($"User {command.User.Username} used automatic-signups-question");
                    await command.DeferAsync(ephemeral: true);
                    embed = BotHandler.ChangeAutomnaticSignupMessage((bool)command.Data.Options.First().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;
                case "automatic-signups-reminder":
                    Logger.LogInformation($"User {command.User.Username} used automatic-signups-reminder");
                    await command.DeferAsync(ephemeral: true);
                    embed = AttendanceHandler.ChangeAutomaticReminder((bool)command.Data.Options.First().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;
            }
        }

        private static bool CheckForPermission(ulong userId)
        {
            bool hasPermission = false;
            string[] privilegedRoleNames = BotData.GetPrivilegedRoleNames();
            SocketGuildUser user = BotData.GetGuild().Users.First(user => user.Id == userId);
            foreach (SocketRole role in user.Roles) if (privilegedRoleNames.Contains(role.Name))
                {
                    hasPermission = true;
                    break;
                }
            return hasPermission;
        }
        private static EmbedBuilder GetPermissionErrorMessage()
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red);
            embed.WithTitle($"You don't have permission to use this command");
            return embed;
        }
    }
}
