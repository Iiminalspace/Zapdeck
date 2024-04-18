using PokemonTcgSdk.Standard.Features.FilterBuilder.Pokemon;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;

namespace Zapdeck.Modules.PokemonTcg
{
    public class PokemonTcgService(PokemonApiClient pokeClient) : IPokemonTcgService
    {
        public async Task<Uri?> GetImageUriAsync(string cardName)
        {
            var filter = PokemonFilterBuilder.CreatePokemonFilter()
                .AddName(cardName);
            
            var cards = await pokeClient.GetApiResourceAsync<Card>(1, 0, filter);
            var card = cards.Results.FirstOrDefault();

            return card?.Images.Large;
        }
    }
}
