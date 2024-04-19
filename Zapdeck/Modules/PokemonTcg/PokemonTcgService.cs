using PokemonTcgSdk.Standard.Features.FilterBuilder.Pokemon;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards.Models;
using Zapdeck.Exceptions;
using Zapdeck.Models.PokemonTcg;

namespace Zapdeck.Modules.PokemonTcg
{
    public class PokemonTcgService(PokemonApiClient pokeClient) : IPokemonTcgService
    {
        public async Task<CardImageUri> GetImageUriAsync(List<string> cardName)
        {
            var card = await GetCardAsync(cardName);

            return new CardImageUri(card.Images.Large, new CardInfo(card.Name, card.Number, card.Set.Name, card.Set.Images.Symbol));
        }

        public async Task<CardLegalities> GetLegalitiesAsync(List<string> cardName)
        {
            var card = await GetCardAsync(cardName);

            return new CardLegalities(card.Legalities, new CardInfo(card.Name, card.Number, card.Set.Name, card.Set.Images.Symbol));
        }

        public async Task<CardPrices> GetPricesAsync(List<string> cardName)
        {
            var card = await GetCardAsync(cardName);

            var tcgPlayerPrices = GetTcgPlayerPrices(card.Tcgplayer.Prices);
            var cardMarketPrices = GetCardMarketPrices(card.Cardmarket.Prices);

            return new CardPrices(tcgPlayerPrices, cardMarketPrices, new CardInfo(card.Name, card.Number, card.Set.Name, card.Set.Images.Symbol));
        }

        private async Task<Card> GetCardAsync(List<string> cardName)
        {
            var filter = PokemonFilterBuilder.CreatePokemonFilter();
            if (cardName.Count == 1)
            {
                filter.AddName(cardName.First());
            }
            else
            {
                filter.AddName(cardName.First())
                      .TryAdd("number", cardName[1].TrimStart('0'));
            }
            

            var cards = await pokeClient.GetApiResourceAsync<Card>(filter);
            var card = cards.Results.OrderByDescending(x => DateTime.Parse(x.Set.ReleaseDate))
                                    .FirstOrDefault(x => x.Name.Equals(cardName.First(), StringComparison.CurrentCultureIgnoreCase));

            return card is null ? throw new CardNotFoundException(cardName.First()) : card;
        }

        private static Dictionary<string, double> GetTcgPlayerPrices(TcgPlayerPrices tcgPrices)
        {
            var tcgPlayerPrices = new Dictionary<string, double>();

            if (tcgPrices.Normal is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.Normal), tcgPrices.Normal.Market);
            }
            if (tcgPrices.Holofoil is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.Holofoil), tcgPrices.Holofoil.Market);
            }
            if (tcgPrices.ReverseHolofoil is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.ReverseHolofoil), tcgPrices.ReverseHolofoil.Market);
            }
            if (tcgPrices.The1StEdition is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.The1StEdition), tcgPrices.The1StEdition.Market);
            }
            if (tcgPrices.The1StEditionHolofoil is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.The1StEditionHolofoil), tcgPrices.The1StEditionHolofoil.Market);
            }
            if (tcgPrices.Unlimited is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.Unlimited), tcgPrices.Unlimited.Market);
            }
            if (tcgPrices.UnlimitedHolofoil is not null)
            {
                tcgPlayerPrices.Add(nameof(tcgPrices.UnlimitedHolofoil), tcgPrices.UnlimitedHolofoil.Market);
            }

            return tcgPlayerPrices;
        }

        private static Dictionary<string, decimal> GetCardMarketPrices(CardMarketPrices cardMarketPrices)
        {
            var cmPrices = new Dictionary<string, decimal>();

            if (cardMarketPrices.AverageSellPrice is not null)
            {
                cmPrices.Add(nameof(cardMarketPrices.AverageSellPrice), cardMarketPrices.AverageSellPrice.Value);
            }
            if (cardMarketPrices.ReverseHoloSell is not null)
            {
                cmPrices.Add(nameof(cardMarketPrices.ReverseHoloSell), cardMarketPrices.ReverseHoloSell.Value);
            }

            return cmPrices;
        }
    }
}
