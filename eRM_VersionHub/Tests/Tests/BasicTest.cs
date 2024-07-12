using Microsoft.AspNetCore.Mvc.Testing;

namespace eRM_VersionHub_Tester.Tests
{
    public class BasicTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory = factory;

        [Theory]
        [InlineData("")]

        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            Uri baseUrl = new("http://localhost:5204");
            HttpClient client = _factory.CreateDefaultClient(baseUrl);

            // Act
            HttpResponseMessage response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.Equal("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }
    }
}