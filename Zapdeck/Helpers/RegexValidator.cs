using System.Text.RegularExpressions;

namespace Zapdeck.Helpers
{
    public static partial class RegexValidator
    {
        [GeneratedRegex(@"\[([^\(]*?)(?:\((.*?)\))?\]")]
        public static partial Regex MatchForCardText();

        [GeneratedRegex(@"\[\!([^\(]*?)(?:\((.*?)\))?\]")]
        public static partial Regex MatchForImage();

        [GeneratedRegex(@"\[\$([^\(]*?)(?:\((.*?)\))?\]")]
        public static partial Regex MatchForPrice();

        [GeneratedRegex(@"\[\#([^\(]*?)(?:\((.*?)\))?\]")]
        public static partial Regex MatchForLegality();

        public static List<string> GetCardTextName(string message) => MatchForCardText().Match(message).Groups[1]
                                                                                        .ToString()
                                                                                        .Split("|", StringSplitOptions.TrimEntries)
                                                                                        .ToList();
        public static List<string> GetImageName(string message) => MatchForImage().Match(message).Groups[1]
                                                                                  .ToString()
                                                                                  .Split("|", StringSplitOptions.TrimEntries)
                                                                                  .ToList();
        public static List<string> GetPriceName(string message) => MatchForPrice().Match(message).Groups[1]
                                                                                  .ToString()
                                                                                  .Split("|", StringSplitOptions.TrimEntries)
                                                                                  .ToList(); 
        public static List<string> GetLegalityName(string message) => MatchForLegality().Match(message).Groups[1]
                                                                                        .ToString()
                                                                                        .Split("|", StringSplitOptions.TrimEntries)
                                                                                        .ToList();
    }
}
