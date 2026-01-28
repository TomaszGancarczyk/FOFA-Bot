using Discord;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceMessage
    {
        internal readonly IMessageChannel signupsChannel = BotData.GetSignupsChannel();
        internal EmbedBuilder embedMessage = new();
        internal ComponentBuilder messageButtons = new();
        internal DateTime? Date;
        internal IMessage? discordMessage;
    }
}
