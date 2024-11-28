using System;
namespace Application.DTO.RoleDTO
{
	public class RoleUpdateDTO
	{
        public int RoleId { get; set; }

        public string Name { get; set; }

        public int[] PermissionsId { get; set; }
    }
}

