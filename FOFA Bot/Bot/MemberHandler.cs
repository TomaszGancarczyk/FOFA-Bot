using Discord.WebSocket;
using FOFA_Bot.Data;

namespace FOFA_Bot.Bot
{
    internal class MemberHandler
    {
        private static List<SocketGuildUser> DiscordMembers = [];
        private static List<Member> Members = [];
        private static List<PlannerData> PlannerDatas = [];

        internal static void CreateMembersList()
        {
            Logger.LogInformation($"    Creating attendance Members");
            List<Member> tempMembers = [];
            CreateDiscordMembers();
            foreach (SocketGuildUser discordMember in DiscordMembers)
            {
                Member? member = CreateMember(discordMember, null, true);
                if (member == null || member.discordUser == null) continue;
                tempMembers.Add(member);
                Logger.LogInformation($"    Added {member.discordUser.Username} to member list to squad {member.squad}");
            }
            Members = [.. tempMembers.OrderByDescending(p => p.priority)];
        }
        internal static void UpdateMemberStatus(SocketUser user, bool status)
        {
            if (Members.Any(member => member.discordUser.Id == user.Id))
            {
                Member? discordUser = Members.First(member => member.discordUser.Id == user.Id);
                if (discordUser != null)
                    if (discordUser.discordUser != null)
                    {
                        Logger.LogInformation($"    Updating status for {discordUser.discordUser.Username}: {status}");
                        discordUser.status = status;
                    }
            }
            else
            {
                Logger.LogInformation($"    Cannot find {user.Username}, adding new member to list");
                Member? member = CreateMember(user, status, false);
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
            Logger.LogInformation($"    Creating Members from Discord");
            DiscordMembers = [];
            PlannerDatas = PlannerGoogleSheet.GetPlannerData();
            SocketGuild guild = BotData.GetGuild();
            string roleName = BotData.GetRofaRoleName();
            DiscordMembers = [.. guild.Users.Where(user => user.Roles.Any(role => role.Name == roleName))];
            for (int i = (DiscordMembers.Count - 1); i >= 0; i--) if (DiscordMembers[i].IsBot)
                {
                    Logger.LogWarning($"    {DiscordMembers[i].Username} is a bot, removing from the list");
                    DiscordMembers.RemoveAt(i);
                }
        }

        internal static void RefreshMemberSquads()
        {
            Logger.LogInformation($"    Refreshing members");
            foreach (var member in Members)
            {
                int? tempSquad = member.squad;
                if (member.discordUser != null)
                {
                    member.squad = GetMemberSquad(member.discordUser, false);
                    if (member.squad != tempSquad) Logger.LogInformation($"      {member.discordUser.Username} changed squad from {tempSquad} to {member.squad}");
                }
            }

        }

        private static Member? CreateMember(SocketUser user, bool? status, bool skipUnassigned)
        {
            SocketGuild guild = BotData.GetGuild();
            SocketGuildUser? guildUser = guild.Users.FirstOrDefault(u => u.Id == user.Id);
            if (guildUser == null)
            {
                Logger.LogWarning($"    Cannot find and create new user for {user.Username}, returning...");
                return null;
            }
            string? inGameName = null;
            int priority = 0;
            bool squadleader = false;
            if (PlannerDatas.Any(name => name.discordName == guildUser.Username))
            {
                PlannerData data = PlannerDatas.First(name => name.discordName == guildUser.Username);
                inGameName = data.inGameName;
                priority = data.priority;
                squadleader = data.squadleader;
                Logger.LogInformation($"      Got {inGameName} name for {guildUser.Username}, priority {priority}, squadleader {squadleader}");
            }
            int squad = GetMemberSquad(guildUser, skipUnassigned);
            if (squad == 0) return null;
            Member member = new()
            {
                discordUser = guildUser,
                squad = squad,
                inGameName = inGameName,
                status = status,
                priority = priority,
                squadleader = squadleader
            };
            return member;
        }
        private static int GetMemberSquad(SocketGuildUser user, bool skipUnassigned)
        {
            for (int i = 1; i <= 6; i++)
            {
                if (user.Roles.Any(id => id.Id == GetRoleByName($"Rofa-Sq{i}").Id))
                    return i;
            }
            if (user.Roles.Any(id => id.Id == GetRoleByName("Rofa-Reserve").Id))
                return 7; //reserve
            else if (!skipUnassigned) return 8; //unassigned
            return 0; //skip
        }
        private static SocketRole? GetRoleByName(string roleName)
        {
            SocketGuild guild = BotData.GetGuild();
            SocketRole? role = guild.Roles.FirstOrDefault(role => role.Name == roleName);
            if (role != null)
                return role;
            else
            {
                Logger.LogError($"    Cannot get role with name {roleName}");
                return null;
            }
        }

        internal static void UpdateBackupMembers(Dictionary<ulong, bool?> backupMembers)
        {
            foreach (Member member in Members)
            {
                foreach (KeyValuePair<ulong, bool?> backupMember in backupMembers)
                {
                    if (member.discordUser.Id == backupMember.Key)
                        member.status = backupMember.Value;
                }
            }
        }
    }
}