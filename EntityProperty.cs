using System.Data;

namespace voidsoft.efbog
{
	public class EntityProperty
	{
		public bool IsNullable;
		public bool IsPrimaryKey;
		public string PropertyName;
		public DbType PropertyType;
	}
}