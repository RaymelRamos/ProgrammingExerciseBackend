using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProgrammingExercise.DTOs;
using ProgrammingExercise.Models;
using ProgrammingExerciseBackend.Data;
using ProgrammingExerciseBackend.Interfaces;
using ProgrammingExerciseBackend.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ProgrammingExerciseBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly IContactService _contactService;
        public ContactsController(IContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ContactDTO>>> Get()
        {
            try
            {
                var contact = await _contactService.GetAll();

                return contact != null ? Ok(contact) : NoContent();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Contact>> Get(int id)
        {
            try 
            {
                var contact = await _contactService.GetById(id);

                return contact != null ? Ok(contact) : NotFound();
            }
            catch (Exception e) 
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<ActionResult<ContactDTO>> Post([FromForm] Contact body)
        {
            var url = HttpContext.Request.GetDisplayUrl();
            try
            {
                var contact = await _contactService.Create(body, Request.Form.Files.Count() > 0 ? Request.Form.Files[0] : null, url);

                return Ok(contact);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ContactDTO>> Put(int id, [FromForm] Contact body)
        {
            var url = $"{this.Request.Scheme}://{this.Request.Host}/api/contacts";

            try
            {
                if (id != body.Id)
                {
                    return BadRequest();

                }
                var contact = await _contactService.Update(body, id, Request.Form.Files.Count() > 0 ? Request.Form.Files[0] : null, url);
                return contact != null ? contact : NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var contact = await _contactService.Delete(id);

                if (contact == string.Empty)
                    return NoContent();
                else if (contact == null)
                    return NotFound();
                else
                    return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("findByName/{name}")]
        public async Task<ActionResult<Contact>> Get(string name)
        {
            try
            {
                var contact = await _contactService.FilterByName(name);

                return contact != null ? Ok(contact) : NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpGet("findByAge/{from}/{to}")]
        public async Task<ActionResult<Contact>> Get(int from, int to)
        {
            try
            {
                var contact = await _contactService.FilterByAge(from, to);

                return contact != null ? Ok(contact) : NotFound();
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
       
        [HttpGet("loadImage/{name}")]
        public async Task<ActionResult> LoadImage(string name)
        {
            try
            {
                var bytes_image = await _contactService.LoadImage(name);
                return File(bytes_image, "image/*"); 
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}
