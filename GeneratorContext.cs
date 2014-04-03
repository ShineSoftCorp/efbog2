using System.Collections.Generic;

namespace voidsoft.efbog
{
	internal static class GeneratorContext
	{
		public static string ContextName;
		public static string EntitiesNamespaceName;
		public static string UserSpecifiedNamespace;
		public static string Path;
		public static List<ColumnAnnotation> Annotations = new List<ColumnAnnotation>();

		public static Dictionary<string, string> dictionaryLookupDisplayNames = new Dictionary<string, string>();

		public static List<EntityData> Entities = new List<EntityData>();
	}
}