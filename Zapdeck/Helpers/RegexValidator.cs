using System.Text.RegularExpressions;

namespace Zapdeck.Helpers
{
    public static partial class RegexValidator
    {
        [GeneratedRegex(@"\[\[([^\(]*?)(?:\((.*?)\))?\]\]")]
        public static partial Regex MatchForCardTextCommand();

        [GeneratedRegex(@"\[\[\!([^\(]*?)(?:\((.*?)\))?\]\]")]
        public static partial Regex MatchForImageCommand();

        [GeneratedRegex(@"\[\[\$([^\(]*?)(?:\((.*?)\))?\]\]")]
        public static partial Regex MatchForPriceCommand();

        [GeneratedRegex(@"\[\[\#([^\(]*?)(?:\((.*?)\))?\]\]")]
        public static partial Regex MatchForLegalityCommand();

        public static string GetCardTextName(string message) => MatchForCardTextCommand().Match(message).Groups[1].ToString();
        public static string GetImageName(string message) => MatchForImageCommand().Match(message).Groups[1].ToString();
        public static string GetPriceName(string message) => MatchForPriceCommand().Match(message).Groups[1].ToString();
        public static string GetLegalityName(string message) => MatchForLegalityCommand().Match(message).Groups[1].ToString();
    }
}
