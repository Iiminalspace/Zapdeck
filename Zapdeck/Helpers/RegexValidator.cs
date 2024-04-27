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

        public static string[] GetCardTextName(string message)
        {
            var args = MatchForCardText().Match(message).Groups[1]
                                         .ToString()
                                         .Split("|", StringSplitOptions.TrimEntries);

            return MapArgs(args);
        }


        public static string[] GetImageName(string message)
        {
            var args = MatchForImage().Match(message).Groups[1]
                                      .ToString()
                                      .Split("|", StringSplitOptions.TrimEntries);

            return MapArgs(args);
        }
        public static string[] GetPriceName(string message)
        {
            var args = MatchForPrice().Match(message).Groups[1]
                                      .ToString()
                                      .Split("|", StringSplitOptions.TrimEntries);

            return MapArgs(args);
        }

        public static string[] GetLegalityName(string message)
        {
            var args = MatchForLegality().Match(message).Groups[1]
                                         .ToString()
                                         .Split("|", StringSplitOptions.TrimEntries);

            return MapArgs(args);
        } 
                                                                                        
        private static string[] MapArgs(string[] args)
        {
            var maxArgs = new string[3];

            for (int i = 0; i < args.Length; i++)
            {
                maxArgs[i] = args[i];
            }

            return maxArgs;
        }
    }
}
