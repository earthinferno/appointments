using System;
using FieldAppointments.Extensions;

namespace FieldAppointments.Exceptions
{
    public class RepositoryException : Exception
    {
        public RepositoryException(RepositiryExceptionType type)
        {
            ExceptionType = type;
        }

        public RepositoryException(RepositiryExceptionType type, string value) : base(type.ToDescriptionString(value))
        {
            ExceptionType = type;
        }

        public RepositoryException(RepositiryExceptionType type, int value) : base(type.ToDescriptionString(value.ToString()))
        {
            ExceptionType = type;
        }

        public RepositiryExceptionType ExceptionType { get; }
    }


}
