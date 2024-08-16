using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace sertif.Models
{
	public class Student
	{
		[Key]
		public int StudentId { get; set; }
		public int NIS { get; set; }
		public string Class { get; set; }
		public string Gender { get; set; }
		public string Religion { get; set; }
		public string PlaceOfBirth { get; set; }
		public DateOnly DateOfBirth { get; set; }
		public string HomeAddress { get; set; }
		public string FatherName { get; set; }
		public string MotherName { get; set; }


		[ForeignKey("User")]
		public int UserId { get; set; }
		public virtual User User { get; set; }
	}
}
