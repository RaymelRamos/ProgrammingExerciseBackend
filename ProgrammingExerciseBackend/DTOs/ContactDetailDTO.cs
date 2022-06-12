using System;

namespace ProgrammingExercise.DTOs
{
    public class ContactDetailDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string SecondName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Addresses{ get; set; }
        public string PhoneNumbers { get; set; }
        public string PersonalPhoto { get; set; }
    }
}
