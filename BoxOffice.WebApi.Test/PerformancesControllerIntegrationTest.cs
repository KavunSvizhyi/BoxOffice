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
    public class PerformancesControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public PerformancesControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            factory.ResetDatabase();
        }

        [Fact]
        public async Task Get_ProperList()
        {
            var response = await _httpClient.GetAsync("api/performances");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<PerformanceModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedPerformances();

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

            var response = await _httpClient.GetAsync($"api/performances/{testId}");
            var actual = await response.Content.ReadFromJsonAsync<PerformanceModel>();
            var expected = ExpectedValuesGenerator.GetExpectedPerformances().First(x => x.Id == testId);

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

            var response = await _httpClient.GetAsync($"api/performances/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ProperModelSaved()
        {
            var model = new AddPerformanceRequest()
            {
                PerformanceName = "Taras Bul'ba",
                AuthorId = 1,
                GenreId = 4,
                EventDate = DateTime.Parse("2026-11-05T19:00:00")
            };

            var response = await _httpClient.PostAsJsonAsync($"api/performances", model);
            var created = await response.Content.ReadFromJsonAsync<PostPerformanceResponse>();
            var persisted = await _httpClient.GetFromJsonAsync<PerformanceModel>($"/api/performances/{created!.Id}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created),
                () => created.PerformanceName.Should().Be(persisted!.PerformanceName)
            );
        }

        [Fact]
        public async Task Post_BadRequest()
        {
            var model = new AddPerformanceRequest()
            {
                PerformanceName = " ",
                AuthorId = 1,
                GenreId = 4,
                EventDate = DateTime.Parse("2026-11-05T19:00:00")
            };

            var response = await _httpClient.PostAsJsonAsync($"api/performances", model);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_SavedProperly()
        {
            int testId = 1;
            var model = new UpdatePerformanceRequest()
            {
                Id = 1,
                PerformanceName = "Taras Bul'ba",
                AuthorId = 1,
                GenreId = 4,
                EventDate = DateTime.Parse("2026-11-05T19:00:00")
            };

            var response = await _httpClient.PutAsJsonAsync($"api/performances/{testId}", model);
            var persisted = await _httpClient.GetFromJsonAsync<PerformanceModel>($"/api/performances/{testId}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => persisted!.PerformanceName.Should().Be(model.PerformanceName)
            );
        }

        [Fact]
        public async Task Put_BadRequest()
        {
            int testId = 1;
            var model = new UpdatePerformanceRequest()
            {
                Id = 2,
                PerformanceName = "Taras Bul'ba",
                AuthorId = 1,
                GenreId = 4,
                EventDate = DateTime.Parse("2026-11-05T19:00:00")
            };

            var response = await _httpClient.PutAsJsonAsync($"api/performances/{testId}", model);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_SuccessfulDeletion()
        {
            int performanceId = 5;
            int ticketIdToDelete1 = 9;
            int ticketIdToDelete2 = 10;

            await _httpClient.DeleteAsync($"api/tickets/{ticketIdToDelete1}");
            await _httpClient.DeleteAsync($"api/tickets/{ticketIdToDelete2}");

            var response = await _httpClient.DeleteAsync($"api/performances/{performanceId}");
            var actual = await _httpClient.GetFromJsonAsync<IEnumerable<PerformanceModel>>($"/api/performances");
            var expected = ExpectedValuesGenerator.GetExpectedPerformances().Where(x => x.Id != performanceId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task Delete_BadRequest()
        {
            int testId = 3;

            var response = await _httpClient.DeleteAsync($"api/performances/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAvailable_ProperList()
        {
            int testId = 2;

            var response = await _httpClient.GetAsync("api/performances/available");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<PerformanceModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedPerformances()
                .Where(x => x.Id != testId || x.EventDate < DateTime.Now);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task GetByFilter_ProperList()
        {
            var testId = 1;
            var filter = new SearchFilterModel()
            {
                PerformanceName = "Hamlet"
            };

            var response = await _httpClient.GetAsync("api/performances/by-filter?PerformanceName=Hamlet");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<PerformanceModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedPerformances().Where(x => x.Id == testId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }
    }
}
