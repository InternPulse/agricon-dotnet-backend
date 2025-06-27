using Agricon.Core.Application.Interface.Repositories;
using Agricon.Core.Application.Interface.Services;
using Agricon.Core.Dtos;
using Agricon.Core.Model;
using Agricon.Core.Model.Entities;
using Agricon.Core.Model.Enums;
using Agricon.Infrastructure.Repository;
using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
                amount = (int)(booking.Amount * 100),
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
            string authUrl = json.data.authorization_url;
            string reference = json.data.reference;

            var paymentDescEnum = request.TransactionDescription.ToLower() switch
            {
                "booking" => TransactionDescription.Booking,
                "penalty" => TransactionDescription.Penalty,
                "extension" => TransactionDescription.Extension,
                "other" => TransactionDescription.Other,
                _ => throw new ArgumentException("Invalid transaction description.")
            };
            TransactionStatus statusEnum;

            if (!Enum.TryParse<TransactionStatus>(TransactionStatus.Pending.ToString(), ignoreCase: true, out statusEnum))
            {
                return BaseResponse<PaymentResponse>.FailResponse("Invalid transaction status.");
            }


            var transaction = new Transaction
            {
                BookingId = request.BookingId,
                Amount = booking.Amount,
                Status = TransactionStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                PaymentMethod = PaymentMethod.pending,
                Description = paymentDescEnum,
                Reference = reference
            };

            var trans = await _repo.CreateAsync(transaction);

            var result = new PaymentResponse
            {
                Id = trans.Id,
                Reference = reference,
                BookingId= request.BookingId,
                CustomerEmail = request.CustomerEmail,
                Amount = (decimal)booking.Amount,
                Status = "pending",
                AuthorizationUrl = authUrl,
                Message = "Redirect to complete payment",
                Description = request.TransactionDescription.ToString(),
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
                transaction.Status = status == "success" ? TransactionStatus.Completed : TransactionStatus.Failed;
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
                BookingId = transaction.BookingId,
                CustomerEmail = json.data.customer?.email ?? "unknown",
                Amount = ((decimal)json.data.amount) / 100,
                Status = status,
                Message = "Payment verified",
                PaymentMethod = paymentMethodEnum.ToString(),
                Description = transaction.Description.ToString(),
                Reference = transaction.Reference,
            };

            return BaseResponse<PaymentResponse>.SuccessResponse(result, "Payment verified successfully");
        }


        public async Task<PaginatedResult<PaymentDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var transactions = await _repo.GetAllAsync(pageNumber, pageSize);

            if (transactions.TotalCount == 0)
            {
                return new PaginatedResult<PaymentDto>
                {
                    Items = new List<PaymentDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            return transactions;
        }

        public async Task<PaginatedResult<PaymentDto>> GetByUserAsync(int bookingId, int pageNumber, int pageSize)
        {
            var transactions = await _repo.GetByUserAsync(bookingId, pageNumber, pageSize);

            if (transactions.TotalCount == 0)
            {
                return new PaginatedResult<PaymentDto>
                {
                    Items = new List<PaymentDto>(),
                    TotalCount = 0,
                    PageNumber = pageNumber,
                    PageSize = pageSize
                };
            }

            return transactions;
        }

        public async Task<byte[]> GenerateTransactionReceiptByBookingIdAsync(int bookingId)
        {
            var transaction = await _repo.GetByBookingIdAsync(bookingId);
            if (transaction == null)
                throw new Exception("Transaction not found");

            using var ms = new MemoryStream();
            using var writer = new PdfWriter(ms);
            using var pdf = new PdfDocument(writer);
            var document = new iText.Layout.Document(pdf);

            // Red Warning Section
            var redBg = new iText.Layout.Element.Paragraph("This is the receipt of your transaction.")
                .SetBackgroundColor(new DeviceRgb(0, 100, 0))
                .SetFontColor(iText.Kernel.Colors.ColorConstants.WHITE)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .SetPadding(5);
            document.Add(redBg);

            document.Add(new Paragraph("\n"));
            var blueBg = new Div()
               .SetBackgroundColor(new DeviceRgb(0, 100, 0))
                .SetPadding(20)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER);

            blueBg.Add(new Paragraph($"You made the payment of ").SetFontColor(iText.Kernel.Colors.ColorConstants.WHITE));
            blueBg.Add(new Paragraph($"NGN {transaction.Amount:N2}").SetFontSize(24).SetFontColor(iText.Kernel.Colors.ColorConstants.WHITE));

            document.Add(blueBg);
            document.Add(new Paragraph("\n"));

            // Transaction Details Heading
            document.Add(new Paragraph("Transaction Details").SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFontSize(14));
            document.Add(new Paragraph("\n"));
            AddDetailRow(document, "Reference", transaction.Reference);
           // AddDetailRow(document, "Booking ID", transaction.BookingId.ToString());
            AddDetailRow(document, "Description", transaction.Description.ToString());
            AddDetailRow(document, "Payment Method", transaction.PaymentMethod.ToString());
            AddDetailRow(document, "Amount Paid", $"NGN {transaction.Amount:N2}");
            AddDetailRow(document, "Payment Status", transaction.Status.ToString());
            AddDetailRow(document, "Date", transaction.CreatedAt.ToString("dd MMM, yyyy HH:mm:ss"));
            document.Close();
            return ms.ToArray();
        }
        private void AddDetailRow(Document document, string label, string value)
        {
            var p = new Paragraph()
                .Add(new Text($"{label}: "))
                .Add(new Text(value));
            document.Add(p);
        }
        public async Task<Transaction> GetByBookingIdAsync(int bookingid)
        {
            return await _repo.GetByBookingIdAsync(bookingid);
        }
    }
}
