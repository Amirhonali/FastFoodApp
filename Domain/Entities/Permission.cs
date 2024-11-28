using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Domain.Entities
{
	[Table("permission")]
	public class Permission
	{
		[Column("permission_id")]
		[Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int PermissionId { get; set; }

		[Column("permission_name")]
		public string? PermissionName { get; set; }

		[JsonIgnore]
		public virtual ICollection<Role>? Roles { get; set; }
	}
}

