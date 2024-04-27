using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Set;

namespace Zapdeck.Models.PokemonTcg
{
    public class CardInfo(Card card)
    {
        private static readonly Dictionary<string, string> _missingSetCodes = new()
        {
            { "sve", "SVE" },
            { "sv1", "SVI" },
            { "sv2", "PAL" },
            { "sv3", "OBF" },
            { "sv3pt5", "MEW" },
            { "sv4", "PAR" },
            { "sv4pt5", "PAF" }
        };

        public string Name { get; } = card.Name;
        public string Number { get; } = FormatCardNumber(card.Number);
        public string SetName { get; } = card.Set.Name;
        public string SetCode { get; } = GetSetCode(card.Set);
        public Uri SetSymbolUri { get; } = card.Set.Images.Symbol;

        private static string FormatCardNumber(string number)
        {
            var isParsed = int.TryParse(number, out var parsedCardNumber);
            return isParsed ? parsedCardNumber.ToString("D3") : number;
        }

        private static string GetSetCode(Set set)
        {
            var setCode = set.PtcgoCode;
            if (string.IsNullOrEmpty(setCode))
            {
                //Check is required due to open bug in TCG API
                setCode = _missingSetCodes.TryGetValue(set.Id, out var code) ? code : set.Id.ToUpper();
            }

            return setCode;
        }
    }
}
