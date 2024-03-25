using System.Text.RegularExpressions;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Zapdeck.Bot
{
   public class ZapdeckBot(DiscordClient discordClient) : IBot
   {
        public async Task StartAsync()
        {
            discordClient.Ready += DiscordClient_Ready;
            Console.WriteLine("Connecting to Discord");
            await discordClient.ConnectAsync();
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
