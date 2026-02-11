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
            Discord.Ready += DiscordReady;
            Discord.Ready += () =>
            {
                Discord.ButtonExecuted += ButtonEventHandler.Handle;
                Discord.SlashCommandExecuted += SlashCommandHandler.Handle;
                Logger.LogInformation($"[FOFA] Bot is running");
                return Task.CompletedTask;
            };
            await Task.Delay(3000);
            BotHandler.Run(Discord);
            await Task.Delay(-1);
        }
        private Task DiscordReady()
        {
            try
            {
                SlashCommandBuilder? createTemplateSignupCommand = new SlashCommandBuilder()
                    .WithName("create-signup-template")
                    .WithDescription("Create new signup with template")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("template")
                        .WithDescription("Leave blank to create question in officer chat")
                        .WithRequired(false)
                        .AddChoice("Tournament", 0)
                        .AddChoice("Brawl", 1)
                        .AddChoice("Base Capture", 2)
                        .AddChoice("Golden Drop", 3)
                        .AddChoice("Stillwaters Chrono/Pulpe/Drops", 4)
                        .WithType(ApplicationCommandOptionType.Integer)
                    );
                SlashCommandBuilder? createCustomSignupCommand = new SlashCommandBuilder()
                    .WithName("create-signup-custom")
                    .WithDescription("Create new custom signup")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("name")
                        .WithDescription("Name of the event")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.String)
                    )
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("when")
                        .WithDescription("Date of the event - [day.month.year.hour.minute] of the event or [hours.minutes] untill the event")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.String)
                    );
                SlashCommandBuilder? changeAutomnaticSignupMessage = new SlashCommandBuilder()
                    .WithName("automatic-signups-question")
                    .WithDescription("Do you want to change automatic signup questions?")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("change")
                        .WithDescription("yes or no")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Boolean)
                    );
                SlashCommandBuilder? changeAutomnaticSignupReminder = new SlashCommandBuilder()
                    .WithName("automatic-signups-reminder")
                    .WithDescription("Do you want to change automatic signup reminders?")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("change")
                        .WithDescription("yes or no")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.Boolean)
                    );
                //await Discord.CreateGlobalApplicationCommandAsync(createTemplateSignupCommand.Build());
                //await Discord.CreateGlobalApplicationCommandAsync(createCustomSignupCommand.Build());
                //await Discord.CreateGlobalApplicationCommandAsync(changeAutomnaticSignupMessage.Build());
                //await Discord.CreateGlobalApplicationCommandAsync(changeAutomnaticSignupReminder.Build());
            }
            catch (Exception e)
            {
                Logger.LogCritical($"{e}");
            }

            return Task.CompletedTask;
        }
    }
}