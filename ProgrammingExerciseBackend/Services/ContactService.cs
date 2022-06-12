using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ProgrammingExercise.DTOs;
using ProgrammingExercise.Models;
using ProgrammingExerciseBackend.Data;
using ProgrammingExerciseBackend.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ProgrammingExerciseBackend.Services
{
    public class ContactService : IContactService
    {
        private readonly DatabaseContext _context;
        public ContactService(DatabaseContext context)
        {
            _context = context;
            _context.Database.EnsureCreated();
        }

        /// <summary>
        /// Method to create a new contact
        /// </summary>
        /// <param name="contact">New Contact</param>
        /// <param name="file">Personal Photo File</param>
        /// <param name="url">Path to personal photo</param>
        /// <returns>Returns the new contact created</returns>
        public async Task<ContactDTO> Create(Contact contact, IFormFile file, string url)
        {
            string uniqueFileName = "";
            if (file != null)
                uniqueFileName = UploadedFile(file, url);

            contact.PersonalPhoto = uniqueFileName;
            _context.Contact.Add(contact);
            await _context.SaveChangesAsync();

            return new ContactDTO
            {
                Id = contact.Id,
                Addresses = contact.Addresses,
                DateOfBirth = contact.DateOfBirth,
                FirstName = contact.FirstName,
                PhoneNumbers = contact.PhoneNumbers,
                SecondName = contact.SecondName
            };

        }

        /// <summary>
        /// Method to create a new contact
        /// </summary>
        /// <param name="id">Receive the id of the contact to be deleted</param>
        /// <returns>Return null if the contact does not exist and return a blank string if the contact is deleted</returns>
        public async Task<string> Delete(int id)
        {
            var contact = await _context.Contact.FindAsync(id);
            if (contact == null)
            {
                return null;
            }

            _context.Contact.Remove(contact);
            await _context.SaveChangesAsync();

            return String.Empty;
        }

        /// <summary>
        /// Method to filter existing contacts based on an age range
        /// </summary>
        /// <param name="from">from age</param>
        /// <param name="to">to age</param>
        /// <returns>Returns the contact list that matches the search criteria</returns>
        public async Task<IEnumerable<ContactDTO>> FilterByAge(int from, int to)
        {
            return await _context.Contact
                .Where(x => DateTime.Now.AddYears(-from) > x.DateOfBirth && DateTime.Now.AddYears(-to) < x.DateOfBirth)
                .Select(x => new ContactDTO
                {
                    Addresses = x.Addresses,
                    DateOfBirth = x.DateOfBirth,
                    FirstName = x.FirstName,
                    Id = x.Id,
                    PhoneNumbers = x.PhoneNumbers,
                    SecondName = x.SecondName
                })
                .ToListAsync();
        }

        /// <summary>
        /// Method to filter existing contacts based on a name
        /// </summary>
        /// <param name="name">Contact first name, second name and address </param>
        /// <returns>Returns the contact list that matches the search criteria</returns>
        public async Task<IEnumerable<ContactDTO>> FilterByName(string name)
        {
            return await _context.Contact
                .Where(x => x.FirstName.ToUpper().Contains(name.ToUpper()) || x.SecondName.ToUpper().Contains(name.ToUpper()) || x.Addresses.ToUpper().Contains(name.ToUpper()))
                .Select(x => new ContactDTO
                {
                    Addresses = x.Addresses,
                    DateOfBirth = x.DateOfBirth,
                    FirstName = x.FirstName,
                    Id = x.Id,
                    PhoneNumbers = x.PhoneNumbers,
                    SecondName = x.SecondName
                })
                .ToListAsync();
        }

        /// <summary>
        /// Method to get all stored contacts
        /// </summary>
        /// <returns>Returns all contacts</returns>
        public async Task<IEnumerable<ContactDTO>> GetAll()
        {
            return await _context.Contact.Select(x => new ContactDTO
            {
                Addresses = x.Addresses,
                DateOfBirth = x.DateOfBirth,
                FirstName = x.FirstName,
                Id = x.Id,
                PhoneNumbers = x.PhoneNumbers,
                SecondName = x.SecondName
            }).ToListAsync();
        }

        /// <summary>
        /// Method to get by id stored contacts
        /// </summary>
        /// <param name="id">Contact Id</param>
        /// <returns>Method to obtain with contact according to the id</returns>
        public async Task<ContactDetailDTO> GetById(int id)
        {
            return await _context.Contact.Select(x => new ContactDetailDTO
            {
                Addresses = x.Addresses,
                DateOfBirth = x.DateOfBirth,
                FirstName = x.FirstName,
                Id = x.Id,
                PhoneNumbers = x.PhoneNumbers,
                SecondName = x.SecondName,
                PersonalPhoto = x.PersonalPhoto
            }).FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// Method to update an existing contact
        /// </summary>
        /// <param name="contact">Updated contact</param>
        /// <param name="id">Ccntact Id</param>
        /// <param name="file">Personal Photo File</param>
        /// <param name="url">Path to personal photo</param>
        /// <returns></returns>
        public async Task<ContactDTO> Update(Contact contact, int id, IFormFile file, string url)
        {
            try
            {
                var contact_updt = await _context.Contact.FirstOrDefaultAsync(x => x.Id == id);
                contact_updt.FirstName = contact.FirstName;
                contact_updt.SecondName = contact.SecondName;
                contact_updt.PhoneNumbers = contact.PhoneNumbers;
                contact_updt.Addresses = contact.Addresses;
                contact_updt.DateOfBirth = contact.DateOfBirth;
                string uniqueFileName = "";
                if (file != null)
                    uniqueFileName = UploadedFile(file, url);

                contact_updt.PersonalPhoto = uniqueFileName;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(id))
                {
                    return null;
                }
                else
                {
                    throw;
                }
            }

            return new ContactDTO
            {
                Id = contact.Id,
                Addresses = contact.Addresses,
                DateOfBirth = contact.DateOfBirth,
                FirstName = contact.FirstName,
                PhoneNumbers = contact.PhoneNumbers,
                SecondName = contact.SecondName
            };
        }

       private bool ContactExists(int id)
        {
            return _context.Contact.Any(e => e.Id == id);
        }
        private string UploadedFile(IFormFile file, string url)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                var folderName = Path.Combine("Resources", "Images");
                var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fileName = uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                var fullPath = Path.Combine(pathToSave, fileName);
                var dbPath = Path.Combine(folderName, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                return $"{url}/loadimage/{uniqueFileName}";
            }
            return uniqueFileName;
        }
        /// <summary>
        /// Method to expose the photo of a given contact
        /// </summary>
        /// <param name="name">Personal photo name</param>
        /// <returns></returns>
        public async Task<byte[]> LoadImage(string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                var folderName = Path.Combine("Resources", "Images");
                var pathToLoad = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                string fullPath = Path.Combine(pathToLoad, name);
                if (File.Exists(fullPath))
                {
                    return await System.IO.File.ReadAllBytesAsync(fullPath);
                }
            }

            return null;
        }
    }
}
