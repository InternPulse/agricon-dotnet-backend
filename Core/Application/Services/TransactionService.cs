using Agricon.Core.Application.Interface.Repositories;
using Agricon.Core.Application.Interface.Services;
using Agricon.Core.Dtos;
using Agricon.Core.Model;
using Agricon.Core.Model.Entities;
using Agricon.Core.Model.Enums;
using Agricon.Infrastructure.Repository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Agricon.Core.Application.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly HttpClient _httpClient;
        private readonly PaystackSettings _settings;
        private readonly ITransactionRepository _repo;
        private readonly IBookingRepository _bookingRepository;

        public TransactionService(HttpClient httpClient, IOptions<PaystackSettings> options, ITransactionRepository repo, IBookingRepository bookingRepository)
        {
            _httpClient = httpClient;
            _settings = options.Value;
            _repo = repo;
            _bookingRepository = bookingRepository;
        }

        public async Task<BaseResponse<PaymentResponse>> InitiatePaymentAsync(PaymentRequestDto request)
        {
            var booking = await _bookingRepository.GetByIdAsync(request.BookingId);
            if(booking == null)
            {
                return BaseResponse<PaymentResponse>.FailResponse("Booking not found.");
            }
            if (booking.Paid == true)
            {
                return BaseResponse<PaymentResponse>.FailResponse("Payment has been made for this booking");
            }
            var payload = new
            {
                email = request.CustomerEmail,
                amount = (int)(booking.Amount * 100)
            };

            var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.SecretKey);

            var response = await _httpClient.PostAsync($"{_settings.BaseUrl}/transaction/initialize", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BaseResponse<PaymentResponse>.FailResponse("Payment initiation failed.");
            }

            dynamic json = JsonConvert.DeserializeObject<dynamic>(responseBody);
            string reference = json.data.reference;
            string authUrl = json.data.authorization_url;

            var transaction = new Transaction
            {
                BookingId = request.BookingId,
                Amount = booking.Amount,
                Status = PaymentStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                PaymentMethod = PaymentMethod.pending,
                Reason = request.Reason,
                Reference = reference
            };

            var trans = await _repo.CreateAsync(transaction);

            var result = new PaymentResponse
            {
                Id = trans.Id,
                ReferenceId = reference,
                CustomerEmail = request.CustomerEmail,
                CustomerName = request.CustomerName,
                Amount = (decimal)booking.Amount,
                Status = "pending",
                AuthorizationUrl = authUrl,
                Message = "Redirect to complete payment",
                PaymentMethod = transaction.PaymentMethod.ToString(),
            };

            return BaseResponse<PaymentResponse>.SuccessResponse(result, "Payment initialized successfully");
        }

        public async Task<BaseResponse<PaymentResponse>> VerifyPaymentAsync(string reference)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _settings.SecretKey);

            var response = await _httpClient.GetAsync($"{_settings.BaseUrl}/transaction/verify/{reference}");
            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                return BaseResponse<PaymentResponse>.FailResponse("Payment verification failed.");
            }

            dynamic json = JsonConvert.DeserializeObject<dynamic>(responseBody);
            string status = json.data.status;
            string method = json.data.channel?.ToString()?.ToLower();

            var paymentMethodEnum = method switch
            {
                "card" => PaymentMethod.Card,
                "bank" => PaymentMethod.BankTransfer,
                "ussd" => PaymentMethod.USSD,
                _ => PaymentMethod.pending
            };

            var transaction = await _repo.GetByReferenceAsync(reference);
            if (transaction != null)
            {
                transaction.Status = status == "success" ? PaymentStatus.Success : PaymentStatus.Failed;
                transaction.PaymentMethod = paymentMethodEnum;
                transaction.UpdatedAt = DateTime.UtcNow;

                if (status == "success")
                {
                    await _bookingRepository.UpdatePaidAsync(transaction.BookingId, true);
                }
                await _repo.SaveChangesAsync();
            }

            var result = new PaymentResponse
            {
                Id = transaction.Id,
                CustomerEmail = json.data.customer?.email ?? "unknown",
                CustomerName = json.data.customer?.name ?? "unknown",
                Amount = ((decimal)json.data.amount) / 100,
                Status = status,
                Message = "Payment verified",
                PaymentMethod = paymentMethodEnum.ToString(),
                ReferenceId = transaction.Reference,
            };

            return BaseResponse<PaymentResponse>.SuccessResponse(result, "Payment verified successfully");
        }


        public async Task<PaginatedResult<Transaction>> GetAllAsync(int pageNumber, int pageSize)
        {
            var transactions = await _repo.GetAllAsync(pageNumber, pageSize);

            if (transactions.TotalCount == 0)
            {
                return new PaginatedResult<Transaction>
                {
                    Items = new List<Transaction>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            return transactions;
        }

        public async Task<PaginatedResult<Transaction>> GetByUserAsync(int bookingId, int pageNumber, int pageSize)
        {
            var transactions = await _repo.GetByUserAsync(bookingId, pageNumber, pageSize);

            if (transactions.TotalCount == 0)
            {
                return new PaginatedResult<Transaction>
                {
                    Items = new List<Transaction>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            return transactions;
        }

    }
}
