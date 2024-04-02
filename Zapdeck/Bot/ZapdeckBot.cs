using DSharpPlus;
using DSharpPlus.EventArgs;
using Zapdeck.Modules;

namespace Zapdeck.Bot
{
   public class ZapdeckBot(DiscordClient discordClient, IModule pokemonTcgModule) : IBot
   {
        public async Task StartAsync()
        {
            discordClient.Ready += DiscordClient_Ready;
            Console.WriteLine("Connecting to Discord");
            await discordClient.ConnectAsync();
            discordClient.MessageCreated += pokemonTcgModule.OnMessageCreated;
        }

        public async Task StopAsync()
        {
            if (discordClient != null)
            {
                await discordClient.DisconnectAsync();
                discordClient.Dispose();
            }
        }

        private static Task DiscordClient_Ready(DiscordClient client, ReadyEventArgs args)
        {
            Console.WriteLine("Client connected");
            return Task.CompletedTask;
        }
    } 
}
