using Discord;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceMessage
    {
        internal readonly IMessageChannel SignupsChannel = BotData.GetSignupsChannel();
        internal EmbedBuilder EmbedMessage = new();
        internal ComponentBuilder MessageButtons = new();
        internal DateTime Date;
        internal IMessage? DiscordMessage;
        internal bool Reminder = false;
    }
    internal class AttendanceMessageBackup
    {
        public DateTime Date;
        public ulong DiscordMessageId;
        public bool Reminder;
        public Dictionary<ulong, bool?> Members;
    }
}
