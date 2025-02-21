using UrlShortener.Data;

namespace UrlShortener.Repositories.Interfaces
{
    public interface IUrlRepository
    {
        void AddUrlMapping(string shortId, UrlMapping urlMapping);
        UrlMapping? GetUrlMapping(string shortId);
        string GenerateShortId();
    }
}
