using Discord.WebSocket;

namespace FOFA_Bot.Bot
{
    internal class SlashCommandHandler
    {
        public static async Task Handle(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "create-signup-template":
                    Logger.LogInformation($"User {command.User.Username} used create-signup-template");
                    await command.DeferAsync(ephemeral: true);
                    //TODO slash command
                    break;
                case "create-signup-custom":
                    Logger.LogInformation($"User {command.User.Username} used create-signup-custom");
                    await command.DeferAsync(ephemeral: true);
                    //TODO slash command
                    break;
                case "automatic-signups-question":
                    Logger.LogInformation($"User {command.User.Username} used automatic-signups-question");
                    await command.DeferAsync(ephemeral: true);
                    //TODO slash command
                    break;
                case "automatic-signups-reminder":
                    Logger.LogInformation($"User {command.User.Username} used automatic-signups-reminder");
                    await command.DeferAsync(ephemeral: true);
                    //TODO slash command
                    break;
            }
        }
    }
}
