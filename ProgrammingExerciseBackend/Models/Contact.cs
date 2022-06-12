using System;


namespace ProgrammingExercise.Models
{
    public class Contact
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
