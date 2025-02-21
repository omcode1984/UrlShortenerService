namespace UrlShortener.Data
{
    public class UrlMapping
    {
        public required string ShortId { get; set; }
        public required string OriginalUrl { get; set; }
    }
}
