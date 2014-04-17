using System.Collections.Generic;

namespace voidsoft.efbog
{
	public class GeneratorContext
	{
		public string ContextName;
		public string EntitiesNamespaceName;
		public string UserSpecifiedNamespace;
		public string Path;

		public List<ColumnAnnotation> Annotations = new List<ColumnAnnotation>();

		public Dictionary<string, string> DictionaryLookupDisplayNames = new Dictionary<string, string>();

		public List<EntityDefinition> Entities = new List<EntityDefinition>();

		public Schema DbContextSchema
		{
			get;
			set;
		}

	}
}