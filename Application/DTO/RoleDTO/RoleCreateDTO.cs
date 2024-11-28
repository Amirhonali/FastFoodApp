using System;
namespace Application.DTO.RoleDTO
{
	public class RoleCreateDTO
	{
        public string Name { get; set; }

        public int[] PermissionsId { get; set; }
    }
}

