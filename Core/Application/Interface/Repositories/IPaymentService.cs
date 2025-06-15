using Agricon.Core.Dtos;
using Agricon.Core.Model.Entities;

namespace Agricon.Core.Application.Interface.Repositories
{
    public interface IPaymentService
    {
        Task<PaymentResponse> InitiatePaymentAsync(PaymentRequestDto request);
        Task<PaymentResponse> VerifyPaymentAsync(string reference);
    }

}
