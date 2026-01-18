using Discord;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceMessage
    {
        internal readonly IMessageChannel signupsChannel = BotData.GetSignupsChannel();
        internal EmbedBuilder embedMessage;
        internal ComponentBuilder messageButtons;
        internal DateTime? Date;
        internal IMessage? discordMessage;
    }
}
