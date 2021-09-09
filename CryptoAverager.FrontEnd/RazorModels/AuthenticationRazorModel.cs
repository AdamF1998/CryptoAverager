using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace CryptoAverager.FrontEnd.RazorModels
{
    public class AuthenticationRazorModel
    {
        [Display(Name = "Api key")]
        public string BinanceApiKey { get; set; }

        [Display(Name = "Api key")]
        public string HuobiApiKey { get; set; }

        public string Exchange { get; set; }

        public string BinanceSecretKey { get; set; }

        public string HuobiSecretKey { get; set; }
    }

    public class AuthenticationRazorModelValidator : AbstractValidator<AuthenticationRazorModel>
    {
        public AuthenticationRazorModelValidator()
        {
            RuleFor(x => x.BinanceApiKey).NotEmpty().When(x => string.IsNullOrEmpty(x.HuobiApiKey)).WithMessage("Please provide a Binance Api key");
            RuleFor(x => x.BinanceSecretKey).NotEmpty().When(x => string.IsNullOrEmpty(x.HuobiSecretKey)).WithMessage("Please provide a Binance secret key");

            RuleFor(x => x.HuobiApiKey).NotEmpty().When(x => string.IsNullOrEmpty(x.BinanceApiKey)).WithMessage("Please provide a Huobi Api key");
            RuleFor(x => x.HuobiSecretKey).NotEmpty().When(x => string.IsNullOrEmpty(x.BinanceSecretKey)).WithMessage("Please provide a Huobi secret key");
        }
    }
}
