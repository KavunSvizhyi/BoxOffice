using BoxOffice.Business.Models;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace BoxOffice.WebApi.Test
{
    public class StatisticsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public StatisticsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            factory.ResetDatabase();
        }

        [Fact]
        public async Task GetMostSuccessfulAuthors_ProperList()
        {
            var response = await _httpClient.GetAsync("api/statistics/most-successful-authors?count=3");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<AuthorActivityModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedAuthorActivityModel();

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering())
            );
        }

        [Fact]
        public async Task GetMostValuableCustomers_ProperList()
        {
            var response = await _httpClient.GetAsync("api/statistics/most-valuable-customers?count=3");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<CustomerActivityModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedCustomerActivityModel();

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().BeEquivalentTo(expected, options => options.WithStrictOrdering())
            );
        }

        [Fact]
        public async Task GetIncomeInPeriod_ProperValue()
        {
            int genreId = 4;
            decimal expected = 750;

            var response = await _httpClient.GetAsync($"api/statistics/income-of-genre/{genreId}?startDate=2025-01-01&endDate=2026-12-01");
            var actual = await response.Content.ReadFromJsonAsync<decimal>();

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().Be(expected)
            );
        }
    }
}
