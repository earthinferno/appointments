using System.ComponentModel;

namespace FieldAppointments.Exceptions
{
    public enum RepositiryExceptionType
    {
        [Description("Appointment not found.")]
        AppointmentNotFound,

        [Description("Appointment must end after it has started.")]
        BadAppointmentEndDateTime,
    }
}
