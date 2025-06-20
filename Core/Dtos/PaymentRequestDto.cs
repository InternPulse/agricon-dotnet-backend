using Agricon.Core.Model.Enums;

namespace Agricon.Core.Dtos
{
    public class PaymentRequestDto
    {
        public int BookingId { get; set; }
        public TransactionDescription TransactionDescription { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }

}
