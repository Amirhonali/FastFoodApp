using System;
using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Application.DTO.PermissionDTO
{
	public class PermissionUpdateDTO
	{
        public string? PermissionName { get; set; }
    }
}

