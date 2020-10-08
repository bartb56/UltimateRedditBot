using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UltimateRedditBot.App.Factories.GuildFactory;

namespace UltimateRedditBot.Discord
{
    public class StartDiscord
    {
        #region Fields

        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly IGuildFactory _guildFactory;
        private readonly ILogger<StartDiscord> _logger;

        #endregion

        #region Constructor

        public StartDiscord(IServiceProvider provider, DiscordSocketClient discord, CommandService commands, IConfiguration config, IGuildFactory guildFactory, ILogger<StartDiscord> logger)
        {
            _provider = provider;
            _discord = discord;
            _commands = commands;
            _config = config;
            _guildFactory = guildFactory;
            _logger = logger;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Start the discord bot.
        /// </summary>
        /// <returns></returns>
        public async Task StartAsync()
        {
            //Connect to discord.
            _logger.LogInformation("Connecting...");
            await Connect();
            _logger.LogInformation("Connected");
            //Load the modules
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

            //Register new guilds.
            await RegisterNewGuilds();
        }

        /// <summary>
        /// Connects the bot to discord.
        /// </summary>
        private async Task Connect()
        {
            // Get the discord token from the config file
            var discordToken = _config["AppKey"];

            //Ensure the bot is not empty.
            if (string.IsNullOrWhiteSpace(discordToken))
                throw new Exception("Please enter your bot's token into the `appsettings.json` file found in the applications root directory.");

            await _discord.LoginAsync(TokenType.Bot, discordToken);
            await _discord.StartAsync();

            //The delay is unfortunately necessary due to discord taking a while to connect to all the guilds.
            await Task.Delay(1000);
            await _discord.SetGameAsync(_discord.Guilds.Count + " servers!", type: ActivityType.Watching);
        }

        /// <summary>
        /// Used to register any guild added while the bot was offline.
        /// </summary>
        /// <returns></returns>
        private async Task RegisterNewGuilds()
        {
            var guildIds = _discord.Guilds.Select(guild => guild.Id);
            await _guildFactory.Insert(guildIds);
        }

        #endregion
    }
}
