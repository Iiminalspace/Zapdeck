using DSharpPlus;

namespace Zapdeck.Bot
{
   public class ZapdeckBot(DiscordClient discordClient) : IBot
   {
        public async Task StartAsync()
        {
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
    } 
}
