using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace voidsoft.efbog
{
	public class EntityFrameworkTypeReflector
	{
		private EdmRelationshipAttribute[] relationshipAttributes;

		private GeneratorContext context;

		public EntityFrameworkTypeReflector(GeneratorContext c)
		{
			context = c;
		}

		
		public void ReflectAndGenerate(string assemblyPath)
		{
			Assembly assembly = Assembly.LoadFrom(assemblyPath);

			relationshipAttributes = (EdmRelationshipAttribute[]) assembly.GetCustomAttributes(typeof (EdmRelationshipAttribute), false);

			Type[] types = assembly.GetTypes();

			//find the DbContext
			if (GetDbContext(types) == false)
			{
				return;
			}

			//GetEntities(types, assembly);

			//get the resources

			Schema sc = ReadEntityMetadataFromResourceFile(assembly);

			context.DbContextSchema = sc;


			(new CsdlTypeReflector()).CreateCodeGenStructure(context.DbContextSchema);


			//load additional annotations
			List<ColumnAnnotation> annotations = (new ColumnAnnotation(context)).ParseAnnotations(Environment.CurrentDirectory + @"\annotations.txt");

			if (annotations != null)
			{
				context.Annotations = annotations;
			}





			StartGenerationProcess();
		}






		private Schema ReadEntityMetadataFromResourceFile(Assembly asm)
		{
			Stream stream = null;

			try
			{
				string[] resourceNames = asm.GetManifestResourceNames();

				string name = resourceNames.FirstOrDefault(s => s.ToLower().EndsWith(".csdl"));

				stream = asm.GetManifestResourceStream(name);

				XmlSerializer xs = new XmlSerializer(typeof(Schema));

				Schema schema = xs.Deserialize(stream) as Schema;

				return schema;
			}
			finally
			{
				if (stream != null)
				{
					stream.Close();
				}
			}
		}

		public EntityDefinitionProperty[] GetProperties(Type tp)
		{
			PropertyInfo[] properties = tp.GetProperties();

			List<EntityDefinitionProperty> listProperties = new List<EntityDefinitionProperty>();

			foreach (PropertyInfo info in properties)
			{
				EdmScalarPropertyAttribute[] attributes = (EdmScalarPropertyAttribute[]) info.GetCustomAttributes(typeof (EdmScalarPropertyAttribute), false);

				if (attributes.Length > 0)
				{
					listProperties.Add(new EntityDefinitionProperty {IsNullable = attributes[0].IsNullable, PropertyName = info.Name, PropertyType = (new Utils()).GetDbType(info.PropertyType), IsPrimaryKey = attributes[0].EntityKeyProperty});
				}
			}

			return listProperties.ToArray();
		}

		public static string GetPrimaryKeyName(Type tp)
		{
			PropertyInfo[] properties = tp.GetProperties();

			foreach (PropertyInfo info in properties)
			{
				EdmScalarPropertyAttribute[] attributes = (EdmScalarPropertyAttribute[]) info.GetCustomAttributes(typeof (EdmScalarPropertyAttribute), false);

				if (attributes[0].EntityKeyProperty)
				{
					return info.Name;
				}
			}

			throw new ArgumentException("The primary key field can't be found for entity " + tp.Name);
		}

		private void StartGenerationProcess()
		{
			(new BusinessObjectGenerator(this.context)).Generate();

			//generate WebViews
			//(new WebClassGenerator(this.context)).Generate();

			//WebGridViewWithEntityDataSourceGenerator.Generate();
		}

		//private void GetEntities(IEnumerable<Type> types, Assembly assembly)
		//{
		//	//try to find the types by going over the DbContext porperties
		//	Type type = assembly.GetType(context.EntitiesNamespaceName + "." + context.ContextName, true, true);

		//	PropertyInfo[] properties = type.GetProperties();

		//	foreach (PropertyInfo property in properties)
		//	{
		//		Type t = property.PropertyType;

		//		if (t.IsGenericType && t.GenericTypeArguments.Length > 0 )
		//		{
		//			string entityName = t.GenericTypeArguments[0].Name;

		//		}
		//	}



		//	List<EntityDefinition> listInitialPass = new List<EntityDefinition>();

		//	foreach (Type t in types)
		//	{
		//		if (t.IsSubclassOf(typeof (EntityObject)))
		//		{
		//			try
		//			{
		//				EntityDefinition entityDefinition = new EntityDefinition();
		//				entityDefinition.Entity = t;
		//				entityDefinition.PrimaryKeyFieldName = GetPrimaryKeyName(t);
		//				// entityData.Relationships = GetRelatedEntities(t.Name);
		//				listInitialPass.Add(entityDefinition);
		//			}
		//			catch (Exception ex)
		//			{
		//				Console.WriteLine(ex.Message);
		//			}
		//		}
		//	}

		//	List<EntityDefinition> listFinal = new List<EntityDefinition>();

		//	for (int i = 0; i < listInitialPass.Count; i++)
		//	{
		//		try
		//		{
		//			listFinal.Add(listInitialPass[i]);
		//			listFinal[listFinal.Count - 1].Relationships = GetRelatedEntities(listInitialPass[i].Name, listInitialPass);
		//		}
		//		catch
		//		{
		//			continue;
		//		}
		//	}

		//	context.Entities = listFinal;
		//}



		private bool GetDbContext(Type[] types)
		{
			foreach (Type type in types)
			{
				if (!type.IsClass)
				{
					continue;
				}

				if (!type.IsSubclassOf(typeof (DbContext)))
				{
					continue;
				}

				context.ContextName = type.Name;
				context.EntitiesNamespaceName = type.Namespace;
				break;
			}

			if (string.IsNullOrEmpty(context.ContextName))
			{
				Console.WriteLine("Can't find the DbContext");
				return false;
			}

			return true;
		}

		
		//private  List<EntityRelationship> GetRelatedEntities(string entityName, List<EntityDefinition> entities)
		//{
		//	List<EntityRelationship> relations = new List<EntityRelationship>();

		//	foreach (EdmRelationshipAttribute a in relationshipAttributes)
		//	{
		//		if (a.Role1Name == entityName)
		//		{
		//			EntityDefinition related = entities.Find(e => e.Entity.Name == a.Role2Name);
		//			relations.Add(new EntityRelationship(RelationshipType.Parent, a.Role2Name, related.PrimaryKeyFieldName));
		//		}
		//		else if (a.Role2Name == entityName)
		//		{
		//			EntityDefinition related = entities.Find(e => e.Entity.Name == a.Role1Name);
		//			relations.Add(new EntityRelationship(RelationshipType.Child, a.Role1Name, related.PrimaryKeyFieldName));
		//		}
		//	}

		//	return relations;
		//}
	}
}