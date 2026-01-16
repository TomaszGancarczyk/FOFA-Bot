using Discord.WebSocket;
using FOFA_Bot.Data;

namespace FOFA_Bot.Bot
{
    internal class MemberHandler
    {
        private static List<SocketGuildUser> DiscordMembers;
        private static List<Member> Members;

        internal async static Task CreateMembersList()
        {
            Logger.LogInformation($"Creating attendance Members");
            Members = [];
            await CreateDiscordMembers();
            foreach (SocketGuildUser discordMember in DiscordMembers)
            {
                Member member = await CreateMember(discordMember, null);
                Members.Add(member);
                Logger.LogInformation($"Added {member.discordUser.DisplayName} to member list to squad {member.squad}");
            }
        }
        internal async static Task UpdateMemberStatus(SocketGuildUser user, bool status) //TODO member status in button
        {
            Logger.LogInformation($"Updating status for {user.DisplayName}");
            if (Members.Any(member => member.discordUser == user))
            {
                Logger.LogInformation($"Found {user.DisplayName}, updating status");
                Members.FirstOrDefault(member => member.discordUser == user).status = status;
            }
            else
            {
                Logger.LogInformation($"Cannot find {user.DisplayName}, adding new member to list");
                Member member = await CreateMember(user, status);
                Members.Add(member);
            }
        }
        internal async static Task<List<Member>> GetMembers()
        {
            return Members;
        }



        private async static Task CreateDiscordMembers()
        {
            Logger.LogInformation($"Creating Members from Discord");
            DiscordMembers = [];

            SocketGuild guild = BotData.GetGuild();
            string roleName = BotData.GetRofaRoleName();
            DiscordMembers = [.. guild.Users.Where(user => user.Roles.Any(role => role.Name == roleName))];
            for (int i = (DiscordMembers.Count - 1); i>=0; i--) if (DiscordMembers[i].IsBot)
                {
                    Logger.LogWarning($"{DiscordMembers[i].DisplayName} is a bot, removing from the list");
                    DiscordMembers.RemoveAt(i);
                }
        }
        private async static Task<Member> CreateMember(SocketGuildUser user, bool? status)
        {
            Member member = new()
            {
                discordUser = user,
                squad = await GetMemberSquad(user),
                inGameName = null, //TODO member in game name
                status = status
            };
            return member;
        }
        private async static Task<int> GetMemberSquad(SocketGuildUser user)
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