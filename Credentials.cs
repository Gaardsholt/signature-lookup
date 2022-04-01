using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace signature_lookup
{
    public interface IValidatable
    {
        void Validate();
    }

    public class Credentials : IValidatable
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }


        public void Validate()
        {
            Validator.ValidateObject(this, new ValidationContext(this), validateAllProperties: true);
        }
    }
}
