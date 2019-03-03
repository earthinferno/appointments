using FieldAppointments.Models;
using Microsoft.EntityFrameworkCore;

namespace FieldAppointments.Data
{
    public interface IAppointmentsDBContext
    {
        DbSet<Appointment> Appointments { get; set; }
    }
}