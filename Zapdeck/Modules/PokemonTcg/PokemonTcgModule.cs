using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using System.Globalization;
using Zapdeck.Exceptions;
using Zapdeck.Helpers;
using Zapdeck.Models.PokemonTcg;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

namespace Zapdeck.Modules.PokemonTcg
{
    public class PokemonTcgModule(IPokemonTcgService pokemonTcgService) : IModule
    {
        private const int LINE_LENGTH = 80;
        private static readonly Dictionary<bool, string> _legalityEmojis = new() { { true, "✅" }, { false, "❌" } };
        private static readonly Dictionary<string, string> _typeEmoji = new()
        {
            { "Colorless", "<:Colorless:1231411954250289213>" },
            { "Darkness", "<:Darkness:1231418975569707008>" },
            { "Dragon", "<:Dragon:1231422307545780294>" },
            { "Fairy", "<:Fairy:1231422648635097118>" },
            { "Fighting", "<:Fighting:1231421578747842630>" },
            { "Fire", "<:Fire:1231420657993256960>" },
            { "Grass", "<:Grass:1231416754161324165>" },
            { "Lightning", "<:Lightning:1231418017724764213>" },
            { "Metal", "<:Metal:1231414304537051208>" },
            { "Psychic", "<:Psychic:1231421970902417428>" },
            { "Water", "<:Water:1231421068099452968>" }
        };
        public async Task OnMessageCreated(DiscordClient client, MessageCreateEventArgs e)
        {
            //Do not reply if the author is a bot
            if (e.Message.Author.IsBot) return;

            try
            {
                if (RegexValidator.MatchForImage().IsMatch(e.Message.Content))
                {
                    await SendImageMessage(e);
                }
                else if (RegexValidator.MatchForPrice().IsMatch(e.Message.Content))
                {
                    await SendPriceMessageAsync(e);
                }
                else if (RegexValidator.MatchForLegality().IsMatch(e.Message.Content))
                {
                    await SendLegalityMessageAsync(e);
                }
                else if (RegexValidator.MatchForCardText().IsMatch(e.Message.Content))
                {
                    await SendCardTextMessageAsync(e);
                }
            }
            catch (CardNotFoundException ex)
            {
                await SendErrorMessageAsync(e, ex);
            }
            
        }

