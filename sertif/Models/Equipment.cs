using System.ComponentModel.DataAnnotations;

namespace sertif.Models
{
    public class Equipment
    {
        [Key]
        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        public string? Description { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than zero")]
        public int QuantityAvailable { get; set; }

        // Relasi dengan LoanTransaction
        public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }

}
