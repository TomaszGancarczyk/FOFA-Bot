using Discord;
using FOFA_Bot.Data;

namespace FOFA_Bot.Attendance
{
    internal class AttendanceMessage
    {
        readonly IMessageChannel messageChannel = BotData.GetSignupsChannel();
        internal EmbedBuilder embedMessage;
        internal ComponentBuilder messageButtons;
        internal IMessage discordMessage;
        internal string? Name;
        internal DateTime? Date;
    }
}
