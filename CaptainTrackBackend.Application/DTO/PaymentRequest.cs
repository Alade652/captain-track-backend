using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptainTrackBackend.Application.DTO
{
    public class PaymentRequest
    {
        //public string TxRef  = ;
        public decimal amount {  get; set; }
        //public string currency { get; set; } = "NGN";
        public string? redirectUrl {  get; set; }
        public string? paymentTitle { get; set; } 
        public string? paymentDescription { get; set; } 
        public string? brandLogoUrl = "https://assets.piedpiper.com/logo.png";
    }
}
