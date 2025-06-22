using Agricon.Core.Application.Services;
using Agricon.Core.Model.Enums;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Agricon.Core.Dtos
{
    public class PaymentRequestDto
    {
        public int BookingId { get; set; }
        [DefaultValue("Penalty")]
        [RegularExpression("(?i)Booking|Extension|Penalty|Other", ErrorMessage = "Description must be one of: Booking, Extension, Penalty, Other.")]
        public string TransactionDescription { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
    }

}
