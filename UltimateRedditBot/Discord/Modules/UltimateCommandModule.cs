using Discord.Commands;

namespace UltimateRedditBot.Discord.Modules
{
    /// <summary>
    /// Used as a base for the ultimate reddit bot commands
    /// </summary>
    [RequireContext(ContextType.Guild)]
    public abstract class UltimateCommandModule : ModuleBase
    {
    }
}
