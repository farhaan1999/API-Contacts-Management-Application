using System.Text.Json;
using ContactManagement.Models;

namespace ContactManagement.Services
{
    public class ContactService
    {
        private readonly string _filePath = "Data/contacts.json";

        public List<ContactModel> GetAllContacts()
        {
            try
            {
                if (!File.Exists(_filePath)) return new List<ContactModel>();
                var json = File.ReadAllText(_filePath);
                return JsonSerializer.Deserialize<List<ContactModel>>(json) ?? new List<ContactModel>();
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading contacts from the file.", ex);
            }
        }

        public void SaveContacts(List<ContactModel> contacts)
        {
            try
            {
                var json = JsonSerializer.Serialize(contacts);
                File.WriteAllText(_filePath, json);
            }
            catch (Exception ex)
            {
                throw new Exception("Error saving contacts to the file.", ex);
            }
        }

        public ContactModel GetContactById(int id)
        {
            return GetAllContacts().FirstOrDefault(c => c.Id == id);
        }

        public ContactModel AddContact(ContactModel contact)
        {   
            var contacts = GetAllContacts();
            contact.Id = contacts.Any() ? contacts.Max(c => c.Id) + 1 : 1;
            contacts.Add(contact);
            SaveContacts(contacts);
            return contact;
        }
    }
}
