using System.ComponentModel.DataAnnotations;

namespace SomeCoreApp.API.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "Please specify a password between 4 and 8 characters.")]
        public string Password { get; set; }
               
    }
}