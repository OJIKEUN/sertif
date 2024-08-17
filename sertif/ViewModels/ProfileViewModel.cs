using sertif.Models;
using System.ComponentModel.DataAnnotations;

public class ProfileViewModel
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string FullName { get; set; }
    public UserRole Role { get; set; }

    // Fields untuk Student
    public int? StudentId { get; set; }  // Tambahkan ini
    public int? NIS { get; set; }
    [Required(ErrorMessage = "Please select a class")]
    public string? Class { get; set; }
    [Required(ErrorMessage = "Please select a gender")]
    public string? Gender { get; set; }
    [Required(ErrorMessage = "Please select a religion")]
    public string? Religion { get; set; }
    public string? PlaceOfBirth { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public string? HomeAddress { get; set; }
    public string? FatherName { get; set; }
    public string? MotherName { get; set; }

    // Fields untuk Laboran
    public int? LaboranId { get; set; }  // Tambahkan ini
    public int? NIP { get; set; }
}
