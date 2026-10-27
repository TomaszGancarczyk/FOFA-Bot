using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Data;
using FOFA_Bot.PlayerStats;

namespace FOFA_Bot.Bot
{
    internal class SlashCommandHandler
    {
        private static readonly ulong StatsChannelId = BotData.GetStatsChannelId();
        public static async Task Handle(SocketSlashCommand command)
        {
            Embed? embed;
            switch (command.Data.Name)
            {
                case "create-signup":
                    Logger.LogInformation($"[command] User {command.User.Username} used create-signup");
                    await command.DeferAsync(ephemeral: true);
                    if (!await CheckRofaPermission(command)) break;
                    embed = await SlashHandler.CreateSignupFromSlashCommand();
                    if (embed != null)
                        await command.FollowupAsync(embed: embed, ephemeral: true);
                    break;

                case "settings-automatic-rofa":
                    Logger.LogInformation($"[command] User {command.User.Username} used settings-automatic-rofa");
                    await command.DeferAsync(ephemeral: true);
                    if (!await CheckRofaPermission(command)) break;
                    embed = AttendanceHandler.ChangeRofaAutomnaticSettings(command.Data.Options);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed, ephemeral: true);
                    break;

                case "stats":
                    Logger.LogInformation($"[command] User {command.User.Username} used stats");
                    await command.DeferAsync();
                    if (command.ChannelId != StatsChannelId)
                    {
                        Logger.LogInformation($"    Wrong channel, responding with error message");
                        await command.RespondAsync(embed: GetStatsChannelErrorMessage().Build(), ephemeral: true);
                        break;
                    }
                    await StatsMessage.SendStatsMessage(command);
                    break;
            }
        }

        private static async Task<bool> CheckRofaPermission(SocketSlashCommand command)
        {
            string[] privilegedRoleNames = [];
            bool hasPermission = false;
            try
            {
                privilegedRoleNames = BotData.GetPrivilegedRoleNames();
            }
            catch (Exception ex)
            {
                Logger.LogError($"    Run into issue getting PrivilegedRoleNames:\n{ex}");
            }
            SocketGuildUser user = BotData.GetGuild().Users.First(user => user.Id == command.User.Id);
            foreach (SocketRole role in user.Roles) if (privilegedRoleNames.Contains(role.Name))
                {
                    hasPermission = true;
                    break;
                }
            if (!hasPermission)
            {
                Logger.LogWarning($"    User {command.User.Username} don't have permission to use {command.Data.Name}");
                await command.RespondAsync(embed: GetPermissionErrorMessage().Build(), ephemeral: true);
            }
            return hasPermission;
        }
        private static EmbedBuilder GetPermissionErrorMessage()
        {
            EmbedBuilder embed = new()
            {
                Color = Color.Red,
                Title = $"You don't have permission to use this command"
            };
            return embed;
        }
        private static EmbedBuilder GetStatsChannelErrorMessage()
        {
            EmbedBuilder embed = new()
            {
                Color = Color.Red,
                Title = $"Please use this command in {MentionUtils.MentionChannel(StatsChannelId)} channel"
            };
            return embed;
        }
    }
}
