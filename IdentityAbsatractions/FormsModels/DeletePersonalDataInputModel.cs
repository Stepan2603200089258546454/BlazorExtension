using System.ComponentModel.DataAnnotations;

namespace IdentityAbstractions.FormsModels
{
    public class DeletePersonalDataInputModel
    {
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";
    }
}
