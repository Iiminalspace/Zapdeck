﻿namespace Zapdeck.Models.PokemonTcg
{
    public record CardPrices(Dictionary<string, double> TcgPlayerPrices, Dictionary<string, decimal> CardMarketPrices, CardInfo CardInfo);
}
