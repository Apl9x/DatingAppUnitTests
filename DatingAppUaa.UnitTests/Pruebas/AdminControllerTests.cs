using DatingApp.Api.DTOs;
using DatingAppUaa.UnitTests.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DatingAppUaa.UnitTests.Pruebas
{
    public class AdminControllerTests
    {
        private string apiRoute = "api/admin";
        private readonly HttpClient _client;
        private HttpResponseMessage httpResponse;
        private string requestUri;
        private string registeredObject;
        private HttpContent httpContent;

        public AdminControllerTests()
        {
            _client = TestHelper.Instance.Client;
        }
        [Theory]
        [InlineData("Unauthorized")]
        public async Task GetUsersWithRoles_ShouldFail(string statusCode)
        {
            // Arrange
            requestUri = $"{apiRoute}/users-with-roles";
            httpContent = GetHttpContent("");

            // Act
            httpResponse = await _client.GetAsync(requestUri);

            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        [Theory]
        [InlineData("OK", "admin", "Pa$$w0rd")]
        public async Task GetUsersWithRoles_ShouldReturnOK(string statusCode, string username, string password)
        {
            // Arrange
            var loginDto = new LoginDto
            {
                Username = username,
                Password = password
            };
            registeredObject = GetRegisterObject(loginDto);
            httpContent = GetHttpContent(registeredObject);
            var result = await _client.PostAsync("api/account/login", httpContent);
            var userJson = await result.Content.ReadAsStringAsync();
            var user = userJson.Split(',');
            var token = user[1].Split("\"")[3];
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            requestUri = $"{apiRoute}/users-with-roles";

            // Act
            httpResponse = await _client.GetAsync(requestUri);

            // Assert
            Assert.Equal(Enum.Parse<HttpStatusCode>(statusCode, true), httpResponse.StatusCode);
            Assert.Equal(statusCode, httpResponse.StatusCode.ToString());
        }

        #region Privated methods
        private static string GetRegisterObject(LoginDto loginDto)
        {
            var entityObject = new JObject()
            {
                { nameof(loginDto.Username), loginDto.Username },
                { nameof(loginDto.Password), loginDto.Password }
            };
            return entityObject.ToString();
        }
        private StringContent GetHttpContent(string objectToEncode)
        {
            return new StringContent(objectToEncode, Encoding.UTF8, "application/json");
        }

        #endregion
    }
}
