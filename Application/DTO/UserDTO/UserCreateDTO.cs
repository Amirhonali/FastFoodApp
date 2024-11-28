using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Application.DTO.UserDTO
{
	public class UserCreateDTO
	{
        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }

        [JsonIgnore]
        public int[] RoleId { get; set; } = {1};
    }
}

