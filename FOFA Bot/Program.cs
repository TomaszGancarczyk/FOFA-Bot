using Discord;
using Discord.WebSocket;
using FOFA_Bot.Bot;
using FOFA_Bot.Data;

namespace FOFA_Bot
{
    internal class Program
    {
        private readonly DiscordSocketClient Discord;
        private static string? Token;
        static async Task Main()
        {
            Logger.LogInformation($"Starting...");
            BotData.LoadJson();
            Token = BotData.GetDiscordToken();
            Logger.LogInformation($"[FOFA] Bot is starting");
            new Program().StartBotAsync().GetAwaiter().GetResult();
            await Task.CompletedTask;
        }
        private Program()
        {
            DiscordSocketConfig? config = new()
            {
                AlwaysDownloadUsers = true,
                MessageCacheSize = 100,
                GatewayIntents = GatewayIntents.All,
                UseInteractionSnowflakeDate = false
            };
            Discord = new DiscordSocketClient(config);
        }
        internal async Task StartBotAsync()
        {
            await Discord.LoginAsync(TokenType.Bot, Token);
            await Discord.StartAsync();
            Discord.Ready += () =>
            {
                Discord.ButtonExecuted += ButtonEvent.Handle;
                Discord.SlashCommandExecuted += SlashCommand.Handle;
                Logger.LogInformation($"[FOFA] Bot is running");
                return Task.CompletedTask;
            };
            Discord.Ready += DiscordReady;
            await Task.Delay(3000);
            BotHandler.Run(Discord);
            await Task.Delay(-1);
        }
        private async Task DiscordReady()
        {
            //TODO commands
            SlashCommandBuilder? createTemplateSignupCommand = new SlashCommandBuilder()
                .WithName("create-signup-template")
                .WithDescription("Create new signup with template")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("template:")
                    .WithDescription("Leave blank to create question in officer chat")
                    .WithRequired(false)
                    .AddChoice("Tournament", 2)
                    .AddChoice("Brawl", 1)
                    .AddChoice("Base Capture", 0)
                    .WithType(ApplicationCommandOptionType.Integer)
                );
            SlashCommandBuilder? createCustomSignupCommand = new SlashCommandBuilder()
                .WithName("create-signup-custom")
                .WithDescription("Create new custom signup")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("name:")
                    .WithDescription("Name of the event")
                    .WithRequired(true)
                )
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("date:")
                    .WithDescription("Date of the event")
                    .WithRequired(true)
                );
            SlashCommandBuilder? changeAutomnaticSignupMessage = new SlashCommandBuilder()
                .WithName("change-automatic-signups")
                .WithDescription("Do you want to change automatic signup questions?")
                .AddOption(new SlashCommandOptionBuilder()
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.Boolean)
                );
            try
            {
                await Discord.CreateGlobalApplicationCommandAsync(createTemplateSignupCommand.Build());
                await Discord.CreateGlobalApplicationCommandAsync(createCustomSignupCommand.Build());
            }
            catch (Exception e)
            {
                Logger.LogCritical($"{e}");
            }
            //TODO commands
        }
    }
}