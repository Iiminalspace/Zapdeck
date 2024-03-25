namespace Zapdeck.Bot
{
    public interface IBot
    {
        Task StartAsync();
        Task StopAsync();
    }
}