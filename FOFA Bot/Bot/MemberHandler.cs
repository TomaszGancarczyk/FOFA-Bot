using Discord;
using Discord.WebSocket;
using FOFA_Bot.Data;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FOFA_Bot.Bot
{
    internal class MemberHandler
    {
        internal async static Task<List<Member>> SetMembers()
        {
            //TODO
            List<Member> members = null;

            SocketGuild guild = BotData.GetGuild();
            string roleName = BotData.GetRofaRoleName();
            List<SocketGuildUser> guildMembers = [.. guild.Users.Where(user => user.Roles.Any(role => role.Name == roleName))];

            return members;
        }
        internal async static Task<List<Member>> GetMembers()
        {

        }
    }
}
