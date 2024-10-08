﻿using System.Configuration;
using System.Reflection;
using DSharpPlus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PokemonTcgSdk.Standard.Infrastructure.HttpClients;
using Zapdeck.Bot;
using Zapdeck.Modules;
using Zapdeck.Modules.PokemonTcg;

namespace Zapdeck
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddUserSecrets(Assembly.GetExecutingAssembly())
                .Build();

            var discordConfig = new DiscordConfiguration
                {
                    Token = configuration["DiscordToken"] ?? throw new ConfigurationErrorsException("Missing Discord token."),
                    AutoReconnect = true,
                    Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
                    TokenType = TokenType.Bot
                };
            
            var discordClient = new DiscordClient(discordConfig);

            var pokeClientKey = configuration["PokemonTcgApiKey"] ?? throw new ConfigurationErrorsException("Missing PokemonTcg API key.");

            var pokeClient = new PokemonApiClient(pokeClientKey);

            var serviceProvider = new ServiceCollection()
                .AddLogging(options =>
                {
                    options.ClearProviders();
                    options.AddConsole();
                })
                .AddSingleton<IConfiguration>(configuration)
                .AddSingleton(discordClient)
                .AddSingleton(pokeClient)
                .AddSingleton<IBot, ZapdeckBot>()
                .AddSingleton<IModule, PokemonTcgModule>()
                .AddSingleton<IPokemonTcgService, PokemonTcgService>()
                .BuildServiceProvider();

            
            try
            {
                var bot = serviceProvider.GetRequiredService<IBot>();

                await bot.StartAsync();
                
                //Keep appplication running until user presses "Q"
                do
                {
                    var keyInfo = Console.ReadKey();

                    if (keyInfo.Key == ConsoleKey.Q)
                    {
                        Console.WriteLine("\nShutting down");

                        await bot.StopAsync();
                        return;
                    }
                } while (true);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Environment.Exit(-1);
            }
        }
    }
}