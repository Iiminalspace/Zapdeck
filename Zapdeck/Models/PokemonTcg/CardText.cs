using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards.Models;

namespace Zapdeck.Models.PokemonTcg
{
    public class CardText(Card card)
    {
        public string Supertype { get; } = card.Supertype;
        public List<string> Types { get; } = card.Types ?? [];
        public int Hp { get; } = card.Hp;
        public List<Ability> Abilities { get; } = card.Abilities ?? [];
        public List<Attack> Attacks { get; } = card.Attacks ?? [];
        public List<Resistance> Weaknesses { get; } = card.Weaknesses ?? [];
        public List<Resistance> Resistances { get; } = card.Resistances ?? [];
        public List<string> RetreatCost { get; } = card.RetreatCost ?? [];
        public List<string> Rules { get; } = card.Rules ?? [];
        public Uri Image { get; } = card.Images.Small;
        public CardInfo CardInfo { get; } = new CardInfo(card);
    }
}
