using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UltimateRedditBot.App.Factories.GuildFactory;

namespace UltimateRedditBot.Discord
{
    public class StartDiscord
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IGuildFactory _guildFactory;

        public StartDiscord(IServiceProvider provider, DiscordSocketClient discord, CommandService commands, IConfigurationRoot config, IGuildFactory guildFactory)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _config = config;
            _guildFactory = guildFactory;
        }

        public async Task StartAsync()
        {
            Console.WriteLine("Conecting...");
            await Connect();
            Console.WriteLine("Connected");

            //Load the modules
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);     // Load commands and modules into the command service

            await SyncDatabase();
        }

        /// <summary>
        /// Connects the bot to discord.
        /// </summary>
        private async Task Connect()
        {
            string discordToken = _config["AppKey"];     // Get the discord token from the config file
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Please enter your bot's token into the `appsettings.json` file found in the applications root directory.");

            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();

            await Task.Delay(2000);
            await _discord.SetGameAsync(_discord.Guilds.Count() + " servers!", type: ActivityType.Watching);
        }

        private async Task SyncDatabase()
        {
            var guildIds = _discord.Guilds.Select(x => x.Id);

            await _guildFactory.Insert(guildIds);
        }
    }
}
