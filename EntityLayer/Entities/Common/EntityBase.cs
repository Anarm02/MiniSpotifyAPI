using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityLayer.Entities.Common
{
	public class EntityBase:IEntityBase
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
		public bool IsDeleted { get; set; } = false;
		public DateTime? UpdatedDate { get; set; }
		public DateTime? DeletedDate { get; set; }
	}
}
