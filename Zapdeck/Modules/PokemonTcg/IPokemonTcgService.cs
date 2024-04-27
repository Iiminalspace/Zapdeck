using Zapdeck.Models.PokemonTcg;

namespace Zapdeck.Modules.PokemonTcg
{
    public interface IPokemonTcgService
    {
        Task<CardText> GetCardTextAsync(string[] args);

        Task<CardImageUri> GetImageUriAsync(string[] args);

        Task<CardLegalities> GetLegalitiesAsync(string[] args);

        Task<CardPrices> GetPricesAsync(string[] args);
    }
}
