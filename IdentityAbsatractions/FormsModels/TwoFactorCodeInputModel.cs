using System.ComponentModel.DataAnnotations;

namespace IdentityAbstractions.FormsModels
{
    public class TwoFactorCodeInputModel
    {
        [Required]
        [StringLength(7, ErrorMessage = "Длина {0} должна быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Код аутентификатора")]
        public string? TwoFactorCode { get; set; }

        [Display(Name = "Запомни эту машину")]
        public bool RememberMachine { get; set; }
    }
}
