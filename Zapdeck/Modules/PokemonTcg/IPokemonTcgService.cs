using Zapdeck.Models.PokemonTcg;

namespace Zapdeck.Modules.PokemonTcg
{
    public interface IPokemonTcgService
    {
        Task<ImageUri> GetImageUriAsync(string cardName);

        Task<CardPrices> GetPricesAsync(string cardName);
    }
}
