using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.CommonModels;
using System.Globalization;
using Zapdeck.Exceptions;
using Zapdeck.Helpers;
using Zapdeck.Models.PokemonTcg;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

namespace Zapdeck.Modules.PokemonTcg
{
    public class PokemonTcgModule(IPokemonTcgService pokemonTcgService) : IModule
    {
        private static readonly Dictionary<bool, string> _legalityEmojis = new() { { true, "✅" }, { false, "❌" } };
        public async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
            //Do not reply if the author is a bot
            if (e.Message.Author.IsBot) return;

            try
            {
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
            catch (CardNotFoundException ex)
            {
                await SendErrorMessageAsync(e, ex);
            }
            
        }

        private async Task SendImageMessage(MessageCreateEventArgs e)
        {
            var imageName = RegexValidator.GetImageName(e.Message.Content);

            var imageUri = await pokemonTcgService.GetImageUriAsync(imageName);
            var title = $"{imageUri.CardInfo.Name} ({imageUri.CardInfo.Number})";
            var msg = BuildBaseDiscordEmbed(title, imageUri.CardInfo);

            msg.WithImageUrl(imageUri.Uri.AbsoluteUri).Build();

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private static async Task SendCardTextMessageAsync(MessageCreateEventArgs e)
        {
            var cardTextName = RegexValidator.GetCardTextName(e.Message.Content);

            var msg = new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                Title = cardTextName.First(),
                Description = cardTextName.First(),
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
            var description = BuildPricesDescription(cardPrices);
            var title = $"Prices for {cardPrices.CardInfo.Name} ({cardPrices.CardInfo.Number})";

            // TODO: Try custom field in embed for TCGPlayer and CardKingdom headers
            var msg = BuildBaseDiscordEmbed(title, cardPrices.CardInfo, description);

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private async Task SendLegalityMessageAsync(MessageCreateEventArgs e)
        {
            var legalityName = RegexValidator.GetLegalityName(e.Message.Content);

            var cardLegalities = await pokemonTcgService.GetLegalitiesAsync(legalityName);
            var legalities = cardLegalities.Legalities;

            var isStandardLegal = DetermineLegality(legalities.Standard);
            var isExpandedLegal = DetermineLegality(legalities.Expanded);
            var isUnlimitedLegal = DetermineLegality(legalities.Unlimited);

            var description = BuildLegalityDescription(isStandardLegal, nameof(legalities.Standard))
                            + BuildLegalityDescription(isExpandedLegal, nameof(legalities.Expanded))
                            + BuildLegalityDescription(isUnlimitedLegal, nameof(legalities.Unlimited));

            var title = $"{cardLegalities.CardInfo.Name} ({cardLegalities.CardInfo.Number}) Legality";

            var msg = BuildBaseDiscordEmbed(title, cardLegalities.CardInfo, description);

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private static async Task SendErrorMessageAsync(MessageCreateEventArgs e, CardNotFoundException ex)
        {
            await e.Channel.SendMessageAsync($"{ex.Message}");
        }

        private static DiscordEmbedBuilder BuildBaseDiscordEmbed(string title, CardInfo cardInfo, string? description = null)
        {
            return new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                Title = title,
                Description = description,
                Footer = new EmbedFooter
                {
                    IconUrl = cardInfo.SetSymbolUri.AbsoluteUri,
                    Text = cardInfo.SetName
                }
            };
        }
        private static string BuildLegalityDescription(bool isLegal, string formatName)
        {
            return _legalityEmojis[isLegal] + " " + formatName + "\n";
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

        private static bool DetermineLegality(string legality)
        {
            return string.Equals(legality, "Legal");
        }
    }
}