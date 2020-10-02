using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using UltimateRedditBot.App.Constants;
using UltimateRedditBot.App.Factories.GuildSettingsFactory;
using UltimateRedditBot.Domain.Enums;

namespace UltimateRedditBot.Discord.Modules
{
    /// <summary>
    /// The command handler for all the settings related commands.
    /// </summary>
    public class SettingsModule : UltimateCommandModule
    {
        #region Fields

        private readonly IGuildSettingsFactory _guildSettingsFactory;

        #endregion

        #region Constructor

        public SettingsModule(IGuildSettingsFactory guildSettingsFactory)
        {
            _guildSettingsFactory = guildSettingsFactory;
        }

        #endregion

        #region Methods

        #region Bulk settings

        /// <summary>
        /// Used to get the value of a setting
        /// </summary>
        /// <returns>Setting value</returns>
        [Command("R-bulk"), Alias("r-bulk")]
        public async Task GetBulkSetting()
        {
            await GetSettingsValue(DefaultSettingKeys.Bulk);
        }

        /// <summary>
        /// Update a settings value
        /// </summary>
        /// <param name="value">New settings value</param>
        /// <returns></returns>
        [Command("R-bulk"), Alias("r-bulk")]
        public async Task UpdateBulkSettings(string value)
        {
            await UpdateSettings(DefaultSettingKeys.Bulk, value);
        }

        #endregion

        #region Sort settings

        /// <summary>
        /// Used to get the value of a setting
        /// </summary>
        /// <returns>Setting value</returns>
        [Command("R-sort"), Alias("r-sort")]
        public async Task GetSortSetting()
        {
            await GetSettingsValue(DefaultSettingKeys.Sort);
        }

        /// <summary>
        /// Update a settings value
        /// </summary>
        /// <param name="value">New settings value</param>
        /// <returns></returns>
        [Command("R-sort"), Alias("r-sort")]
        public async Task UpdateSortSettings(string value)
        {
            await UpdateSettings(DefaultSettingKeys.Sort, value);
        }

        #endregion

        /// <summary>
        /// Used to get the value of a setting
        /// </summary>
        /// <param name="key">Settings key </param>
        /// <returns>Setting value</returns>
        [Command("R-Setting"), Alias("r-s")]
        public async Task GetSettingsValue(string key)
        {
            var guild = ((SocketGuildChannel)Context.Channel).Guild;
            var settingValue = await _guildSettingsFactory.GetGuildSettingByKey<string>(guild.Id, key);

            if (string.IsNullOrEmpty(settingValue))
                await ReplyAsync("Couldn't find the requested setting");

            await ReplyAsync(settingValue);
        }

        /// <summary>
        /// Update a settings value
        /// </summary>
        /// <param name="key">Settings key</param>
        /// <param name="value">New settings value</param>
        /// <returns></returns>
        [Command("R-Setting"), Alias("r-s")]
        public async Task UpdateSettings(string key, string value)
        {
            //Validate
            if (!await ValidateUpdateSetting(key, value))
                return;

            var guild = ((SocketGuildChannel)Context.Channel).Guild;
            var success = await _guildSettingsFactory.SaveChanges(guild.Id, key, value);

            var responseMsg = success ? "Successfully saved the setting." : "Couldn't find the setting";
            await ReplyAsync(responseMsg);
        }

        /// <summary>
        /// Validate the update key request
        /// </summary>
        /// <param name="key">The setting key</param>
        /// <param name="value">New value</param>
        /// <returns></returns>
        private async Task<bool> ValidateUpdateSetting(string key, string value)
        {
            if(key.Equals(DefaultSettingKeys.Bulk, StringComparison.OrdinalIgnoreCase))
            {
                var isInt = int.TryParse(value, out var amount);
                if (!isInt)
                {
                    await ReplyAsync("This setting can only have numbers");
                    return false;
                }

                if (amount <= 100)
                    return true;

                await ReplyAsync("The max amount for this setting is 100");
                return false;
            }

            if (!key.Equals(DefaultSettingKeys.Sort, StringComparison.OrdinalIgnoreCase))
                return true;

            if (Enum.TryParse<Sort>(value, true, out _))
                return true;

            await ReplyAsync("Not a valid value, either use 'new' or 'hot'.");
            return false;
        }

        #endregion
    }
}
