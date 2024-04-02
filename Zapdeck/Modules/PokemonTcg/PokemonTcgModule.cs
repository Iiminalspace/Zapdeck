using DSharpPlus;
using DSharpPlus.EventArgs;
using Zapdeck.Helpers;

namespace Zapdeck.Modules.PokemonTcg
{
    public class PokemonTcgModule : IModule
    {
        public async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
             //Do not reply if the author is a bot
            if (e.Message.Author.IsBot) return;

            if (RegexValidator.MatchForLegality().IsMatch(e.Message.Content))
            {
                var response = RegexValidator.GetLegalityName(e.Message.Content);
                await e.Channel.SendMessageAsync(response);
            }
        }
    }
}