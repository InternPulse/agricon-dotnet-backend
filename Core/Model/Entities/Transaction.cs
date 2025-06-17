using Agricon.Core.Model.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agricon.Core.Model.Entities
{
    public class Transaction 
    {
        [Key]
        [Required]
        public string Id { get; set; }
        [Required]
        [ForeignKey("Booking")]
        public string BookingId { get; set; }

        [Required]
        public Reason Reason { get; set; }
        public PaymentMethod PaymentMethod { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [MaxLength(100)]
        public string Reference { get; set; }

    }


}
