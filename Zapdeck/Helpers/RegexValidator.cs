using System.Text.RegularExpressions;

namespace Zapdeck.Helpers
{
    public static partial class RegexValidator
    {
        [GeneratedRegex(@"\[\[\#([^\(]*?)(?:\((.*?)\))?\]\]")]
        public static partial Regex MatchForLegality();

        public static string GetLegalityName(string message) => MatchForLegality().Match(message).Groups[1].ToString();
    }
}
