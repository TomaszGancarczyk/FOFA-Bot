using Discord.WebSocket;

namespace FOFA_Bot.Bot
{
    internal class SlashCommand
    {
        public static Task Handle(SocketSlashCommand command)
        {
            return Task.CompletedTask;
            //TODO slash command
            // auto nade toggle
            // auto signup toggle
            // create signup
            // custom signup
            // reminder toggle
        }
    }
}
