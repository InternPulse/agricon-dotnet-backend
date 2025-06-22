using Agricon.Core.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agricon.Core.Model.Entities
{
    public class Transaction 
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Booking")]
        public int BookingId { get; set; }

        public TransactionDescription Description { get; set; }

        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public double Amount { get; set; }

        [Required]
        public TransactionStatus Status { get; set; } 

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [MaxLength(100)]
        public string Reference { get; set; }

    }
}
