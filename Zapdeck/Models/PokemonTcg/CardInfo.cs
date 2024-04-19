namespace Zapdeck.Models.PokemonTcg
{
    public class CardInfo(string name, string number, string setName, Uri setSymbolUri)
    {
        public string Name { get; } = name;
        public string Number { get; } = FormatCardNumber(number);
        public string SetName { get; } = setName;
        public Uri SetSymbolUri { get; } = setSymbolUri;

        private static string FormatCardNumber(string number)
        {
            var isParsed = int.TryParse(number, out var parsedCardNumber);
            return isParsed ? parsedCardNumber.ToString("D3") : number;
        }
    }
}
