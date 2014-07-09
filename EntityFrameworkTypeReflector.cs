using System;
using System.Collections.Generic;
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
		private GeneratorContext context;
		private EdmRelationshipAttribute[] relationshipAttributes;

		public EntityFrameworkTypeReflector(GeneratorContext c)
		{
			context = c;
		}

		public void ReflectAndGenerate(string assemblyPath)
		{
			Assembly assembly = Assembly.LoadFrom(assemblyPath);

			Type[] types = assembly.GetTypes();

			//find the DbContext
			if (GetDbContext(types) == false)
			{
				return;
			}

			try
			{
				Schema sc = ReadEntityMetadataFromResourceFile(assembly);
				context.DbContextSchema = sc;
			}
			catch
			{
			}

			if (context.DbContextSchema != null)
			{
				List<EntityDefinition> entities = (new CsdlTypeReflector()).CreateCodeGenStructure(context.DbContextSchema);

				context.Entities = entities;
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

				XmlSerializer xs = new XmlSerializer(typeof (Schema));

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

		private EntityDefinitionProperty[] GetProperties(Type tp)
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

		private void StartGenerationProcess()
		{
			(new BusinessObjectGenerator(context)).Generate();
		}

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
	}
}