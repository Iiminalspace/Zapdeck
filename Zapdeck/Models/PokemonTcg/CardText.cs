using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards.Models;

namespace Zapdeck.Models.PokemonTcg
{
    public class CardText(string supertype,
                    List<string> types,
                    int hp,
                    List<Ability> abilities,
                    List<Attack> attacks,
                    List<Resistance> weaknesses,
                    List<Resistance> resistances,
                    List<string> retreatCost,
                    List<string> rules,
                    Uri image,
                    CardInfo cardInfo)
    {
        public string Supertype { get; } = supertype;
        public List<string> Types { get; } = types ?? [];
        public int Hp { get; } = hp;
        public List<Ability> Abilities { get; } = abilities ?? [];
        public List<Attack> Attacks { get; } = attacks ?? [];
        public List<Resistance> Weaknesses { get; } = weaknesses ?? [];
        public List<Resistance> Resistances { get; } = resistances ?? [];
        public List<string> RetreatCost { get; } = retreatCost ?? [];
        public List<string> Rules { get; } = rules ?? [];
        public Uri Image { get; } = image;
        public CardInfo CardInfo { get; } = cardInfo;
    }
}
