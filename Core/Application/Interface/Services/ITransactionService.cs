using Agricon.Core.Dtos;
using Agricon.Core.Model;
using Agricon.Core.Model.Entities;
using Agricon.Infrastructure.Repository;

namespace Agricon.Core.Application.Interface.Services
{
    public interface ITransactionService
    {
        Task<BaseResponse<PaymentResponse>> InitiatePaymentAsync(PaymentRequestDto request);
        Task<BaseResponse<PaymentResponse>> VerifyPaymentAsync(string reference);
        Task<PaginatedResult<Transaction>> GetAllAsync(int pageNumber, int pageSize);
        Task<PaginatedResult<Transaction>> GetByUserAsync(int userId, int pageNumber, int pageSize);
    }

}
