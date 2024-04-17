using System.Text.RegularExpressions;

namespace Zapdeck.Helpers
{
    public static partial class RegexValidator
    {
        [GeneratedRegex(@"\[\[([^\(]*?)(?:\((.*?)\))?\]\]")]
        public static partial Regex MatchForCardText();

        [GeneratedRegex(@"\[\[\!([^\(]*?)(?:\((.*?)\))?\]\]")]
        public static partial Regex MatchForImage();

        [GeneratedRegex(@"\[\[\$([^\(]*?)(?:\((.*?)\))?\]\]")]
        public static partial Regex MatchForPrice();

        [GeneratedRegex(@"\[\[\#([^\(]*?)(?:\((.*?)\))?\]\]")]
        public static partial Regex MatchForLegality();

        public static string GetCardTextName(string message) => MatchForCardText().Match(message).Groups[1].ToString();
        public static string GetImageName(string message) => MatchForImage().Match(message).Groups[1].ToString();
        public static string GetPriceName(string message) => MatchForPrice().Match(message).Groups[1].ToString(); 
        public static string GetLegalityName(string message) => MatchForLegality().Match(message).Groups[1].ToString();
    }
}
