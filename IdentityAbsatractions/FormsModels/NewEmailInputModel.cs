using System.ComponentModel.DataAnnotations;

namespace IdentityAbstractions.FormsModels
{
    public class NewEmailInputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Новое письмо")]
        public string? NewEmail { get; set; }
    }
}
