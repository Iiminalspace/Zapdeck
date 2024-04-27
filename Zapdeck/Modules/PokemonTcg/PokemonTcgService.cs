using PokemonTcgSdk.Standard.Features.FilterBuilder.Pokemon;
using PokemonTcgSdk.Standard.Features.FilterBuilder.Set;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards.Models;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Set;
using Zapdeck.Exceptions;
using Zapdeck.Models.PokemonTcg;

namespace Zapdeck.Modules.PokemonTcg
{
    public class PokemonTcgService(PokemonApiClient pokeClient) : IPokemonTcgService
    {
        public async Task<CardText> GetCardTextAsync(string[] args)
        {
            var card = await GetCardAsync(args);

            return new CardText(card.Supertype,
                                card.Types,
                                card.Hp,
                                card.Abilities,
                                card.Attacks,
                                card.Weaknesses,
                                card.Resistances,
                                card.RetreatCost,
                                card.Rules,
                                card.Images.Small,
                                new CardInfo(card));
        }

        public async Task<CardImageUri> GetImageUriAsync(string[] args)
        {
            var card = await GetCardAsync(args);

            return new CardImageUri(card.Images.Large, new CardInfo(card));
        }

        public async Task<CardLegalities> GetLegalitiesAsync(string[] args)
        {
            var card = await GetCardAsync(args);

            return new CardLegalities(card.Legalities, new CardInfo(card));
        }

        public async Task<CardPrices> GetPricesAsync(string[] args)
        {
            var card = await GetCardAsync(args);

            var tcgPlayerPrices = MapTcgPlayerPrices(card.Tcgplayer.Prices);
            var cardMarketPrices = MapCardMarketPrices(card.Cardmarket.Prices);

            return new CardPrices(tcgPlayerPrices, cardMarketPrices, new CardInfo(card));
        }

        private async Task<Card> GetCardAsync(string[] args)
        {
            //Array is always length 3
            var name = args[0];
            var code = args[1];
            var number = args[2];

            Card? card;
            if (string.IsNullOrEmpty(code))
            {
                var nameFilter = PokemonFilterBuilder.CreatePokemonFilter().AddName(name);
                var cards = await pokeClient.GetApiResourceAsync<Card>(nameFilter);
                card = FilterCards(cards.Results, name);
            }
            else if (string.IsNullOrEmpty(number))
            {
                var setFilter = await BuildSetFilter(name, code);
                var cards = await pokeClient.GetApiResourceAsync<Card>(setFilter);
                card = FilterCards(cards.Results, name);
            }
            else
            {
                var trimmedNumber = number.TrimStart('0');
                var setFilter = await BuildSetFilter(name, code, trimmedNumber);
                var cards = await pokeClient.GetApiResourceAsync<Card>(setFilter);

                card = cards.Results.FirstOrDefault();
            }

            return card is null ? throw new CardNotFoundException(name) : card;
        }

        private async Task<PokemonFilterCollection<string, string>> BuildSetFilter(string name, string code, string? number = null)
        {
            var ptcgoFilter = SetFilterBuilder.CreateSetFilter();
            ptcgoFilter.Add("ptcgoCode", code);
            var sets = await pokeClient.GetApiResourceAsync<Set>(ptcgoFilter);
            var setIdFilter = PokemonFilterBuilder.CreatePokemonFilter();

            switch (sets.Results.Count)
            {
                case 0:
                    setIdFilter.AddName(name)
                               .AddSetId(code);
                    break;
                case >= 2:
                    setIdFilter.AddName(name);
                    foreach (var set in sets.Results)
                    {
                        setIdFilter.AddSetId(set.Id);
                    }
                    break;
                default:
                    setIdFilter.AddName(name)
                               .AddSetId(sets.Results.First().Id);
                    break;
            }

            if (number is not null)
            {
                setIdFilter.Add("number", number);
            }

            return setIdFilter;
        }

        private static Card? FilterCards(List<Card> cards, string name)
        {
            return cards.OrderByDescending(x => DateTime.Parse(x.Set.ReleaseDate))
                        .FirstOrDefault(x => x.Name.Equals(name, StringComparison.CurrentCultureIgnoreCase));
        }

        private static Dictionary<string, double> MapTcgPlayerPrices(TcgPlayerPrices tcgPrices)
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

        private static Dictionary<string, decimal> MapCardMarketPrices(CardMarketPrices cardMarketPrices)
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
