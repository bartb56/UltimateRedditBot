using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using UltimateRedditBot.App.Factories.GuildFactory;
using UltimateRedditBot.Domain.Models;

namespace UltimateRedditBot.Discord.Commands
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;
        private readonly IGuildFactory _guildFactory;

        public CommandHandler(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
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
            _discord.LeftGuild += OnGuildLeave;

            _commands.CommandExecuted += CommandExecutedAsync;


        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;     // Ensure the message is from a user/bot
            if (msg == null || msg.Author.Id == _discord.CurrentUser.Id)
                return;

            var context = new SocketCommandContext(_discord, msg);     // Create the command context
            int argPos = 0;     // Check if the message has a valid command prefix
            if (msg.HasStringPrefix(_config["Prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command


               

                if (!result.IsSuccess)     // If not successful, reply with the error.
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }

        public async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // if a command isn't found, log that info to console and exit this method
            if (!command.IsSpecified)
            {
                return;
            }
        }

        private async Task OnGuildJoin(SocketGuild socketGuild)
        {
            await _guildFactory.Insert(socketGuild.Id);
            await UpdateBotStatus();
        }

        private async Task OnGuildLeave(SocketGuild socketGuild)
        {
            /*var guild = await _guildAppService.GetByServerId(socketGuild.Id);
            if (guild == null)
                throw new NullReferenceException(nameof(guild));

            await _guildAppService.Delete(guild);
            await UpdateBotStatus();*/
        }

        private async Task UpdateBotStatus()
        {
            await _discord.SetGameAsync(_discord.Guilds.Count() + " servers!", type: ActivityType.Watching);
        }
    }
}
