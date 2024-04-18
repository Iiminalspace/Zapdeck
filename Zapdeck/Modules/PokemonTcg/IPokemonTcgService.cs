namespace Zapdeck.Modules.PokemonTcg
{
    public interface IPokemonTcgService
    {
        Task<Uri?> GetImageUriAsync(string cardName);
    }
}
