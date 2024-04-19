using PokemonTcgSdk.Standard.Features.FilterBuilder.Pokemon;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards.Models;
using Zapdeck.Models.PokemonTcg;

namespace Zapdeck.Modules.PokemonTcg
{
    public class PokemonTcgService(PokemonApiClient pokeClient) : IPokemonTcgService
    {
        public async Task<ImageUri> GetImageUriAsync(string cardName)
        {
            var filter = PokemonFilterBuilder.CreatePokemonFilter()
                                             .AddName(cardName);
            
            var cards = await pokeClient.GetApiResourceAsync<Card>(filter);
            var card = cards.Results.OrderByDescending(x => DateTime.Parse(x.Set.ReleaseDate))
                                    .FirstOrDefault(x => x.Name.Equals(cardName, StringComparison.CurrentCultureIgnoreCase));

            return new ImageUri(card.Images.Large, card.Name);
        }

        public async Task<CardPrices> GetPricesAsync(string cardName)
        {
            var filter = PokemonFilterBuilder.CreatePokemonFilter()
                                             .AddName(cardName);

            var cards = await pokeClient.GetApiResourceAsync<Card>(filter);

            var card = cards.Results.OrderByDescending(x => DateTime.Parse(x.Set.ReleaseDate))
                                    .FirstOrDefault(x => x.Name.Equals(cardName, StringComparison.CurrentCultureIgnoreCase));

            var tcgPlayerPrices = GetTcgPlayerPrices(card?.Tcgplayer.Prices);

            var cardMarketPrices = GetCardMarketPrices(card?.Cardmarket.Prices);

            var isParsed = int.TryParse(card?.Number, out var cardNumber);

            var cardPrices = isParsed ? new CardPrices(tcgPlayerPrices, cardMarketPrices, new CardInfo(card.Name, cardNumber.ToString("D3")))
                : new CardPrices(tcgPlayerPrices, cardMarketPrices, new CardInfo(card.Name, card.Number));

            return cardPrices;
        }

        private static Dictionary<string, double> GetTcgPlayerPrices(TcgPlayerPrices? tcgPrices)
        {
            var tcgPlayerPrices = new Dictionary<string, double>();

            if (tcgPrices?.Normal is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.Normal), tcgPrices.Normal.Market);
            }
            if (tcgPrices?.Holofoil is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.Holofoil), tcgPrices.Holofoil.Market);
            }
            if (tcgPrices?.ReverseHolofoil is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.ReverseHolofoil), tcgPrices.ReverseHolofoil.Market);
            }
            if (tcgPrices?.The1StEdition is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.The1StEdition), tcgPrices.The1StEdition.Market);
            }
            if (tcgPrices?.The1StEditionHolofoil is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.The1StEditionHolofoil), tcgPrices.The1StEditionHolofoil.Market);
            }
            if (tcgPrices?.Unlimited is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.Unlimited), tcgPrices.Unlimited.Market);
            }
            if (tcgPrices?.UnlimitedHolofoil is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.UnlimitedHolofoil), tcgPrices.UnlimitedHolofoil.Market);
            }

            return tcgPlayerPrices;
        }

        private static Dictionary<string, decimal> GetCardMarketPrices(CardMarketPrices? cardMarketPrices)
        {
            var cmPrices = new Dictionary<string, decimal>();

            if (cardMarketPrices?.AverageSellPrice is not null)
            {
                cmPrices.Add(nameof(cardMarketPrices.AverageSellPrice), cardMarketPrices.AverageSellPrice.Value);
            }
            if (cardMarketPrices?.ReverseHoloSell is not null)
            {
                cmPrices.Add(nameof(cardMarketPrices.ReverseHoloSell), cardMarketPrices.ReverseHoloSell.Value);
            }

            return cmPrices;
        }
    }
}
