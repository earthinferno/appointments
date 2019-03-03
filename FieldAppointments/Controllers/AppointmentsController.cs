using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FieldAppointments.Models;
using Microsoft.AspNetCore.Mvc;

namespace FieldAppointments.Controllers
{
    public class AppointmentsController : Controller
    {

        //return all appointments page by month
        public IActionResult Index()
        {
            return View();
        }

        //return a single appointment
        public IActionResult GetAppointment(int appointmentId)
        {
            return View("Appointment");
        }

        //add an appointment --> create calender entry
        public IActionResult AddAppointment(int appointmentId)
        {
            return View("Appointment");
        }



        public IActionResult SaveAppointment(Appointment appointment)
        {
            return View("Index");
        }
        
        //update a calender appointment
        public IActionResult UpdateAppointment(Appointment appointment)
        {
            return View("Index");
        }

        //delete a calander appointment
        public IActionResult DeleteAppointment(int id)
        {
            return View("Index");
        }
    }
}