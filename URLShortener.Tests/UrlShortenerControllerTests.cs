using Moq;
using System;
using URLShortener.Controllers;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using UrlShortener.Data;
using UrlShortener.Models;
using UrlShortener.Repositories.Interfaces;
using System.Xml.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;

namespace URLShortener.Tests
{
    public class UrlShortenerControllerTests
    {
        private readonly Mock<IUrlRepository> _mockUrlRepository;
        private readonly UrlShortenerController _controller;

        public UrlShortenerControllerTests()
        {
            _mockUrlRepository = new Mock<IUrlRepository>();
            _controller = new UrlShortenerController(_mockUrlRepository.Object);
        }

        // Test for valid URL shortening
        [Fact]
        public void ShortenUrl_WhenGivenValidUrl_ReturnsShortenedUrl()
        {
            // Arrange
            var originalUrl = "https://www.example.com";
            var urlRequest = CreateValidUrlRequest(originalUrl);
            var expectedShortId = "abc123";
            SetupUrlRepositoryToReturnShortenedUrl(originalUrl, expectedShortId);
            SetupUrlRepositoryToReturnShortenedId(expectedShortId);

            // Act
            var result = _controller.ShortenUrl(urlRequest) as OkObjectResult;

            // Assert
            _mockUrlRepository.Verify(repo => repo.AddUrlMapping(It.IsAny<string>(), It.IsAny<UrlMapping>()), Times.Once);
            AssertShortenedUrlResponse(result, originalUrl, expectedShortId);
           
        }

        // Test for invalid URL shortening
        [Fact]
        public void ShortenUrl_WhenGivenInvalidUrl_ReturnsBadRequest()
        {
            // Arrange
            var invalidUrlRequest = new UrlRequest { OriginalUrl = "invalid_url" };

            // Act
            var result = _controller.ShortenUrl(invalidUrlRequest) as BadRequestObjectResult;

            // Assert
            AssertBadRequest(result);
        }

        // Test for resolving an existing short URL
        [Fact]
        public void ResolveUrl_WhenShortIdExists_ReturnsRedirectToOriginalUrl()
        {
            // Arrange
            var shortId = "abc123";
            var originalUrl = "https://www.example.com";
            SetupUrlRepositoryToReturnOriginalUrl(shortId, originalUrl);

            // Act
            var result = _controller.ResolveUrl(shortId) as OkObjectResult;

            // Assert
            AssertRedirectResult(result, originalUrl);
        }

        // Test for resolving a non-existing short URL
        [Fact]
        public void ResolveUrl_WhenShortIdDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var nonExistingShortId = "nonexistent123";
            SetupUrlRepositoryToReturnNullForShortId(nonExistingShortId);

            // Act
            var result = _controller.ResolveUrl(nonExistingShortId) as NotFoundObjectResult;

            // Assert
            AssertNotFound(result);
        }

        // Helper method to create a valid URL request
        private UrlRequest CreateValidUrlRequest(string url)
        {
            return new UrlRequest { OriginalUrl = url };
        }

        // Helper method to set up the repository to return a valid mapping
        private void SetupUrlRepositoryToReturnShortenedUrl(string originalUrl, string shortId)
        {
            var urlMapping = new UrlMapping { OriginalUrl = originalUrl, ShortId = shortId };
            _mockUrlRepository.Setup(repo => repo.AddUrlMapping(shortId, urlMapping)).Verifiable();
        }
        private void SetupUrlRepositoryToReturnShortenedId(string shortId)
        {
            _mockUrlRepository.Setup(repo => repo.GenerateShortId()).Returns(shortId);
        }

        // Helper method to set up the repository to return the original URL for a short ID
        private void SetupUrlRepositoryToReturnOriginalUrl(string shortId, string originalUrl)
        {
            var urlMapping = new UrlMapping { OriginalUrl = originalUrl, ShortId = shortId };
            _mockUrlRepository.Setup(repo => repo.GetUrlMapping(shortId)).Returns(urlMapping);
        }

        // Helper method to set up the repository to return null for a non-existent short ID
        private void SetupUrlRepositoryToReturnNullForShortId(string shortId)
        {
            _mockUrlRepository.Setup(repo => repo.GetUrlMapping(shortId)).Returns((UrlMapping)null);
        }

        // Helper method to assert the shortened URL response
        private void AssertShortenedUrlResponse(OkObjectResult result, string originalUrl, string expectedShortId)
        {
            Assert.NotNull(result);
            var response = result.Value as UrlResponse;
            Assert.Equal($"http://localhost:5000/{expectedShortId}", response.ShortenedUrl);
            Assert.Equal(expectedShortId, response.ShortId);
        }

        // Helper method to assert the BadRequest response
        private void AssertBadRequest(BadRequestObjectResult result)
        {
            Assert.NotNull(result);
            Assert.Equal("Invalid URL.", result.Value);
        }

        // Helper method to assert the RedirectResult
        private void AssertRedirectResult(OkObjectResult result, string expectedUrl)
        {
            Assert.NotNull(result);
            Assert.Equal(expectedUrl, result.Value);
        }

        // Helper method to assert the NotFoundObjectResult
        private void AssertNotFound(NotFoundObjectResult result)
        {
            Assert.NotNull(result);
            Assert.Equal("URL not found.", result.Value);
        }
    }
}
