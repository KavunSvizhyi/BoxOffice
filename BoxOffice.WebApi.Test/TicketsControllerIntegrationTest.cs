using BoxOffice.Business.Models;
using BoxOffice.Data.Entities;
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
    public class TicketsControllerIntegrationTest : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;

        public TicketsControllerIntegrationTest(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            factory.ResetDatabase();
        }

        [Fact]
        public async Task Get_ProperList()
        {
            var response = await _httpClient.GetAsync("api/tickets");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<TicketModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedTickets();

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

            var response = await _httpClient.GetAsync($"api/tickets/{testId}");
            var actual = await response.Content.ReadFromJsonAsync<TicketModel>();
            var expected = ExpectedValuesGenerator.GetExpectedTickets().First(x => x.Id == testId);

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

            var response = await _httpClient.GetAsync($"api/tickets/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Post_ProperModelSaved()
        {
            var model = new AddTicketRequest()
            {
                SeatName = "ABS-01",
                PerformanceId = 1,
                Price = 50m
            };

            var response = await _httpClient.PostAsJsonAsync($"api/tickets", model);
            var created = await response.Content.ReadFromJsonAsync<PostTicketResponse>();
            var persisted = await _httpClient.GetFromJsonAsync<TicketModel>($"/api/tickets/{created!.Id}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created),
                () => created.SeatName.Should().Be(persisted!.SeatName),
                () => created.PerformanceId.Should().Be(persisted!.PerformanceId),
                () => created.Price.Should().Be(persisted!.Price)
            );
        }

        [Fact]
        public async Task Post_BadRequest()
        {
            var model = new AddTicketRequest()
            {
                SeatName = " ",
                PerformanceId = 1,
                Price = 50m
            };

            var response = await _httpClient.PostAsJsonAsync($"api/tickets", model);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Put_SavedProperly()
        {
            int testId = 1;
            var model = new UpdateTicketRequest()
            {
                Id = 1,
                SeatName = "ABS-01",
                PerformanceId = 1, 
                Price = 50m
            };

            var response = await _httpClient.PutAsJsonAsync($"api/tickets/{testId}", model);
            var persisted = await _httpClient.GetFromJsonAsync<TicketModel>($"/api/tickets/{testId}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => persisted!.SeatName.Should().Be(model.SeatName)
            );
        }

        [Fact]
        public async Task Put_BadRequest()
        {
            int testId = 1;
            var model = new UpdateTicketRequest()
            {
                Id = 2,
                SeatName = "Petro",
                PerformanceId = 1,
                Price = 50m
            };

            var response = await _httpClient.PutAsJsonAsync($"api/tickets/{testId}", model);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_SuccessfulDeletion()
        {
            int testId = 12;

            var response = await _httpClient.DeleteAsync($"api/tickets/{testId}");
            var actual = await _httpClient.GetFromJsonAsync<IEnumerable<TicketModel>>($"/api/tickets");
            var expected = ExpectedValuesGenerator.GetExpectedTickets().Where(x => x.Id != testId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task Delete_BadRequest()
        {
            int testId = 8;

            var response = await _httpClient.DeleteAsync($"api/tickets/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetAvailableByPerformanceId_ProperList()
        {
            int testId = 1;
            int expectedRemainingId = 2;

            var response = await _httpClient.GetAsync($"api/tickets/available/by-performance/{testId}");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<TicketModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedTickets()
                .Where(x => x.Id == expectedRemainingId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task GetAvailableByPerformanceId_BadRequest()
        {
            int testId = 99;

            var response = await _httpClient.GetAsync($"api/tickets/available/by-performance/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetBookingsByCustomerId_ProperList()
        {
            int testId = 1;

            var response = await _httpClient.GetAsync($"api/tickets/bookings/by-customer/{testId}");
            var actual = await response.Content.ReadFromJsonAsync<IEnumerable<TicketBookingModel>>();
            var expected = ExpectedValuesGenerator.GetExpectedTicketBookings().Where(x => x.CustomerId == testId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task GetBookingsByCustomerId_BadRequest()
        {
            int testId = 99;

            var response = await _httpClient.GetAsync($"api/tickets/bookings/by-customer/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetBookingByTicketId_ProperValue()
        {
            int testId = 1;

            var response = await _httpClient.GetAsync($"api/tickets/bookings/by-ticket/{testId}");
            var actual = await response.Content.ReadFromJsonAsync<TicketBookingModel>();
            var expected = ExpectedValuesGenerator.GetExpectedTicketBookings().First(x => x.TicketId == testId);

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK),
                () => actual.Should().BeEquivalentTo(expected)
            );
        }

        [Fact]
        public async Task GetBookingByTicketId_NotFound()
        {
            int testId = 2;

            var response = await _httpClient.GetAsync($"api/tickets/bookings/by-ticket/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetBookingByTicketId_BadRequest()
        {
            int testId = 99;

            var response = await _httpClient.GetAsync($"api/tickets/bookings/by-ticket/{testId}");

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostBooking_ProperModelSaved()
        {
            var model = new AddTicketBookingRequest()
            {
                CustomerId = 1,
                TicketId = 2,
            };

            var response = await _httpClient.PostAsJsonAsync($"api/tickets/bookings", model);
            var created = await response.Content.ReadFromJsonAsync<TicketBookingModel>();
            var persisted = await _httpClient.GetFromJsonAsync<TicketBookingModel>($"/api/tickets/bookings/by-ticket/{created!.TicketId}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created),
                () => created.CustomerId.Should().Be(persisted!.CustomerId),
                () => created.TicketId.Should().Be(persisted!.TicketId),
                () => created.OperationDate.Date.Should().Be(persisted!.OperationDate.Date)
            );
        }

        [Fact]
        public async Task PostBooking_BadRequest()
        {
            var model = new AddTicketBookingRequest()
            {
                CustomerId = 1,
                TicketId = 1,
            };

            var response = await _httpClient.PostAsJsonAsync($"api/tickets", model);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task SellTicket_ValueUpdated()
        {
            int testId = 4;

            var response = await _httpClient.PostAsync($"api/tickets/sell/{testId}", null);
            var persisted = await _httpClient.GetFromJsonAsync<TicketBookingModel>($"api/tickets/bookings/by-ticket/{testId}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => persisted!.IsSold.Should().BeTrue()
            );
        }

        [Fact]
        public async Task SellTicket_BadRequest()
        {
            int testId = 2;

            var response = await _httpClient.PostAsync($"api/tickets/sell/{testId}", null);

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task DeleteBooking_TicketUpdated()
        {
            int testId = 3;
            int ticketId = 4;

            var response = await _httpClient.DeleteAsync($"api/tickets/bookings/{testId}");
            var persisted = await _httpClient.GetAsync($"api/tickets/bookings/by-ticket/{ticketId}");
            var ticket = await _httpClient.GetFromJsonAsync<TicketModel>($"api/tickets/{ticketId}");

            Assert.Multiple
            (
                () => response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent),
                () => persisted.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound),
                () => ticket!.TicketBookingId.Should().BeNull()
            );
        }
    }
}