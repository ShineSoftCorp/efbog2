using System;
using System.Collections.Generic;

namespace voidsoft.efbog
{
	public class EntityData
	{
		public Type Entity;
		public string PrimaryKeyFieldName;
		public List<EntityRelationship> Relationships = null;
	}
}