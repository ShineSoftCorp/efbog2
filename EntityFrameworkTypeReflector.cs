using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Core.Objects.DataClasses;
using System.Reflection;

namespace voidsoft.efbog
{
	public class EntityFrameworkTypeReflector
	{
		private static EdmRelationshipAttribute[] relationshipAttributes;


		public static void ReflectAndGenerate(string assemblyPath)
		{
			Assembly assembly = Assembly.LoadFrom(assemblyPath);

			relationshipAttributes = (EdmRelationshipAttribute[]) assembly.GetCustomAttributes(typeof (EdmRelationshipAttribute), false);

			Type[] types = assembly.GetTypes();

			//find the DbContext
			if (GetDbContext(types) == false)
			{
				return;
			}

			GetEntities(types, assembly);

			//load additional annotations
			List<ColumnAnnotation> annotations = ColumnAnnotation.ParseAnnotations(Environment.CurrentDirectory + @"\annotations.txt");

			if (annotations != null)
			{
				GeneratorContext.Annotations = annotations;
			}

			StartGenerationProcess();
		}

		public static EntityProperty[] GetProperties(Type tp)
		{
			PropertyInfo[] properties = tp.GetProperties();

			List<EntityProperty> listProperties = new List<EntityProperty>();

			foreach (PropertyInfo info in properties)
			{
				EdmScalarPropertyAttribute[] attributes = (EdmScalarPropertyAttribute[]) info.GetCustomAttributes(typeof (EdmScalarPropertyAttribute), false);

				if (attributes.Length > 0)
				{
					listProperties.Add(new EntityProperty {IsNullable = attributes[0].IsNullable, PropertyName = info.Name, PropertyType = GetDbType(info.PropertyType), IsPrimaryKey = attributes[0].EntityKeyProperty});
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

		private static void StartGenerationProcess()
		{
			BusinessObjectGenerator.Generate();

			//generate WebViews
			WebClassGenerator.Generate();

			//WebGridViewWithEntityDataSourceGenerator.Generate();
		}

		private static void GetEntities(IEnumerable<Type> types, Assembly assembly)
		{
			//try to find the types by going over the DbContext porperties
			Type type = assembly.GetType(GeneratorContext.EntitiesNamespaceName + "." + GeneratorContext.ContextName, true, true);

			PropertyInfo[] properties = type.GetProperties();

			foreach (PropertyInfo property in properties)
			{
				Type t = property.PropertyType;

				if (t.IsGenericType && t.GenericTypeArguments.Length > 0 )
				{
					string entityName = t.GenericTypeArguments[0].Name;

				}
			}



			List<EntityData> listInitialPass = new List<EntityData>();

			foreach (Type t in types)
			{
				if (t.IsSubclassOf(typeof (EntityObject)))
				{
					try
					{
						EntityData entityData = new EntityData();
						entityData.Entity = t;
						entityData.PrimaryKeyFieldName = GetPrimaryKeyName(t);
						// entityData.Relationships = GetRelatedEntities(t.Name);
						listInitialPass.Add(entityData);
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}

			List<EntityData> listFinal = new List<EntityData>();

			for (int i = 0; i < listInitialPass.Count; i++)
			{
				try
				{
					listFinal.Add(listInitialPass[i]);
					listFinal[listFinal.Count - 1].Relationships = GetRelatedEntities(listInitialPass[i].Entity.Name, listInitialPass);
				}
				catch
				{
					continue;
				}
			}

			GeneratorContext.Entities = listFinal;
		}



		private static bool GetDbContext(Type[] types)
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

				GeneratorContext.ContextName = type.Name;
				GeneratorContext.EntitiesNamespaceName = type.Namespace;
				break;
			}

			if (string.IsNullOrEmpty(GeneratorContext.ContextName))
			{
				Console.WriteLine("Can't find the DbContext");
				return false;
			}

			return true;
		}

		private static DbType GetDbType(Type t)
		{
			//check if it's nullable
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof (Nullable<>))
			{
				Type[] arguments = t.GetGenericArguments();

				//get the underlying type
				t = arguments[0];
			}

			if (t.FullName == "System.Int32")
			{
				return DbType.Int32;
			}
			else if (t.FullName == "System.Byte")
			{
				return DbType.Byte;
			}
			else if (t.FullName == "System.Int64")
			{
				return DbType.Int64;
			}
			else if (t.FullName == "System.Decimal")
			{
				return DbType.Decimal;
			}
			else if (t.FullName == "System.String")
			{
				return DbType.String;
			}
			else if (t.FullName == "System.DateTime")
			{
				return DbType.DateTime;
			}
			else if (t.FullName == "System.Boolean")
			{
				return DbType.Boolean;
			}

			return DbType.Object;
		}

		private static List<EntityRelationship> GetRelatedEntities(string entityName, List<EntityData> entities)
		{
			List<EntityRelationship> relations = new List<EntityRelationship>();

			foreach (EdmRelationshipAttribute a in relationshipAttributes)
			{
				if (a.Role1Name == entityName)
				{
					EntityData related = entities.Find(e => e.Entity.Name == a.Role2Name);
					relations.Add(new EntityRelationship(RelationshipType.Parent, a.Role2Name, related.PrimaryKeyFieldName));
				}
				else if (a.Role2Name == entityName)
				{
					EntityData related = entities.Find(e => e.Entity.Name == a.Role1Name);
					relations.Add(new EntityRelationship(RelationshipType.Child, a.Role1Name, related.PrimaryKeyFieldName));
				}
			}

			return relations;
		}
	}
}