        private async Task SendImageMessage(MessageCreateEventArgs e)
        {
            var imageName = RegexValidator.GetImageName(e.Message.Content);

            var imageUri = await pokemonTcgService.GetImageUriAsync(imageName);
            var title = $"{imageUri.CardInfo.Name} ({imageUri.CardInfo.Number})";
            var msg = BuildBaseDiscordEmbed(title, imageUri.CardInfo);

            msg.WithImageUrl(imageUri.Uri.AbsoluteUri).Build();

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private async Task SendCardTextMessageAsync(MessageCreateEventArgs e)
        {
            var cardTextName = RegexValidator.GetCardTextName(e.Message.Content);

            var cardText = await pokemonTcgService.GetCardTextAsync(cardTextName);
            var cardName = $"{cardText.CardInfo.Name} ({cardText.CardInfo.Number})";
            var hp = "HP" + cardText.Hp + " ";

            foreach (var pokemonType in cardText.Types)
            {
                hp += _typeEmoji[pokemonType];
            }

            var title = $"{cardName} — {hp}";

            var abilities = new Dictionary<string, string>();

            if(cardText.Abilities is not null && cardText.Abilities.Count is not 0)
            {
                foreach(var ability in cardText.Abilities)
                {
                    var abilityText = ReplaceEnergyEmoji(ability.Text);
                    abilities.Add($"{ability.Type} — {ability.Name}", abilityText);
                }
            }

            var attacks = new Dictionary<string, string>();

            if(cardText.Attacks is not null && cardText.Attacks.Count is not 0)
            {
                foreach(var attack in cardText.Attacks)
                {
                    if (String.IsNullOrEmpty(attack.Text))
                    {
                        //Set to a value to pass to embed builder
                        attack.Text = "\u0000";
                    }

                    var costEmoji = string.Empty;

                    foreach(var cost in attack.Cost)
                    {
                        costEmoji += _typeEmoji[cost];
                    }

                    var attackText = ReplaceEnergyEmoji(attack.Text);

                    if (String.IsNullOrEmpty(attack.Damage))
                    {
                        attacks.Add($"{costEmoji} {attack.Name}", attackText);
                    }
                    else
                    {
                        attacks.Add($"{costEmoji} {attack.Name} *{attack.Damage}*", attackText);
                    }
                }
            }

            var msg = BuildBaseDiscordEmbed(title, cardText.CardInfo);
            msg.WithThumbnail(cardText.Image.AbsoluteUri);
            
            foreach(var ability in abilities)
            {
                msg.AddField(ability.Key, ability.Value);
            }

            foreach(var attack in attacks)
            {
                msg.AddField(attack.Key, attack.Value);
            }

            var cardWeakness = string.Empty;
            if(cardText.Weaknesses is not null && cardText.Weaknesses.Count is not 0)
            {
                foreach (var weakness in cardText.Weaknesses)
                {
                    cardWeakness += _typeEmoji[weakness.Type] + weakness.Value;
                }
            }
            else
            {
                cardWeakness = "\u0000";
            }

            msg.AddField("weakness", cardWeakness, true);

            var cardResistance = string.Empty;
            if(cardText.Resistances is not null && cardText.Resistances.Count is not 0)
            {
                foreach (var resistance in cardText.Resistances)
                {
                    cardResistance += _typeEmoji[resistance.Type] + resistance.Value;
                }
            }
            else
            {
                cardResistance = "\u0000";
            }

            msg.AddField("resistance", cardResistance, true);

            var retreatCost = string.Empty;
            if(cardText.RetreatCost is not null && cardText.RetreatCost.Count is not 0)
            {
                foreach (var cost in cardText.RetreatCost)
                {
                    retreatCost += _typeEmoji[cost];
                }
            }
            else
            {
                retreatCost = "\u0000";
            }

            msg.AddField("retreat", retreatCost, true);

            msg.Build();

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private async Task SendPriceMessageAsync(MessageCreateEventArgs e)
        {
            var priceName = RegexValidator.GetPriceName(e.Message.Content);

            var cardPrices = await pokemonTcgService.GetPricesAsync(priceName);
            var title = $"Prices for {cardPrices.CardInfo.Name} ({cardPrices.CardInfo.Number})";
            var msg = BuildBaseDiscordEmbed(title, cardPrices.CardInfo);

            var tcgPlayerPrices = string.Empty;
            foreach (var tcgPlayerPrice in cardPrices.TcgPlayerPrices)
            {
                tcgPlayerPrices = tcgPlayerPrice.Key + ": " + tcgPlayerPrice.Value.ToString("C2") + "\n";
            }

            var cardMarketPrices = string.Empty;
            foreach (var cardMarketPrice in cardPrices.CardMarketPrices)
            {
                if (cardMarketPrice.Value > 0M)
                {
                    //Using French culture info for Euro formatting
                    cardMarketPrices += cardMarketPrice.Key + ": " + cardMarketPrice.Value.ToString("C2", new CultureInfo("fr-FR")) + "\n";
                }
            }

            msg.AddField("TCGPlayer", tcgPlayerPrices)
               .AddField("CardMarket", cardMarketPrices)
               .Build();


            await e.Channel.SendMessageAsync(embed: msg);
        }

        private async Task SendLegalityMessageAsync(MessageCreateEventArgs e)
        {
            var legalityName = RegexValidator.GetLegalityName(e.Message.Content);

            var cardLegalities = await pokemonTcgService.GetLegalitiesAsync(legalityName);
            var legalities = cardLegalities.Legalities;

            var isStandardLegal = DetermineLegality(legalities.Standard);
            var isExpandedLegal = DetermineLegality(legalities.Expanded);
            var isUnlimitedLegal = DetermineLegality(legalities.Unlimited);

            var description = BuildLegalityDescription(isStandardLegal, nameof(legalities.Standard))
                            + BuildLegalityDescription(isExpandedLegal, nameof(legalities.Expanded))
                            + BuildLegalityDescription(isUnlimitedLegal, nameof(legalities.Unlimited));

            var title = $"{cardLegalities.CardInfo.Name} ({cardLegalities.CardInfo.Number}) Legality";

            var msg = BuildBaseDiscordEmbed(title, cardLegalities.CardInfo, description);

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private static async Task SendErrorMessageAsync(MessageCreateEventArgs e, CardNotFoundException ex)
        {
            await e.Channel.SendMessageAsync($"{ex.Message}");
        }

        private static DiscordEmbedBuilder BuildBaseDiscordEmbed(string title, CardInfo cardInfo, string? description = null)
        {
            return new DiscordEmbedBuilder
            {
                Color = DiscordColor.Orange,
                Title = title,
                Description = description,
                Footer = new EmbedFooter
                {
                    IconUrl = cardInfo.SetSymbolUri.AbsoluteUri,
                    Text = cardInfo.SetName
                }
            };
        }

        private static string BuildLegalityDescription(bool isLegal, string formatName)
        {
            return _legalityEmojis[isLegal] + " " + formatName + "\n";
        }

        private static bool DetermineLegality(string legality)
        {
            return string.Equals(legality, "Legal");
        }

        private static string ReplaceEnergyEmoji(string text)
        {
            foreach (var typeEmoji in _typeEmoji)
            {
                text = text.Replace(typeEmoji.Key, typeEmoji.Value);
            }

            return text;
        }
    }
}