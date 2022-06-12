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

        public async Task<IEnumerable<ContactDTO>> FilterByName(string name)
        {
            return await _context.Contact
                .Where(x => x.FirstName.Contains(name) || x.SecondName.Contains(name) || x.Addresses.Contains(name))
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

        private int GetAge(DateTime birthdate)
        {
            DateTime now = DateTime.Today;
            DateTime birthday = birthdate;

            int age = now.Year - birthday.Year;

            if (now.Month < birthday.Month || (now.Month == birthday.Month && now.Day < birthday.Day))//not had bday this year yet
                age--;

            return age;
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
