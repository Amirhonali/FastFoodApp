using System;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Application.DTO.PermissionDTO
{
	public class PermissionCreateDTO
	{
        public required string PermissionName { get; set; }
    }
}

