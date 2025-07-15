using FluentAssertions;
using System.Net.Http.Json;
using UserService.BLL.DTOs.Request;

namespace UserService.Tests.FunctionalTests.Controllers
{
    [CollectionDefinition("AuthE2E", DisableParallelization = true)]
    public class AuthControllerTests : IClassFixture<FunctionalTestWebAppFactory>
    {
        private readonly HttpClient _httpClient;

        public AuthControllerTests(FunctionalTestWebAppFactory factory)
        {
            _httpClient = factory.CreateClient();
        }

        [Fact]
        public async Task Register_WhenInvalidFieldsProvided_ReturnsBadRequest()
        {
            //Arrange
            var request = new RegisterRequestDTO()
            {
                UserName = "test",
                Email = "test@example.com",
                Password = "Password123!",
                PhoneNumber = "not a phone number",
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Register_WhenValidFieldsProvided_ReturnsOk()
        {
            //Arrange
            var request = new RegisterRequestDTO()
            {
                UserName = "test",
                Email = "test@example.com",
                Password = "Password123!",
                PhoneNumber = "+375291111111",
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_WhenValidCredentialsProvided_ReturnsOK()
        {
            //Arrange
            var request = new LoginRequestDTO()
            {
                Email = "initial@test.com",
                Password = "Password123!",
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task Login_WhenInvalidValidCredentialsProvided_ReturnsUnauthorized()
        {
            //Arrange
            var request = new LoginRequestDTO()
            {
                Email = "initial@test.com",
                Password = "Qwerty123!",
            };

            //Act
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", request);

            //Assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Unauthorized);
        }
    }
}
