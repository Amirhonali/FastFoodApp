using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Application.Models;
using Domain.Entities;

namespace Application.DTO.UserDTO
{
	public class UserRegisterDTO 
	{
        public User User { get; set; }

		public Token UserTokens { get; set; }
	}
}

