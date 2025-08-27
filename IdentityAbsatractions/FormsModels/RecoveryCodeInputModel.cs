using System.ComponentModel.DataAnnotations;

namespace IdentityAbstractions.FormsModels
{
    public class RecoveryCodeInputModel
    {
        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Код восстановления")]
        public string RecoveryCode { get; set; } = "";
    }
}
