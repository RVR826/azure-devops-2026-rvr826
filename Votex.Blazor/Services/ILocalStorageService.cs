
namespace Votex.Blazor.Services
{
    public interface ILocalStorageService
    {
        Task<string> GetItemAsync(string key);
        Task RemoveItemAsync(string key);
        Task SetItemAsync(string key, string value);
    }
}