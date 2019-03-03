using FieldAppointments.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FieldAppointments.Data
{
    public class AppointmentsDBContext : DbContext, IAppointmentsDBContext
    {
        public AppointmentsDBContext(DbContextOptions<AppointmentsDBContext> options) : base(options)
        {

        }

        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
