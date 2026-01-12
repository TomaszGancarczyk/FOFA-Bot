using Discord;
using Discord.WebSocket;

namespace FOFA_Bot.Bot
{
    internal class Member
    {
        private SocketGuildUser discordUser;
        private string inGameName;
        private bool? status;
    }
}
