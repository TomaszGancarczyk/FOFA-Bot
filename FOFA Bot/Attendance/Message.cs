using Discord;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class Message
    {
        internal IMessageChannel? SignupsChannel;
        internal EmbedBuilder EmbedMessage = new();
        internal ComponentBuilderV2? MessageButtons;
        internal DateTime ThursdayDate;
        internal IMessage? DiscordMessage;
        internal string? Clan;
    }
    internal class AttendanceMessageBackup
    {
        public DateTime ThursdayDate;
        public ulong DiscordMessageId;
        public Dictionary<ulong, bool?[]> Members = [];
        public string? Clan;
    }
}
