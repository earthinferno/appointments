using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FieldAppointments.Models;
using FieldAppointments.Services;
using Microsoft.AspNetCore.Mvc;


namespace FieldAppointmentsAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController : Controller
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        // GET: api/<controller>
        [HttpGet("{month}/{year}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> Get(int month, int year)
        {
            try
            {
                return await _appointmentService.GetAppointments(new DateTime(year, month, 1));
            }
            catch (Exception ex)
            {
                return NotFound(ex);
            }
            
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Appointment>> Get(int id)
        {
            return await _appointmentService.GetAppointment(id);
        }

        // POST api/<controller>
        [HttpPost]
        public async Task<ActionResult<int>> Post([FromBody]Appointment appointment)
        {
            return await _appointmentService.AddAppointment(appointment);
        }

        // PUT api/<controller>
        [HttpPut]
        public async Task<ActionResult<Appointment>> Put([FromBody]Appointment appointment)
        {
            return await _appointmentService.UpdateAppointment(appointment);
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await _appointmentService.DeleteAppointment(id);
            return NoContent();
        }
    }
}
