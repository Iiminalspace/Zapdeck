using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients.Cards.Models;
using System.Globalization;
using Zapdeck.Exceptions;
using Zapdeck.Helpers;
using Zapdeck.Models.PokemonTcg;
using static DSharpPlus.Entities.DiscordEmbedBuilder;

namespace Zapdeck.Modules.PokemonTcg
{
    public class PokemonTcgModule(IPokemonTcgService pokemonTcgService) : IModule
    {
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
            catch(Exception)
            {
                throw;
            }
            
        }

        private async Task SendImageMessage(MessageCreateEventArgs e)
        {
            var imageArgs = RegexValidator.GetImageName(e.Message.Content);

            var imageUri = await pokemonTcgService.GetImageUriAsync(imageArgs);

            var title = FormatCardName(imageUri.CardInfo);

            var msg = BuildBaseDiscordEmbed(title, imageUri.CardInfo);

            msg.WithImageUrl(imageUri.Uri.AbsoluteUri).Build();

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private async Task SendCardTextMessageAsync(MessageCreateEventArgs e)
        {
            var cardTextNameArgs = RegexValidator.GetCardTextName(e.Message.Content);

            var cardText = await pokemonTcgService.GetCardTextAsync(cardTextNameArgs);

            var msg = cardText.Supertype.Equals("Pokémon") ? BuildPokemonCard(cardText) : BuildCard(cardText);

            await e.Channel.SendMessageAsync(embed: msg);
        }

        private async Task SendPriceMessageAsync(MessageCreateEventArgs e)
        {
            var priceNameArgs = RegexValidator.GetPriceName(e.Message.Content);

            var cardPrices = await pokemonTcgService.GetPricesAsync(priceNameArgs);

            var cardName = FormatCardName(cardPrices.CardInfo);

            var title = $"Prices for {cardName}";
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
            var legalityNameArgs = RegexValidator.GetLegalityName(e.Message.Content);

            var cardLegalities = await pokemonTcgService.GetLegalitiesAsync(legalityNameArgs);
            var legalities = cardLegalities.Legalities;

            var isStandardLegal = DetermineLegality(legalities.Standard);
            var isExpandedLegal = DetermineLegality(legalities.Expanded);
            var isUnlimitedLegal = DetermineLegality(legalities.Unlimited);

            var description = BuildLegalityDescription(isStandardLegal, nameof(legalities.Standard))
                            + BuildLegalityDescription(isExpandedLegal, nameof(legalities.Expanded))
                            + BuildLegalityDescription(isUnlimitedLegal, nameof(legalities.Unlimited));

            var title = FormatCardName(cardLegalities.CardInfo) + " Legality";

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

        private static DiscordEmbedBuilder BuildCard(CardText cardText)
        {
            var cardName = FormatCardName(cardText.CardInfo);
            var description = string.Empty;
            if (cardText.Rules.Count is 1)
            {
                description = ReplaceEnergyEmoji(cardText.Rules.First());
            }
            else if (cardText.Rules.Count >= 2)
            {
                var rules = cardText.Rules.SkipLast(1);
                foreach (var rule in rules)
                {
                    description += ReplaceEnergyEmoji(rule) + "\n";
                }
            }

            var msg = BuildBaseDiscordEmbed(cardName, cardText.CardInfo, description);
            msg.WithThumbnail(cardText.Image.AbsoluteUri).Build();

            return msg;
        }

        private static DiscordEmbedBuilder BuildPokemonCard(CardText cardText)
        {
            var cardName = FormatCardName(cardText.CardInfo);
            var hp = "HP" + cardText.Hp + " " + CostToEmoji(cardText.Types);

            var title = $"{cardName} — {hp}";

            var abilities = new Dictionary<string, string>();

            if (cardText.Abilities.Count is not 0)
            {
                foreach (var ability in cardText.Abilities)
                {
                    var abilityText = ReplaceEnergyEmoji(ability.Text);
                    abilities.Add($"{ability.Type} — {ability.Name}", abilityText);
                }
            }

            var attacks = new Dictionary<string, string>();

            if (cardText.Attacks.Count is not 0)
            {
                foreach (var attack in cardText.Attacks)
                {
                    if (string.IsNullOrEmpty(attack.Text))
                    {
                        //Set to a value to pass to embed builder
                        attack.Text = "\u0000";
                    }

                    var attackCost = CostToEmoji(attack.Cost);

                    var attackText = ReplaceEnergyEmoji(attack.Text);

                    if (string.IsNullOrEmpty(attack.Damage))
                    {
                        attacks.Add($"{attackCost} {attack.Name}", attackText);
                    }
                    else
                    {
                        attacks.Add($"{attackCost} {attack.Name} *{attack.Damage}*", attackText);
                    }
                }
            }

            var msg = BuildBaseDiscordEmbed(title, cardText.CardInfo);
            msg.WithThumbnail(cardText.Image.AbsoluteUri);

            foreach (var ability in abilities)
            {
                msg.AddField(ability.Key, ability.Value);
            }

            foreach (var attack in attacks)
            {
                msg.AddField(attack.Key, attack.Value);
            }

            var weakness = FormatResistance(cardText.Weaknesses);
            msg.AddField("weakness", weakness, true);

            var resistance = FormatResistance(cardText.Resistances);
            msg.AddField("resistance", resistance, true);

            var retreatCost = CostToEmoji(cardText.RetreatCost);
            msg.AddField("retreat", retreatCost, true);

            msg.Build();

            return msg;
        }

        private static string CostToEmoji(List<string> costs)
        {
            var costEmoji = string.Empty;
            if(costs.Count is not 0) 
            {
                foreach (var cost in costs)
                {
                    costEmoji += _typeEmoji[cost];
                }
            }
            else
            {
                costEmoji = "\u0000";
            }

            return costEmoji;
        }

        private static bool DetermineLegality(string legality)
        {
            return string.Equals(legality, "Legal");
        }

        private static string FormatCardName(CardInfo cardInfo)
        {
            return $"{cardInfo.Name} ({cardInfo.SetCode} {cardInfo.Number})";
        }
        private static string FormatResistance(List<Resistance> resistances)
        {
            var resistanceEmoji = string.Empty;
            if (resistances.Count is not 0)
            {
                foreach (var resistance in resistances)
                {
                    resistanceEmoji += _typeEmoji[resistance.Type] + resistance.Value;
                }
            }
            else
            {
                resistanceEmoji = "\u0000";
            }

            return resistanceEmoji;
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