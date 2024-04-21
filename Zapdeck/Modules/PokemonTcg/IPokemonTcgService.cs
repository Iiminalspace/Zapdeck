using Zapdeck.Models.PokemonTcg;

namespace Zapdeck.Modules.PokemonTcg
{
    public interface IPokemonTcgService
    {
        Task<CardText> GetCardTextAsync(List<string> cardName);

        Task<CardImageUri> GetImageUriAsync(List<string> cardName);

        Task<CardLegalities> GetLegalitiesAsync(List<string> cardName);

        Task<CardPrices> GetPricesAsync(List<string> cardName);
    }
}
