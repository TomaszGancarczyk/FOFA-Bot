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
                case "create-signup-template":
                    Logger.LogInformation($"[command] User {command.User.Username} used create-signup-template");
                    await command.DeferAsync(ephemeral: true);
                    if (!await CheckRofaPermission(command)) break;
                    if (command.Data.Options.Count > 0)
                        embed = await SlashHandler.CreateSignupFromSlashCommand((Int64)command.Data.Options.First().Value);
                    else embed = await SlashHandler.CreateSignupFromSlashCommand(10);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed, ephemeral: true);
                    break;

                case "create-signup-custom":
                    Logger.LogInformation($"[command] User {command.User.Username} used create-signup-custom");
                    await command.DeferAsync(ephemeral: true);
                    if (!await CheckRofaPermission(command)) break;
                    embed = SlashHandler.CreateSignupCustom((string)command.Data.Options.First().Value, (string)command.Data.Options.Last().Value);
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
