using BoxOffice.Business.Models;
using BoxOffice.DTO.AddRequest;
using BoxOffice.DTO.PostResponse;
using BoxOffice.DTO.UpdateRequest;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace BoxOffice.WebApi.Test
{
    public class AuthorsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public AuthorsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            factory.ResetDatabase();
        }

        [Fact]
        public async Task Get_ProperList()
        {
            var response = await _httpClient.GetAsync("api/authors");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<AuthorModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedAuthors();

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task GetById_ProperModel()
        {
            int testId = 2;

            var response = await _httpClient.GetAsync($"api/authors/{testId}");
            var actual = await response.Content.ReadFromJsonAsync<AuthorModel>();
            var expected = ExpectedValuesGenerator.GetExpectedAuthors().First(x => x.Id == testId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task GetById_InvalidId()
        {
            int testId = 99;

            var response = await _httpClient.GetAsync($"api/authors/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ProperModelSaved()
        {
            var model = new AddPersonRequest()
            {
                FirstName = "Petro",
                LastName = "Oleksa",
                BirthDate = DateTime.Parse("2007-01-01"),
                Email = "manonamission@gmail.com"
            };

            var response = await _httpClient.PostAsJsonAsync($"api/authors", model);
            var created = await response.Content.ReadFromJsonAsync<PostPersonResponse>();
            var persisted = await _httpClient.GetFromJsonAsync<AuthorModel>($"/api/authors/{created!.Id}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created),
                () => created.FirstName.Should().Be(persisted!.FirstName),
                () => created.LastName.Should().Be(persisted!.LastName),
                () => created.BirthDate.Should().Be(persisted!.BirthDate),
                () => created.Email.Should().Be(persisted!.Email)
            );
        }

        [Fact]
        public async Task Post_BadRequest()
        {
            var model = new AddPersonRequest()
            {
                FirstName = " ",
                LastName = "Oleksa",
                BirthDate = DateTime.Parse("2007-01-01"),
                Email = "manonamission@gmail.com"
            };

            var response = await _httpClient.PostAsJsonAsync($"api/authors", model);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_SavedProperly()
        {
            int testId = 1;
            var model = new UpdatePersonRequest()
            {
                Id = 1,
                FirstName = "Petro",
                LastName = "Oleksa",
                BirthDate = DateTime.Parse("2007-01-01"),
                Email = "manonamission@gmail.com"
            };

            var response = await _httpClient.PutAsJsonAsync($"api/authors/{testId}", model);
            var persisted = await _httpClient.GetFromJsonAsync<AuthorModel>($"/api/authors/{testId}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => persisted!.FirstName.Should().Be(model.FirstName),
                () => persisted!.LastName.Should().Be(model.LastName),
                () => persisted!.BirthDate.Should().Be(model.BirthDate),
                () => persisted!.Email.Should().Be(model.Email)
            );
        }

        [Fact]
        public async Task Put_BadRequest()
        {
            int testId = 1;
            var model = new UpdatePersonRequest()
            {
                Id = 2,
                FirstName = "Petro",
                LastName = "Oleksa",
                BirthDate = DateTime.Parse("2007-01-01"),
                Email = "manonamission@gmail.com"
            };

            var response = await _httpClient.PutAsJsonAsync($"api/authors/{testId}", model);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_SuccessfulDeletion()
        {
            int testId = 3; 

            var response = await _httpClient.DeleteAsync($"api/authors/{testId}");
            var actual = await _httpClient.GetFromJsonAsync<IEnumerable<AuthorModel>>($"/api/authors");
            var expected = ExpectedValuesGenerator.GetExpectedAuthors().Where(x => x.Id != testId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }
    }
}
