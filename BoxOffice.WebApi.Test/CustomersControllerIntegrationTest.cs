using BoxOffice.Business.Models;
using BoxOffice.DTO.AddRequest;
using BoxOffice.DTO.PostResponse;
using BoxOffice.DTO.UpdateRequest;
using FluentAssertions;
using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace BoxOffice.WebApi.Test
{
    public class CustomersControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public CustomersControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            factory.ResetDatabase();
        }

        [Fact]
        public async Task Get_ProperList()
        {
            var response = await _httpClient.GetAsync("api/customers");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<CustomerModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedCustomers();

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

            var response = await _httpClient.GetAsync($"api/customers/{testId}");
            var actual = await response.Content.ReadFromJsonAsync<CustomerModel>();
            var expected = ExpectedValuesGenerator.GetExpectedCustomers().First(x => x.Id == testId);

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

            var response = await _httpClient.GetAsync($"api/customers/{testId}");

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

            var response = await _httpClient.PostAsJsonAsync($"api/customers", model);
            var created = await response.Content.ReadFromJsonAsync<PostPersonResponse>();
            var persisted = await _httpClient.GetFromJsonAsync<CustomerModel>($"/api/customers/{created!.Id}");

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

            var response = await _httpClient.PostAsJsonAsync($"api/customers", model);

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

            var response = await _httpClient.PutAsJsonAsync($"api/customers/{testId}", model);
            var persisted = await _httpClient.GetFromJsonAsync<CustomerModel>($"/api/customers/{testId}");

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

            var response = await _httpClient.PutAsJsonAsync($"api/customers/{testId}", model);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_SuccessfulDeletion()
        {
            int testId = 3;

            var response = await _httpClient.DeleteAsync($"api/customers/{testId}");
            var actual = await _httpClient.GetFromJsonAsync<IEnumerable<CustomerModel>>($"/api/customers");
            var expected = ExpectedValuesGenerator.GetExpectedCustomers().Where(x => x.Id != testId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task GetByPerformanceId_ProperList()
        {
            int performanceId = 1;
            int customerId = 1;

            var response = await _httpClient.GetAsync($"api/customers/by-performance/{performanceId}");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<CustomerModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedCustomers().Where(x => x.Id == customerId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task GetByPerformanceId_BadRequest()
        {
            int performanceId = 99;

            var response = await _httpClient.GetAsync($"api/customers/by-performance/{performanceId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
