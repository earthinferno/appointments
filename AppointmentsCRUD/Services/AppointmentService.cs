using FieldAppointments.Data;
using FieldAppointments.Exceptions;
using FieldAppointments.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FieldAppointments.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppointmentsDBContext _dbContext;

        public AppointmentService(AppointmentsDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        //return all appointments page by month
        public async Task<List<Appointment>> GetAppointments(DateTime month)
        {
            return await _dbContext.Appointments.Where(a => a.StartDate.Month == month.Month).ToListAsync();
        }

        //return a single appointment
        public async Task<Appointment> GetAppointment(int id)
        {
            return await _dbContext.Appointments.FindAsync(id);
        }

        //add an appointment --> create calender entry
        public async Task<int> AddAppointment(Appointment appointment)
        {
            if (appointment.EndDate <= appointment.StartDate)
            {
                throw new RepositoryException(RepositiryExceptionType.BadAppointmentEndDateTime, null);
            }
            var newAppointment = _dbContext.Appointments.Add(appointment).Entity.ID;
            await _dbContext.SaveChangesAsync();
            return newAppointment;
        }

        //update a calender appointment
        public async Task<Appointment> UpdateAppointment(Appointment appointment)
        {
            var updatedAppointment =  _dbContext.Appointments.Update(appointment).Entity;
            await _dbContext.SaveChangesAsync();
            return updatedAppointment;
        }

        //delete a calander appointment
        public async Task DeleteAppointment(int id)
        {
            var appointment = _dbContext.Appointments.Find(id);
            if (appointment == null)
            {
                throw new RepositoryException(RepositiryExceptionType.AppointmentNotFound, id);
            }
            _dbContext.Appointments.Remove(appointment);
            await _dbContext.SaveChangesAsync();
        }
    }
}
