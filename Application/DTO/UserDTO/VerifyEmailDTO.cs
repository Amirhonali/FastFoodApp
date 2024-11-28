using System;
namespace Application.DTO.UserDTO
{
    public class VerifyEmailDTO
    {
        public string Email { get; set; }

        public string VerificationCode { get; set; }
    }
}

