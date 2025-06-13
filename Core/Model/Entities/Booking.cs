namespace Agricon.Core.Model.Entities
{
    public class Booking
    {
        public string Id { get; set; }
        public string FacilityId { get; set; }
        public string FarmerId { get; set; }
        public decimal Amount { get; set; }
        public bool Paid { get; set; }
        public bool Active { get; set; }
        public DateTime? Reserved { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
