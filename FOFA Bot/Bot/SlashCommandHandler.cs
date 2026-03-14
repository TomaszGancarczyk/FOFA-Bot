using Discord;
using Discord.WebSocket;
using FOFA_Bot.Attendance;
using FOFA_Bot.Data;
using FOFA_Bot.PlayerStats;

namespace FOFA_Bot.Bot
{
    internal class SlashCommandHandler
    {
        private readonly static ulong StatsChannelId = BotData.GetStatsChannelId();
        public static async Task Handle(SocketSlashCommand command)
        {
            EmbedBuilder? embed;
            switch (command.Data.Name)
            {
                case "create-signup-template":
                    Logger.LogInformation($"[command] User {command.User.Username} used create-signup-template");
                    await command.DeferAsync(ephemeral: true);
                    if (!await CheckRofaPermission(command)) break;
                    if (command.Data.Options.Count > 0)
                        embed = SlashAttendanceHandler.CreateSignupTemplate((Int64)command.Data.Options.First().Value);
                    else embed = SlashAttendanceHandler.CreateSignupTemplate(10);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;

                case "create-signup-custom":
                    Logger.LogInformation($"[command] User {command.User.Username} used create-signup-custom");
                    await command.DeferAsync(ephemeral: true);
                    if (!await CheckRofaPermission(command)) break;
                    embed = SlashAttendanceHandler.CreateSignupCustom((string)command.Data.Options.First().Value, (string)command.Data.Options.Last().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;

                case "automatic-signups-question":
                    Logger.LogInformation($"[command] User {command.User.Username} used automatic-signups-question");
                    await command.DeferAsync(ephemeral: true);
                    if (!await CheckRofaPermission(command)) break;
                    embed = BotHandler.ChangeAutomnaticSignupMessage((bool)command.Data.Options.First().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
                    break;

                case "automatic-signups-reminder":
                    Logger.LogInformation($"[command] User {command.User.Username} used automatic-signups-reminder");
                    await command.DeferAsync(ephemeral: true);
                    if (!await CheckRofaPermission(command)) break;
                    embed = AttendanceHandler.ChangeAutomaticReminder((bool)command.Data.Options.First().Value);
                    if (embed != null)
                        await command.FollowupAsync(embed: embed.Build(), ephemeral: true);
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

        private async static Task<bool> CheckRofaPermission(SocketSlashCommand command)
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
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red);
            embed.WithTitle($"You don't have permission to use this command");
            return embed;
        }
        private static EmbedBuilder GetStatsChannelErrorMessage()
        {
            EmbedBuilder embed = new();
            embed.WithColor(Color.Red);
            embed.WithTitle($"Please use this command in {MentionUtils.MentionChannel(StatsChannelId)} channel");
            return embed;
        }
    }
}
