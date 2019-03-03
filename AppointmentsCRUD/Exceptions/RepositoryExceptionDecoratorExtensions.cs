using System.ComponentModel;
using FieldAppointments.Exceptions;

namespace FieldAppointments.Extensions
{
    public static class RepositoryExceptionDecoratorExtensions
    {
        public static string ToDescriptionString(this RepositiryExceptionType val, string suppliedValue)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0
                ? attributes[0].Description + " Value: " + suppliedValue
                : "Undefined exception type.  Value: " + suppliedValue;
        }

    }
}
