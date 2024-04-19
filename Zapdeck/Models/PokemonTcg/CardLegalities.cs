using PokemonTcgSdk.Standard.Infrastructure.HttpClients.CommonModels;

namespace Zapdeck.Models.PokemonTcg
{
    public record CardLegalities(Legalities Legalities, CardInfo CardInfo);
}
