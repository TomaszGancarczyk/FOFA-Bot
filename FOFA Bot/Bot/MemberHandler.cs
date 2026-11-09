using Discord.WebSocket;
using FOFA_Bot.Data;

namespace FOFA_Bot.Bot
{
    internal class MemberHandler
    {
        private static List<Member> RofaMembers = [];
        private static List<Member> BofaMembers = [];
        private static List<PlannerData> PlannerDatas = [];

        internal static void CreateMembersList(string clan)
        {
            Logger.LogInformation($"    Creating attendance Members");
            List<Member> tempMembers = [];
            List<SocketGuildUser> discordMembers = CreateDiscordMembers(clan);
            foreach (SocketGuildUser discordMember in discordMembers)
            {
                Member? member = CreateMember(discordMember, true, clan);
                if (member == null || member.discordUser == null) continue;
                tempMembers.Add(member);
                Logger.LogInformation($"    Added {member.discordUser.Username} to member list to squad {member.squad}");
            }
            if (clan == "ROFA")
                RofaMembers = [.. tempMembers.OrderByDescending(p => p.priority)];
            else if (clan == "BOFA")
                BofaMembers = [.. tempMembers.OrderByDescending(p => p.priority)];
        }
        internal static void UpdateMemberStatus(SocketUser user, bool status, int dayOfWeek, string clan)
        {
            if (clan == "ROFA" && RofaMembers.Any(member => member.discordUser.Id == user.Id))
            {
                Member? discordUser = RofaMembers.First(member => member.discordUser.Id == user.Id);
                if (discordUser != null && discordUser.discordUser != null)
                {
                    Logger.LogInformation($"    Updating status for {discordUser.discordUser.Username}: {status}");
                    discordUser.status[dayOfWeek] = status;
                }
            }
            else if (clan == "BOFA" && BofaMembers.Any(member => member.discordUser.Id == user.Id))
            {
                Member? discordUser = BofaMembers.First(member => member.discordUser.Id == user.Id);
                if (discordUser != null && discordUser.discordUser != null)
                {
                    Logger.LogInformation($"    Updating status for {discordUser.discordUser.Username}: {status}");
                    discordUser.status[dayOfWeek] = status;
                }
            }
            else
            {
                Logger.LogInformation($"    Cannot find {user.Username}, adding new member to list");
                Member? member = CreateMember(user, false, clan);
                if (member != null && clan == "ROFA")
                {
                    member.status[dayOfWeek] = status;
                    RofaMembers.Add(member);
                }
                else if (member != null && clan == "BOFA")
                {
                    member.status[dayOfWeek] = status;
                    BofaMembers.Add(member);
                }
            }
        }
        internal static List<Member>? GetMembers(string clan)
        {
            if (clan == "ROFA")
                return RofaMembers;
            else if (clan == "BOFA")
                return BofaMembers;
            return null;
        }
        private static List<SocketGuildUser> CreateDiscordMembers(string clan)
        {
            Logger.LogInformation($"    Creating Members from Discord");
            List<SocketGuildUser> discordMembers = [];
            PlannerDatas = PlannerGoogleSheet.GetPlannerData();
            SocketGuild guild = BotData.GetGuild();
            string roleName = "";
            if (clan == "ROFA")
                roleName = BotData.GetROFARoleName();
            else if (clan == "BOFA")
                roleName = BotData.GetBOFARoleName();
            discordMembers = [.. guild.Users.Where(user => user.Roles.Any(role => role.Name == roleName))];
            for (int i = (discordMembers.Count - 1); i >= 0; i--) if (discordMembers[i].IsBot)
                    discordMembers.RemoveAt(i);
            return discordMembers;
        }

        internal static void RefreshMemberSquads(string clan)
        {
            Logger.LogInformation($"    Refreshing members");
            if (clan == "ROFA") foreach (var member in RofaMembers)
                {
                    member.squad = GetMemberSquad(member.discordUser, false, "ROFA");
                }
            if (clan == "BOFA") foreach (var member in BofaMembers)
                {
                    member.squad = GetMemberSquad(member.discordUser, false, "BOFA");
                }
        }

        private static Member? CreateMember(SocketUser user, bool skipUnassigned, string clan)
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
            int? squad = GetMemberSquad(guildUser, skipUnassigned, clan);
            if (squad == null || squad == 0) return null;
            Member member = new()
            {
                discordUser = guildUser,
                squad = squad,
                inGameName = inGameName,
                status = [null, null, null, null],
                priority = priority,
                squadleader = squadleader
            };
            return member;
        }
        private static int? GetMemberSquad(SocketGuildUser user, bool skipUnassigned, string clan)
        {
            if (clan == "ROFA")
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
            else if (clan == "BOFA")
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (user.Roles.Any(id => id.Id == GetRoleByName($"BOFA Squad {i}").Id))
                        return i;
                }
                if (user.Roles.Any(id => id.Id == GetRoleByName("BOFA Reserve").Id))
                    return 7; //reserve
                else if (!skipUnassigned) return 8; //unassigned
                return 0; //skip
            }
            return null;
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

        internal static void UpdateBackupMembers(Dictionary<ulong, bool?[]> backupMembers, string clan)
        {
            if (clan == "ROFA")
            {
                foreach (Member member in RofaMembers)
                {
                    foreach (KeyValuePair<ulong, bool?[]> backupMember in backupMembers)
                    {
                        if (member.discordUser.Id == backupMember.Key)
                            member.status = backupMember.Value;
                    }
                }
            }
            else if (clan == "BOFA")
            {
                foreach (Member member in BofaMembers)
                {
                    foreach (KeyValuePair<ulong, bool?[]> backupMember in backupMembers)
                    {
                        if (member.discordUser.Id == backupMember.Key)
                            member.status = backupMember.Value;
                    }
                }
            }
        }
    }
}