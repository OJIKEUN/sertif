using sertif.Models;
using System;

namespace sertif.ViewModels
{
    public class ManageAccountViewModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
        public string FullName { get; set; }
        public UserRole Role { get; set; }

        // Fields untuk Student
        public int? StudentId { get; set; }
        public int? NIS { get; set; }
        public string? Class { get; set; }
        public string? Gender { get; set; }
        public string? Religion { get; set; }
        public string? PlaceOfBirth { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? HomeAddress { get; set; }
        public string? FatherName { get; set; }
        public string? MotherName { get; set; }

        // Fields untuk Laboran
        public int? LaboranId { get; set; }
        public int? NIP { get; set; }
    }
}
