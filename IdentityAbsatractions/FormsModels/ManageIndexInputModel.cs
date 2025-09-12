using System.ComponentModel.DataAnnotations;

namespace IdentityAbstractions.FormsModels
{
    public class ManageIndexInputModel
    {
        [Phone]
        [Display(Name = "Номер телефона")]
        public string? PhoneNumber { get; set; }
    }
}
