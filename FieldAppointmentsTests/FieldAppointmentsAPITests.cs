using FieldAppointments.Models;
using FieldAppointmentsAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using Web.Api.IntegrationTests;
using Xunit;


namespace FieldAppointmentsTests
{
    public class FieldAppointmentsAPITests : IClassFixture<CustomWebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public FieldAppointmentsAPITests(CustomWebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
        }


        [Fact]
        public async Task GetAppointments_OK()
        {
            //SETUP
            string location = "Integration Test Location", summary = "Integration Test Sumamry";
            DateTime startDate = DateTime.Now, endDate = startDate.AddHours(3);
            Appointment appointment = new Appointment
            {
                StartDate = startDate,
                EndDate = endDate,
                Location = location,
                Summary = summary
            };

            // add some appointments
            await _client.PostAsJsonAsync<Appointment>("/api/appointments", appointment);
            await _client.PostAsJsonAsync<Appointment>("/api/appointments", appointment);
            await _client.PostAsJsonAsync<Appointment>("/api/appointments", appointment);


            //ACT
            // The endpoint or route of the controller action.
            var httpResponse = await _client.GetAsync($"/api/appointments/{startDate.Month}/{startDate.Year}");

            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = httpResponse.Content.ReadAsStringAsync().Result;
            var appointments = JsonConvert.DeserializeObject<List<Appointment>>(stringResponse);

            //ASSERT
            // Deserialize and examine results.
            Assert.Contains(appointments, p => p.Location == location);
            Assert.Contains(appointments, p => p.Summary == summary);
            Assert.Contains(appointments, p => p.StartDate == startDate);
            Assert.Contains(appointments, p => p.EndDate == endDate);

            //TEARDOWN
            foreach (var app in appointments)
            {
                await _client.DeleteAsync($"/api/appointments/{app.ID}");
            }


        }

        [Fact]
        public async Task AddAppointment_OK()
        {
            // SETUP
            string location = "Integration Test Location", summary = "Integration Test Sumamry";
            DateTime startDate = DateTime.Now, endDate = startDate.AddHours(27);

            // ACT
            // Add an appointment
            var PosthttpResponse = await _client.PostAsJsonAsync<Appointment>("/api/appointments",
                new Appointment
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Location = location,
                    Summary= summary
                });

            // Must be successful.
            PosthttpResponse.EnsureSuccessStatusCode();

            //ASSERT
            //Get the created appointment so that we can examine it
            var ID = PosthttpResponse.Content.ReadAsStringAsync().Result;
            var GetHttpResponse = await _client.GetAsync($"/api/appointments/{ID}");
            GetHttpResponse.EnsureSuccessStatusCode();
            var stringResponse = GetHttpResponse.Content.ReadAsStringAsync().Result;
            var appointment = JsonConvert.DeserializeObject<Appointment>(stringResponse);

            // Deserialize and examine results.
            Assert.Equal(startDate, appointment.StartDate);
            Assert.Equal(endDate, appointment.EndDate);
            Assert.Equal(location, appointment.Location);
            Assert.Equal(summary, appointment.Summary);

            //TEARDOWN
            await _client.DeleteAsync($"/api/appointments/{appointment.ID}");

        }

        [Fact]
        public async Task GetPaginatedAppointments_OK()
        {
            //SETUP
            //SETUP
            string location = "Integration Test Location", summary = "Integration Test Sumamry";
            DateTime startDate = DateTime.Now, endDate = startDate.AddHours(3);
            Appointment janAppointment = new Appointment
            {
                StartDate = new DateTime(2018,01,01),
                EndDate = new DateTime(2018, 01, 02),
                Location = location,
                Summary = summary
            };
            Appointment febAppointment = new Appointment
            {
                StartDate = new DateTime(2018, 02, 01),
                EndDate = new DateTime(2018, 02, 02),
                Location = location,
                Summary = summary
            };
            Appointment marAppointment = new Appointment
            {
                StartDate = new DateTime(2018, 03, 01),
                EndDate = new DateTime(2018, 03, 02),
                Location = location,
                Summary = summary
            };


            // add some appointments
            await _client.PostAsJsonAsync<Appointment>("/api/appointments", janAppointment);
            await _client.PostAsJsonAsync<Appointment>("/api/appointments", janAppointment);
            await _client.PostAsJsonAsync<Appointment>("/api/appointments", febAppointment);
            await _client.PostAsJsonAsync<Appointment>("/api/appointments", marAppointment);
            await _client.PostAsJsonAsync<Appointment>("/api/appointments", marAppointment);
            await _client.PostAsJsonAsync<Appointment>("/api/appointments", marAppointment);

            //ACT
            // Get all feb appointments
            var httpResponse = await _client.GetAsync("/api/appointments/2/2018");
            // Must be successful.
            httpResponse.EnsureSuccessStatusCode();
            var stringResponse = httpResponse.Content.ReadAsStringAsync().Result;
            var appointments = JsonConvert.DeserializeObject<List<Appointment>>(stringResponse);

            //ASSERT
            // Deserialize and examine results.
            Assert.Single(appointments);
            Assert.Equal(new DateTime(2018, 02, 01), appointments[0].StartDate);
            Assert.Equal(new DateTime(2018, 02, 02), appointments[0].EndDate);

            //TEARDOWN
            foreach (var app in appointments)
            {
                await _client.DeleteAsync($"/api/appointments/{app.ID}");
            }

        }

        [Fact]
        public async Task UpdateAppointment_OK()
        {
            // SETUP
            string location = "Integration Test Location", summary = "Integration Test Sumamry";
            DateTime startDate = new DateTime(2019,2,1), endDate = startDate.AddHours(27);

            // Add an appointment
            var PosthttpResponse = await _client.PostAsJsonAsync<Appointment>("/api/appointments",
                new Appointment
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Location = location,
                    Summary = summary
                });

            // Must be successful.
            PosthttpResponse.EnsureSuccessStatusCode();


            //ASSERT
            var response = await _client.PutAsJsonAsync("/api/appointments",
                new Appointment
                {
                    StartDate = startDate.AddDays(2),
                    EndDate = endDate.AddDays(2),
                    Location = location,
                    Summary = summary
                });


            response.EnsureSuccessStatusCode();
            var stringResponse = response.Content.ReadAsStringAsync().Result;
            var appointment = JsonConvert.DeserializeObject<Appointment>(stringResponse);

            // Deserialize and examine results.
            Assert.Equal(startDate.AddDays(2), appointment.StartDate);
            Assert.Equal(endDate.AddDays(2), appointment.EndDate);
            Assert.Equal(location, appointment.Location);
            Assert.Equal(summary, appointment.Summary);

            //TEARDOWN
            await _client.DeleteAsync($"/api/appointments/{appointment.ID}");

        }


    }
}
