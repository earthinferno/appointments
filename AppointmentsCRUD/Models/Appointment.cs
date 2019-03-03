using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FieldAppointments.Models
{
    public class Appointment
    {
        public int ID { get; set; }
        [Required]
        [MaxLength(255)]
        public string Summary { get; set; }
        [Required]
        [MaxLength(255)]
        public string Location { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
    }
}
