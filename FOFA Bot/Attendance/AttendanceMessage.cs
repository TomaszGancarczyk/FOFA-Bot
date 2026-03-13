using Discord;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceMessage
    {
        internal readonly IMessageChannel signupsChannel = BotData.GetSignupsChannel();
        internal EmbedBuilder embedMessage = new();
        internal ComponentBuilder messageButtons = new();
        internal DateTime Date;
        internal IMessage? discordMessage;
        internal bool Reminder = false;
    }
    internal class AttendanceMessageBackup
    {
        public DateTime Date;
        public ulong discordMessageId;
        public bool Reminder;
    }
}
