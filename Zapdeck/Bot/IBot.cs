using Microsoft.Extensions.DependencyInjection;

namespace Zapdeck.Bot
{
    public interface IBot
    {
        Task StartAsync();
        Task StopAsync();
    }
}