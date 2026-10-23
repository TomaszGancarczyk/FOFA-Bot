using Discord.WebSocket;

namespace FOFA_Bot.Bot
{
    internal class Member
    {
        internal SocketGuildUser? discordUser;
        internal string? inGameName;
        internal int? squad;
        internal bool?[] status = [null, null, null, null];
        internal int priority = 0;
        internal bool squadleader = false;
    }
}
