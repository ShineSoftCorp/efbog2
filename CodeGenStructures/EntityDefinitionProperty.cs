using System.Data;

namespace voidsoft.efbog
{
	public class EntityDefinitionProperty
	{
		public bool IsNullable;
		public bool IsPrimaryKey;
		public string PropertyName;
		public DbType PropertyType;
	}
}