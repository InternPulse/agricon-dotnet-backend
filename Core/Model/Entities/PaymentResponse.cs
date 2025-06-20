namespace Agricon.Core.Model.Entities
{
    public class PaymentResponse
    {
        public int Id { get; set; }
        public string CustomerEmail { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string AuthorizationUrl { get; set; }
        public string Message { get; set; }
        public string Reference { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
    }

    public class PaymentDto
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
        public string Reference { get; set; }
        public string PaymentMethod { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
