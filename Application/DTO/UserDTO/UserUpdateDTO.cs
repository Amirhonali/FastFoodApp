using System;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Application.DTO.UserDTO
{
	public class UserUpdateDTO
	{
        public int Id { get; set; }

        public string FullName { get; set; }

        [EmailAddress]
        public string Email { get; set; }


        public int[] RoleId { get; set; }
    }
}

