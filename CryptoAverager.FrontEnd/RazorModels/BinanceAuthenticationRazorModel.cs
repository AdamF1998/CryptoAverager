using System.ComponentModel.DataAnnotations;

namespace CryptoAverager.FrontEnd.RazorModels
{
    public class BinanceAuthenticationRazorModel
    {
        [Required]
        [Display(Name = "Api key")]
        public string ApiKey { get; set; }

        [Required]
        public string SecretKey { get; set; }
    }
}
