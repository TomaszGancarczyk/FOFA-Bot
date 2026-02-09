using Discord.WebSocket;
using FOFA_Bot.Data;

namespace FOFA_Bot.Bot
{
    internal class MemberHandler
    {
        private static List<SocketGuildUser> DiscordMembers = [];
        private static List<Member> Members = [];

        internal static void CreateMembersList()
        {
            Logger.LogInformation($"Creating attendance Members");
            Members = [];
            CreateDiscordMembers();
            foreach (SocketGuildUser discordMember in DiscordMembers)
            {
                Member? member = CreateMember(discordMember, null);
                if (member == null) continue;
                if (member.discordUser == null) continue;
                Members.Add(member);
                Logger.LogInformation($"Added {member.discordUser.DisplayName} to member list to squad {member.squad}");
            }
        }
        internal static void UpdateMemberStatus(SocketUser user, bool status) //TODO member status in button
        {
            if (Members.Any(member => member.discordUser.Id == user.Id))
            {
                Member? discordUser = Members.FirstOrDefault(m => m.discordUser.Id == user.Id);
                if (discordUser != null)
                    if (discordUser.discordUser != null)
                    {
                        Logger.LogInformation($"Updating status for {discordUser.discordUser.DisplayName}: {status}");
                        discordUser.status = status;
                    }
            }
            else
            {
                Logger.LogInformation($"Cannot find {user.GlobalName}, adding new member to list");
                Member? member = CreateMember(user, status);
                if (member != null)
                    Members.Add(member);
            }
        }
        internal static List<Member> GetMembers()
        {
            return Members;
        }

        private static void CreateDiscordMembers()
        {
            Logger.LogInformation($"Creating Members from Discord");
            DiscordMembers = [];

            SocketGuild guild = BotData.GetGuild();
            string roleName = BotData.GetRofaRoleName();
            DiscordMembers = [.. guild.Users.Where(user => user.Roles.Any(role => role.Name == roleName))];
            for (int i = (DiscordMembers.Count - 1); i >= 0; i--) if (DiscordMembers[i].IsBot)
                {
                    Logger.LogWarning($"{DiscordMembers[i].DisplayName} is a bot, removing from the list");
                    DiscordMembers.RemoveAt(i);
                }
        }
        private static Member? CreateMember(SocketUser user, bool? status)
        {
            SocketGuild guild = BotData.GetGuild();
            SocketGuildUser? guildUser = guild.Users.FirstOrDefault(u => u.Id == user.Id);
            if (guildUser == null)
            {
                Logger.LogWarning($"Cannot find and create new user for {user.Username}, returning...");
                return null;
            }
            Member member = new()
            {
                discordUser = guildUser,
                squad = GetMemberSquad(guildUser),
                inGameName = null,
                status = status
            };
            return member;
        }
        private static int GetMemberSquad(SocketGuildUser user)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (user.Roles.Contains(GetRoleByName($"Rofa-Sq{i}")))
                    return i;
            }
            if (user.Roles.Contains(GetRoleByName("Rofa-Reserve")))
                return 7; //reserve
            return 0; //unassigned
        }
        private static SocketRole? GetRoleByName(string roleName)
        {
            SocketGuild guild = BotData.GetGuild();
            SocketRole? role = guild.Roles.FirstOrDefault(role => role.Name == roleName);
            if (role != null)
                return role;
            else
            {
                Logger.LogError($"Cannot get role with name {roleName}");
                return null;
            }
        }
    }
}