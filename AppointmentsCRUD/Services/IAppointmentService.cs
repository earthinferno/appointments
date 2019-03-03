using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FieldAppointments.Models;

namespace FieldAppointments.Services
{
    public interface IAppointmentService
    {
        Task<int> AddAppointment(Appointment appointment);
        Task DeleteAppointment(int id);
        Task<Appointment> GetAppointment(int id);
        Task<List<Appointment>> GetAppointments(DateTime Month);
        Task<Appointment> UpdateAppointment(Appointment appointment);
    }
}