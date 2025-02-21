using Microsoft.AspNetCore.Mvc;
using System;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.Repositories.Interfaces;

namespace URLShortener.Controllers
{
    [Route("api")]
    [ApiController]
    public class UrlShortenerController : ControllerBase
    {
        private readonly IUrlRepository _urlRepository;
        private const string BaseUrl = "http://localhost:5000/";

        public UrlShortenerController(IUrlRepository urlRepository)
        {
            _urlRepository = urlRepository ?? throw new ArgumentNullException(nameof(urlRepository));
        }

        // POST: api/shorten
        [HttpPost("shorten")]
        public IActionResult ShortenUrl([FromBody] UrlRequest request)
        {
            if (request == null || !IsValidUrl(request.OriginalUrl))
            {
                return BadRequest("Invalid URL.");
            }

            var shortId = _urlRepository.GenerateShortId();
            var urlMapping = CreateUrlMapping(request.OriginalUrl, shortId);

            _urlRepository.AddUrlMapping(shortId, urlMapping);

            return Ok(CreateUrlResponse(shortId));
        }

        // GET: api/{shortId}
        [HttpGet("{shortId}")]
        public IActionResult ResolveUrl(string shortId)
        {
            var urlMapping = _urlRepository.GetUrlMapping(shortId);
            if (urlMapping == null)
            {
                return NotFound("URL not found.");
            }

            return Ok(urlMapping.OriginalUrl);
        }

        // Helper method to validate the URL
        private bool IsValidUrl(string url)
        {
            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        // Helper method to generate a random short ID
       

        // Helper method to create a URL mapping object
        private UrlMapping CreateUrlMapping(string originalUrl, string shortId)
        {
            return new UrlMapping
            {
                OriginalUrl = originalUrl,
                ShortId = shortId
            };
        }

        // Helper method to create a URL response with the shortened URL
        private UrlResponse CreateUrlResponse(string shortId)
        {
            return new UrlResponse
            {
                ShortenedUrl = $"{BaseUrl}{shortId}",
                ShortId = shortId
            };
        }
    }
}
