using System.Xml.Serialization;
using UrlShortener.Data;

namespace UrlShortener.Services
{
    public interface IUrlShortenerService
    {
        void AddUrlMapping(string shortId, UrlMapping urlMapping);
        UrlMapping GetUrlMapping(string shortId);
    }

}
