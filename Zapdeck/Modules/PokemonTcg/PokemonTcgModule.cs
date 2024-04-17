using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Zapdeck.Helpers;

namespace Zapdeck.Modules.PokemonTcg
{
    public class PokemonTcgModule(IPokemonTcgService pokemonTcgService) : IModule
    {
        public async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
            //Do not reply if the author is a bot
            if (e.Message.Author.IsBot) return;

            if (RegexValidator.MatchForImage().IsMatch(e.Message.Content))
            {
                await SendImageMessage(e);
            }
            else if (RegexValidator.MatchForPrice().IsMatch(e.Message.Content))
            {
                await SendPriceMessage(e);
            }
            else if (RegexValidator.MatchForLegality().IsMatch(e.Message.Content))
            {
                await SendLegalityMessage(e);
            }
            else if (RegexValidator.MatchForCardText().IsMatch(e.Message.Content))
            {
                await SendCardTextMessage(e);
            }
        }
        private static async Task SendImageMessage(MessageCreateEventArgs e)
        {
            var imageName = RegexValidator.GetImageName(e.Message.Content);

            var msg = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                ImageUrl = "https://archives.bulbagarden.net/media/upload/thumb/1/17/Cardback.jpg/429px-Cardback.jpg?20240322144729",
                Description = imageName
            };

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private static async Task SendCardTextMessage(MessageCreateEventArgs e)
        {
            var cardTextName = RegexValidator.GetCardTextName(e.Message.Content);

            var msg = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                Description = cardTextName
            };

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private static async Task SendPriceMessage(MessageCreateEventArgs e)
        {
            var priceName = RegexValidator.GetPriceName(e.Message.Content);

            var msg = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                Description = priceName
            };

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private static async Task SendLegalityMessage(MessageCreateEventArgs e)
        {
            var legalityName = RegexValidator.GetLegalityName(e.Message.Content);

            var msg = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                Title = $"{legalityName} Legality",
                Description = "✅ Standard\n" + "❌ Expanded\n" + "❌ Unlimited"
            };

            await e.Channel.SendMessageAsync(embed: msg);
        }

    }
}