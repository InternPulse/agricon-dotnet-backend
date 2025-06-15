using Agricon.Core.Application.Interface.Repositories;
using Agricon.Core.Dtos;
using Agricon.Core.Model.Entities;
using Agricon.Core.Model.Enums;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Agricon.Core.Application.Services
{
    public class PaystackService : IPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ITransactionRepository _repo;

        public PaystackService(HttpClient httpClient, IConfiguration config, ITransactionRepository repo)
        {
            _httpClient = httpClient;
            _config = config;
            _repo = repo;
        }

        public async Task<PaymentResponse> InitiatePaymentAsync(PaymentRequestDto request)
        {
            var payload = new
            {
                email = request.CustomerEmail,
                amount = (int)(request.Amount * 100)
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config["Paystack:SecretKey"]);

            var res = await _httpClient.PostAsync("https://api.paystack.co/transaction/initialize", content);
            var resBody = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                return new PaymentResponse { Status = "failed", Message = "Payment initiation failed." };
            }

            dynamic json = JsonConvert.DeserializeObject<dynamic>(resBody);
            string reference = json.data.reference;
            string authUrl = json.data.authorization_url;

            await _repo.CreateAsync(new Transaction
            {
                Id = Guid.NewGuid().ToString(),
                BookingId = request.BookingId,
                Reason = request.Reason,
                Amount = request.Amount,
                PaymentMethod = "Paystack",
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Reference = reference
            });

            return new PaymentResponse
            {
                Id = reference,
                CustomerEmail = request.CustomerEmail,
                CustomerName = request.CustomerName,
                Amount = request.Amount,
                Status = "pending",
                AuthorizationUrl = authUrl,
                Message = "Redirect to complete payment"
            };
        }

        public async Task<PaymentResponse> VerifyPaymentAsync(string reference)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _config["Paystack:SecretKey"]);
            var res = await _httpClient.GetAsync($"https://api.paystack.co/transaction/verify/{reference}");
            var resBody = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                return new PaymentResponse { Id = reference, Status = "failed", Message = "Verification failed." };
            }

            dynamic json = JsonConvert.DeserializeObject<dynamic>(resBody);
            string status = json.data.status;

            var transaction = await _repo.GetByReferenceAsync(reference);
            if (transaction != null)
            {
                transaction.Status = status == "SUCCESS" ? PaymentStatus.Success : PaymentStatus.Failed;
                transaction.UpdatedAt = DateTime.UtcNow;
                await _repo.SaveChangesAsync();
            }

            return new PaymentResponse
            {
                Id = reference,
                CustomerEmail = json.data.customer?.email ?? "unknown",
                CustomerName = json.data.customer?.name ?? "unknown",
                Amount = ((decimal)json.data.amount) / 100,
                Status = status,
                Message = "Payment verified"
            };
        }
    }

}
