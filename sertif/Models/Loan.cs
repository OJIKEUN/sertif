using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace sertif.Models
{
    public class Loan
    {
        [Key]
        public int LoanId { get; set; }

        // Relasi ke User yang meminjam
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }

        // Relasi ke Equipment yang dipinjam
        [ForeignKey("Equipment")]
        public int EquipmentId { get; set; }
        public virtual Equipment Equipment { get; set; }

        public string Reason { get; set; }

        public int QuantityBorrowed { get; set; }

        public DateTime LoanDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public LoanStatus Status { get; set; }
    }

    // Enum untuk status peminjaman
    public enum LoanStatus
    {
        PendingApproval,
        Approved,
        Borrowed,
        Returned,
        Rejected
    }
}
