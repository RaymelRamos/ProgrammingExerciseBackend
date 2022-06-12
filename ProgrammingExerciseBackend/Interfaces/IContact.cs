using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProgrammingExercise.DTOs;
using ProgrammingExercise.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProgrammingExerciseBackend.Interfaces
{
    public interface IContactService
    {
        public Task<IEnumerable<ContactDTO>> GetAll();
        public Task<ContactDetailDTO> GetById(int id);
        public Task<ContactDTO> Create(Contact contact, IFormFile image, string url);
        public Task<ContactDTO> Update(Contact contact, int id, IFormFile file, string url);
        public Task<string> Delete(int id);
        public Task<IEnumerable<ContactDTO>> FilterByName( string name);
        public Task<IEnumerable<ContactDTO>> FilterByAge(int from, int to);
        public Task<byte[]> LoadImage(string name);
    }
}
