using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Globalization;
using Zapdeck.Helpers;
using Zapdeck.Models.PokemonTcg;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

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
                await SendPriceMessageAsync(e);
            }
            else if (RegexValidator.MatchForLegality().IsMatch(e.Message.Content))
            {
                await SendLegalityMessageAsync(e);
            }
            else if (RegexValidator.MatchForCardText().IsMatch(e.Message.Content))
            {
                await SendCardTextMessageAsync(e);
            }
        }

        private async Task SendImageMessage(MessageCreateEventArgs e)
        {
            var imageName = RegexValidator.GetImageName(e.Message.Content);

            var imageUriResponse = await pokemonTcgService.GetImageUriAsync(imageName);

            var msg = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                ImageUrl = imageUriResponse.Uri.AbsoluteUri,
                Title = imageUriResponse.CardName
            };

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private static async Task SendCardTextMessageAsync(MessageCreateEventArgs e)
        {
            var cardTextName = RegexValidator.GetCardTextName(e.Message.Content);

            var msg = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                Title = cardTextName,
                Description = cardTextName,
                Thumbnail = new EmbedThumbnail
                {
                    Url = "https://images.pokemontcg.io/swsh12pt5/60.png"
                }
            };

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private async Task SendPriceMessageAsync(MessageCreateEventArgs e)
        {
            var priceName = RegexValidator.GetPriceName(e.Message.Content);

            var cardPrices = await pokemonTcgService.GetPricesAsync(priceName);
            string description = BuildPricesDescription(cardPrices);

            //TODO: Format Set and number in title - Charizard ex - PAF 054/091
            var msg = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                Title = $"Prices for {cardPrices.CardInfo.Name} ({cardPrices.CardInfo.Number})",
                Description = description
            };

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private static async Task SendLegalityMessageAsync(MessageCreateEventArgs e)
        {
            var legalityName = RegexValidator.GetLegalityName(e.Message.Content);

            //TODO: Format Set and number in title - Charizard ex - 054/091
            var msg = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                Title = $"{legalityName} Legality",
                Description = "✅ Standard\n" + "❌ Expanded\n" + "❌ Unlimited"
            };

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private static string BuildPricesDescription(CardPrices cardPrices)
        {
            var description = "**TCGPlayer**\n";

            foreach (var tcgPlayerPrice in cardPrices.TcgPlayerPrices)
            {
                description += tcgPlayerPrice.Key + ": " + tcgPlayerPrice.Value.ToString("C2") + "\n";
            }

            description += "\n**CardMarket**\n";

            foreach (var cardMarketPrice in cardPrices.CardMarketPrices)
            {
                if (cardMarketPrice.Value > 0M)
                {
                    //Using French culture info for Euro formatting
                    description += cardMarketPrice.Key + ": " + cardMarketPrice.Value.ToString("C2", new CultureInfo("fr-FR")) + "\n";
                }
            }

            return description;
        }
    }
}