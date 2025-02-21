namespace UrlShortener.Models
{
    public class UrlResponse
    {
        public required string ShortenedUrl { get; set; }
        public required string ShortId { get; set; }
    }
}
