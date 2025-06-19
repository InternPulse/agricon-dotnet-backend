using System.ComponentModel.DataAnnotations.Schema;

namespace Agricon.Core.Model.Entities
{
    [Table("Booking")]
    public class Booking
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("facilityId")]
        public int FacilityId { get; set; }

        [Column("farmerId")]
        public int FarmerId { get; set; }

        [Column("amount")]
        public double Amount { get; set; }

        [Column("paid")]
        public bool Paid { get; set; }

        [Column("active")]
        public bool Active { get; set; }

        [Column("startDate")]
        public DateTime StartDate { get; set; }

        [Column("endDate")]
        public DateTime EndDate { get; set; }

        [Column("createdAt")]
        public DateTime CreatedAt { get; set; }

        [Column("updatedAt")]
        public DateTime UpdatedAt { get; set; }
    }

}
