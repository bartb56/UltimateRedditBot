using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using UltimateRedditBot.App.Factories.GuildFactory;

namespace UltimateRedditBot.Discord.Commands
{
    public class CommandHandler
    {
        #region Fields

        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfiguration _config;
        private readonly IServiceProvider _provider;
        private readonly IGuildFactory _guildFactory;

        #endregion

        #region Constructor

        public CommandHandler(DiscordSocketClient discord,
            CommandService commands,
            IConfiguration config,
            IServiceProvider provider,
            IGuildFactory guildFactory)
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;
            _guildFactory = guildFactory;

            _discord.MessageReceived += OnMessageReceivedAsync;
            _discord.JoinedGuild += OnGuildJoin;

        }

        #endregion

        #region Methods

        /// <summary>
        /// Used to handle the on message received call.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            //Ensure that the message is sent by a user.
            if (!(s is SocketUserMessage msg) || msg.Author.Id == _discord.CurrentUser.Id)
                return;

            //Set the command context.
            var context = new SocketCommandContext(_discord, msg);

            //Ensure that the message being sent had the configured prefix.
            //In the future I might make the prefix configurable per guild.
            var argPos = 0;
            if (msg.HasStringPrefix(_config["Prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                //Execute the command
                var result = await _commands.ExecuteAsync(context, argPos, _provider);

                //If sending the message was not successful send the error message.
                if (!result.IsSuccess)
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }

        /// <summary>
        /// Handles the on guild joined event.
        /// Every guild that joins has to ge registered in the database.
        /// </summary>
        /// <param name="socketGuild">socketGuild event</param>
        /// <returns></returns>
        private async Task OnGuildJoin(SocketGuild socketGuild)
        {
            await _guildFactory.Insert(socketGuild.Id);
            await UpdateBotStatus();
        }

        /// <summary>
        /// Sets the bot status to amount of guilds it has joined.
        /// </summary>
        /// <returns></returns>
        private async Task UpdateBotStatus()
        {
            await _discord.SetGameAsync(_discord.Guilds.Count + " servers!", type: ActivityType.Watching);
        }

        #endregion
    }
}
