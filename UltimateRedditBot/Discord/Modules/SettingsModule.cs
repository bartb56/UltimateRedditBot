using Discord;
using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using UltimateRedditBot.App.Constants;
using UltimateRedditBot.App.Factories.GuildSettingsFactory;

namespace UltimateRedditBot.Discord.Modules
{
    public class SettingsModule : UltimateCommandModule
    {

        private readonly DiscordSocketClient _discord;
        private readonly IGuildSettingsFactory _guildSettingsFactory;

        public SettingsModule(
            DiscordSocketClient discord,
            IGuildSettingsFactory guildSettingsFactory)
        {
            _discord = discord;
            _guildSettingsFactory = guildSettingsFactory;
        }

        [Command("R-Settings"), Alias("r-s")]
        public async Task Reddit(string key)
        {
            SocketGuild guild = ((SocketGuildChannel)Context.Channel).Guild;
            var settingValue = await _guildSettingsFactory.GetGuildSettingByKey<string>(guild.Id, key);

            if (string.IsNullOrEmpty(settingValue))
                await ReplyAsync("Couldn't find the requested setting");

            await ReplyAsync(settingValue);
        }

        [Command("R-Settings-update"), Alias("r-s-u"), RequireUserPermission(GuildPermission.BanMembers)]
        public async Task UpdateBulkSettings(string key, string value)
        {
            SocketGuild guild = ((SocketGuildChannel)Context.Channel).Guild;

            if(key.Equals(DefaultSettingKeys.Bulk, System.StringComparison.OrdinalIgnoreCase))
            {
                var isInt = Int32.TryParse(value, out int amount);
                if (!isInt)
                {
                    await ReplyAsync("This setting can only have numbers");
                    return;
                }
                else if(amount > Int32.MaxValue)
                {
                    await ReplyAsync("The max amount for this setting is 500");
                    return;
                }
            }

            var success = await _guildSettingsFactory.SaveChanges(guild.Id, key, value);

            var responseMsg = (success) ? "Successfully saved the setting." : "Couldn't find the setting";
            await ReplyAsync(responseMsg);
        }

    }
}
