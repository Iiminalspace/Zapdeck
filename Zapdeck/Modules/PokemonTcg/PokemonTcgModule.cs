using System.Text.RegularExpressions;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace Zapdeck.Modules.PokemonTcg
{
    public partial class PokemonTcgModule : BaseCommandModule
    {
        [Command("#")]
        public static async Task Legality(CommandContext ctx)
        {
             //Do not reply if the author is a bot
            if (ctx.Message.Author.IsBot) return;

            var regex = MatchForLegality();
            if (regex.IsMatch(ctx.Message.Content))
            {
                var response = regex.Match(ctx.Message.Content).Groups[1].ToString();
                await ctx.Channel.SendMessageAsync(response);
            }
        }

        [GeneratedRegex(@"\[\[\#([^\(]*?)(?:\((.*?)\))?\]\]")]
        private static partial Regex MatchForLegality();
    }
}