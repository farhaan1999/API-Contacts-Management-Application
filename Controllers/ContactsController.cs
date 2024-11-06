using ContactManagement.Models;
using ContactManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContactManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContactsController : ControllerBase
    {
        private readonly ContactService _contactService;

        public ContactsController(ContactService contactService)
        {
            _contactService = contactService;
        }

        [HttpGet]
        public ActionResult<List<ContactModel>> GetAll()
        {
            try
            {
                return Ok(_contactService.GetAllContacts());
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get/{id}")]
        public ActionResult<ContactModel> GetContactById(int id)
        {
            try
            {
                var contact = _contactService.GetContactById(id);
                if (contact == null) return NotFound();
                return Ok(contact);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

   

        [HttpPost("create")]
        public IActionResult CreateContact(ContactModel contact)
        {
            var contacts = _contactService.GetAllContacts();
            contact.Id = contacts.Any() ? contacts.Max(c => c.Id) + 1 : 1;
            contacts.Add(contact);
            _contactService.SaveContacts(contacts);
            return CreatedAtAction(nameof(GetContactById), new { id = contact.Id }, contact);
        }


        [HttpPut("update/{id}")]
        public IActionResult UpdateContact(int id, ContactModel updatedContact)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var contacts = _contactService.GetAllContacts();
            var contact = contacts.FirstOrDefault(c => c.Id == id);

            if (contact == null) return NotFound();

            if (contacts.Any(c => c.Email == updatedContact.Email && c.Id != id))
                return Conflict("Email already exists.");

            try
            {
                contact.FirstName = updatedContact.FirstName;
                contact.LastName = updatedContact.LastName;
                contact.Email = updatedContact.Email;
                _contactService.SaveContacts(contacts);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/{id}")]
        public IActionResult DeleteContact(int id)
        {
            var contacts = _contactService.GetAllContacts();
            var contact = contacts.FirstOrDefault(c => c.Id == id);
            if (contact == null) return NotFound();

            try
            {
                contacts.Remove(contact);
                _contactService.SaveContacts(contacts);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
