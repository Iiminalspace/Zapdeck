using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Zapdeck.Modules
{
    public interface IModule
    {
        Task OnMessageCreated(DiscordClient discordClient, MessageCreateEventArgs e);
    }
}
