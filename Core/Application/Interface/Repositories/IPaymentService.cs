using Agricon.Core.Dtos;
using Agricon.Core.Model;
using Agricon.Core.Model.Entities;

namespace Agricon.Core.Application.Interface.Repositories
{
    public interface IPaymentService
    {
        Task<BaseResponse<PaymentResponse>> InitiatePaymentAsync(PaymentRequestDto request);
        Task<BaseResponse<PaymentResponse>> VerifyPaymentAsync(string reference);
    }

}
