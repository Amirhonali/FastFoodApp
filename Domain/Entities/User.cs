using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "Full Name cannot exceed 100 characters.")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be between 8 and 100 characters.")]
        public string Password { get; set; }

        public virtual ICollection<Role> Roles { get; set; } = new List<Role>();

        [StringLength(4, ErrorMessage = "Confirmation Code must be 4 characters.")]
        public string? ConfirmationCode { get; set; }

        public bool IsConfirmed { get; set; }

        public DateTime? ResetTokenExpiry { get; set; }

        public string? PasswordResetToken { get; set; }

        public ICollection<Cart> Carts { get; set; }

        public byte[]? ImageData { get; set; }
    }
}

