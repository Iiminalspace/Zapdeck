using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards.Models;

namespace Zapdeck.Models.PokemonTcg
{
    public record CardText(List<string> Types, int Hp, List<Ability> Abilities, List<Attack> Attacks, List<Resistance> Weaknesses, List<Resistance> Resistances, List<string> RetreatCost, Uri Image, CardInfo CardInfo);
}
