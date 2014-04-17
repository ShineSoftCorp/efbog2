using System.Collections.Generic;

namespace voidsoft.efbog
{
	public class EntityDefinition
	{
		public EntityDefinition()
		{
			Relationships = new List<EntityRelationship>();
			Fields = new List<EntityDefinitionProperty>();
		}

		public string Name
		{
			get;
			set;
		}

		public string PrimaryKeyFieldName
		{
			get;
			set;
		}
		public List<EntityRelationship> Relationships 
		{
			get;
			set;
		}

		public List<EntityDefinitionProperty> Fields
		{
			get;
			set;
		}

	}
}