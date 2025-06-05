using System;
using System.ComponentModel.DataAnnotations;

namespace StudentCrud.Models
{
    public class Student
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, DataType(DataType.Date)]
        public DateTime Dob { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
