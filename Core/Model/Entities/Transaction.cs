using Agricon.Core.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agricon.Core.Model.Entities
{
    [Table("Transaction")]
    public class Transaction 
    {
        [Key]
        [Required]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Booking")]
        [Column("bookingId")]
        public int BookingId { get; set; }

        [Column("description")]
        public TransactionDescription TransactionDescription { get; set; }

        [Column("paymentMethod")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        [Column("amount", TypeName = "decimal(18,2)")]
        public double Amount { get; set; }

        [Required]
        [Column("status")]
        public TransactionStatus TransactionStatus { get; set; } 

        [Required]
        [Column("createdAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updatedAt")]
        public DateTime? UpdatedAt { get; set; }

        [Required]
        [MaxLength(100)]
        [Column("ref")]
        public string Reference { get; set; }

    }
}
