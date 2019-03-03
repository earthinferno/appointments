using FieldAppointments.Data;
using FieldAppointments.Exceptions;
using FieldAppointments.Models;
using FieldAppointments.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace FeildAppointmentsTests
{
    public class AppointmentServiceTests
    {
        #region setup
        DbContextOptionsBuilder<AppointmentsDBContext> _optionsBuilder;
        //IAppointmentService _appointmentService;

       
        public AppointmentServiceTests()
        {
            _optionsBuilder = new DbContextOptionsBuilder<AppointmentsDBContext>();
        }


        private AppointmentService Setup(AppointmentsDBContext dbContext)
        {
            return (
                new AppointmentService(
                    dbContext
                ));
        }
        #endregion

        #region test helper methods

            private Appointment InsertTestAppointment(DateTime startTime, DateTime endTime, string location, string summary)
            {
                Appointment appointment;
                using (var dbContext = new AppointmentsDBContext(_optionsBuilder.Options))
                {
                    appointment = dbContext.Appointments.Add(new Appointment()
                        {
                            StartDate = startTime,
                            EndDate = endTime,
                            Location = location,
                            Summary = summary
                        }).Entity;
                    dbContext.SaveChangesAsync();
                }
                return appointment;
            }

            private int AddTestAppointment(DateTime startTime, DateTime endTime, string location = "Test Location", string summary = "Test Summary")
            {
                Appointment appointment = InsertTestAppointment(startTime, endTime, location, summary);
                return appointment.ID;
            }

        #endregion


        //return all appointments page by month
        // public List<Appointment> GetAppointments(DateTime Month)
        [Fact]
        public void GetAllAppointments_OK()
        {
            // ARRANGE
            var startTime = DateTime.Now;
            var endTime = startTime.AddSeconds(1);
            string summary = "Test summary..", location = "Test location....";
            TestActionExecutor<List<Appointment>> action = new TestActionExecutor<List<Appointment>>();

            _optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            var result = AddTestAppointment(startTime: startTime, endTime: endTime, location: location, summary: summary);

            // ACT
            // I'd like to make this a one liner but I need a refactor. I'd need to inject dbContext
            List<Appointment> response;
            using (var dbContext = new AppointmentsDBContext(_optionsBuilder.Options))
            {
                var appointmentService = Setup(dbContext);
                response = action.ExecuteAction( async () =>  await appointmentService.GetAppointments(DateTime.Now)).Result;
            }


            // ASSERT
            Assert.NotEmpty(response);
            Assert.NotEqual(0, response.First().ID);
            Assert.Equal(startTime, response.First().StartDate);
            Assert.Equal(endTime, response.First().EndDate);
            Assert.Equal(location, response.First().Location);
            Assert.Equal(summary, response.First().Summary);

        }



        //return a single appointment
        // public Appointment GetAppointment(int id)
        [Fact]
        public async Task GetAppointment_OK()
        {
            // ARRANGE
            DateTime startTime = DateTime.Now, 
                endTime = startTime.AddSeconds(1);
            string summary = "Test summary..", 
                location = "Test location....";

            _optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());


            var appointmentId = AddTestAppointment(startTime: startTime, endTime: endTime, location: location, summary: summary);


            // ACT
            Appointment response;
            using (var dbContext = new AppointmentsDBContext(_optionsBuilder.Options))
            {
                var appointmentService = Setup(dbContext);
                response = await appointmentService.GetAppointment(appointmentId);
            }

            // ASSERT
            Assert.NotNull(response);
            Assert.NotEqual(0, response.ID);
            Assert.Equal(startTime, response.StartDate);
            Assert.Equal(endTime, response.EndDate);
            Assert.Equal(summary, response.Summary);
            Assert.Equal(location, response.Location);
        }



        //add an appointment --> create calender entry
        // public int AddAppointment(Appointment appointment)
        [Fact]
        public async Task  AddAppointment_OK()
        {
            // ARRANGE
            DateTime startTime = DateTime.Now,
                endTime = startTime.AddSeconds(1);
            string summary = "Test summary..",
                location = "Test location....";

           _optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());


            // ACT
            int appointmentId;
            using (var dbContext = new AppointmentsDBContext(_optionsBuilder.Options))
            {
                var appointmentService = Setup(dbContext);
                appointmentId = await appointmentService.AddAppointment(new Appointment()
                {
                    StartDate = startTime,
                    EndDate = endTime,
                    Location = location,
                    Summary = summary
                });
            }

            // ASSERT
            Assert.NotEqual(-99, appointmentId);
            using (var dbContext = new AppointmentsDBContext(_optionsBuilder.Options))
            {
                if (dbContext.Appointments.Find(appointmentId) is Appointment appointment)
                {
                    Assert.Equal(startTime, appointment.StartDate);
                    Assert.Equal(endTime, appointment.EndDate);
                    Assert.Equal(summary, appointment.Summary);
                    Assert.Equal(location, appointment.Location);
                }
            }
        }


        //update a calender appointment
        //public Appointment UpdateAppointment(Appointment appointment)
        [Fact]
        public async Task UpdateAppointment_OK()
        {
            // ARRANGE
            DateTime startTime = DateTime.Now,
                endTime = startTime.AddSeconds(1);
            string summary = "Test summary..",
                location = "Test location....";

            _optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            var oldAppointment = InsertTestAppointment(startTime: startTime, endTime: endTime, location: location, summary: summary);

            // ACT
            Appointment updatedAppointment;
            using (var dbContext = new AppointmentsDBContext(_optionsBuilder.Options))
            {
                var appointmentService = Setup(dbContext);
                updatedAppointment = await appointmentService.UpdateAppointment(new Appointment()
                {
                    ID = oldAppointment.ID,
                    EndDate = endTime, // don't use oldAppointment.endDate to ensure no insert performed.
                    StartDate = startTime, // don't use oldAppointment.startDate to ensure no insert performed.
                    Location = "LocationUpdated",
                    Summary = "SumamryUpdated"
                });
            }

            // ASSERT
            Assert.Equal(oldAppointment.ID, updatedAppointment.ID);
            Assert.Equal(oldAppointment.StartDate, updatedAppointment.StartDate);
            Assert.Equal(oldAppointment.EndDate, updatedAppointment.EndDate);
            Assert.NotEqual(oldAppointment.Location, updatedAppointment.Location);
            Assert.NotEqual(oldAppointment.Summary, updatedAppointment.Summary);

        }

        //delete a calander appointment
        //public void DeleteAppointment(int id)
        [Fact]
        public async Task DeleteAppointment_OK()
        {
            // ARRANGE
            DateTime startTime = DateTime.Now,
                endTime = startTime.AddSeconds(1);
            string summary = "Test summary..",
                location = "Test location....";

            _optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            var oldAppointment = InsertTestAppointment(startTime: startTime, endTime: endTime, location: location, summary: summary);
            //Assert that the appointment record was created
            Assert.NotNull(oldAppointment);

            // ACT
            using (var dbContext = new AppointmentsDBContext(_optionsBuilder.Options))
            {
                var appointmentService = Setup(dbContext);
                await appointmentService.DeleteAppointment(oldAppointment.ID);
            }

            // ASSERT
            using (var dbContext = new AppointmentsDBContext(_optionsBuilder.Options))
            {
                if (dbContext.Appointments.Find(oldAppointment.ID) is Appointment appointment)
                {
                    Assert.Null(appointment);
                }
            }
        }

        //delete a calander appointment
        //public void DeleteAppointment(int id)
        [Fact]
        public async Task DeleteAppointment_NotOK()
        {
            // ARRANGE
            DateTime startTime = DateTime.Now,
                endTime = startTime.AddSeconds(1);
            string summary = "Test summary..",
                location = "Test location....";

            _optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            var oldAppointment = InsertTestAppointment(startTime: startTime, endTime: endTime, location: location, summary: summary);
            //Assert that the appointment record was created
            Assert.NotNull(oldAppointment);

            // ACT
            Exception ex;
            using (var dbContext = new AppointmentsDBContext(_optionsBuilder.Options))
            {
                var appointmentService = Setup(dbContext);
                //ex = Assert.Throws<RepositoryException>(async () => await appointmentService.DeleteAppointment(oldAppointment.ID + -99));
                ex = await Assert.ThrowsAsync<RepositoryException>(() => appointmentService.DeleteAppointment(oldAppointment.ID + -99));

            }

            // ASSERT
            Assert.Equal("Appointment not found.", ex.Message.Substring(0,22));
            if (ex is RepositoryException repoEx)
            {
                Assert.Equal(RepositiryExceptionType.AppointmentNotFound, repoEx.ExceptionType);
            }
        }

        [Fact]
        public async Task AddBadAppointment_NotOK()
        {
            // ARRANGE
            DateTime startTime = DateTime.Now,
                endTime = startTime.AddHours(-1);
            string summary = "Test summary..",
                location = "Test location....";

            _optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());

            Appointment appointment = new Appointment
            {
                StartDate = startTime,
                EndDate = endTime,
                Location = location,
                Summary = summary
            };

            // ACT
            Exception ex;
            using (var dbContext = new AppointmentsDBContext(_optionsBuilder.Options))
            {
                var appointmentService = Setup(dbContext);
                ex = await Assert.ThrowsAsync<RepositoryException>(() => appointmentService.AddAppointment(appointment));

            }

            // ASSERT
            Assert.Contains("Appointment must end after it has started.", ex.Message);
            if (ex is RepositoryException repoEx)
            {
                Assert.Equal(RepositiryExceptionType.BadAppointmentEndDateTime, repoEx.ExceptionType);
            }
        }
    }

    // I'd like to tidy up the action -> invoke servcie method but.....
    // Gads this isn't going to work cuz I can't get at the appointmentservice dependancy using DI. Id have to 
    // pass the dbContext object as a method parameter to AppointmentService rather than injecting into its constructor. 
    // I could do this by injecting dbContext into the appointments controller instead -- hmm maybe refactor?
    //public class TestActionExecutor<T>
    //{
    //    internal T ExecuteAction(Func<T> action, DbContextOptionsBuilder<AppointmentsDBContext> optionsBuilder)
    //    {
    //        T response;
    //        using (var dbContext = new AppointmentsDBContext(optionsBuilder.Options))
    //        {
    //            var appointmentService = new AppointmentService(dbContext);
    //            response = action.Invoke();
    //        }

    //        return response;
    //    }
    //}

    public class TestActionExecutor<T>
    {
        internal async Task<T> ExecuteAction(Func<Task<T>> action)
        {
            T response;
            response = await action.Invoke();
            return response;
        }

    }
}

