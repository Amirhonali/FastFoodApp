using System;
using System.ComponentModel.DataAnnotations;

namespace Application.DTO.UserDTO
{
	public class UserNewCreateDTO
	{
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
        public string Password { get; set; }
    }
}

