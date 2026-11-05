using BoxOffice.Business.Models;
using BoxOffice.DTO.AddRequest;
using BoxOffice.DTO.PostResponse;
using BoxOffice.DTO.UpdateRequest;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace BoxOffice.WebApi.Test
{
    public class GenresControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public GenresControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            factory.ResetDatabase();
        }

        [Fact]
        public async Task Get_ProperList()
        {
            var response = await _httpClient.GetAsync("api/genres");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<GenreModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedGenres();

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

            var response = await _httpClient.GetAsync($"api/genres/{testId}");
            var actual = await response.Content.ReadFromJsonAsync<GenreModel>();
            var expected = ExpectedValuesGenerator.GetExpectedGenres().First(x => x.Id == testId);

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

            var response = await _httpClient.GetAsync($"api/genres/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ProperModelSaved()
        {
            var model = new AddGenreRequest()
            {
                GenreName = "Petro"
            };

            var response = await _httpClient.PostAsJsonAsync($"api/genres", model);
            var created = await response.Content.ReadFromJsonAsync<PostGenreResponse>();
            var persisted = await _httpClient.GetFromJsonAsync<GenreModel>($"/api/genres/{created!.Id}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created),
                () => created.GenreName.Should().Be(persisted!.GenreName)
            );
        }

        [Fact]
        public async Task Post_BadRequest()
        {
            var model = new AddGenreRequest()
            {
                GenreName = " "
            };

            var response = await _httpClient.PostAsJsonAsync($"api/genres", model);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_SavedProperly()
        {
            int testId = 1;
            var model = new UpdateGenreRequest()
            {
                Id = 1,
                GenreName = "Petro"
            };

            var response = await _httpClient.PutAsJsonAsync($"api/genres/{testId}", model);
            var persisted = await _httpClient.GetFromJsonAsync<GenreModel>($"/api/genres/{testId}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => persisted!.GenreName.Should().Be(model.GenreName)
            );
        }

        [Fact]
        public async Task Put_BadRequest()
        {
            int testId = 1;
            var model = new UpdateGenreRequest()
            {
                Id = 2,
                GenreName = "Petro"
            };

            var response = await _httpClient.PutAsJsonAsync($"api/genres/{testId}", model);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_SuccessfulDeletion()
        {
            int testId = 3;

            var response = await _httpClient.DeleteAsync($"api/genres/{testId}");
            var actual = await _httpClient.GetFromJsonAsync<IEnumerable<GenreModel>>($"/api/genres");
            var expected = ExpectedValuesGenerator.GetExpectedGenres().Where(x => x.Id != testId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task Delete_BadRequest()
        {
            int testId = 4;

            var response = await _httpClient.DeleteAsync($"api/genres/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
