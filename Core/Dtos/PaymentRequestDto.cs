using Agricon.Core.Model.Enums;

namespace Agricon.Core.Dtos
{
    public class PaymentRequestDto
    {
        public int BookingId { get; set; }
        public Reason Reason { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }

}
