using System;
using System.Collections.Generic;
using System.Data;

namespace voidsoft.efbog
{
	internal class CsdlTypeReflector
	{

		private const string BINARY = "Binary";

		public List<EntityDefinition> CreateCodeGenStructure(Schema s)
		{
			Utils u = new Utils();

			List<EntityDefinition> list = new List<EntityDefinition>();

			foreach (SchemaEntityType sp in s.EntityType)
			{
				try
				{
					EntityDefinition dt = new EntityDefinition();

					if (sp.Key.Length > 0)
					{
						dt.PrimaryKeyFieldName = sp.Key[0].Name;
					}

					dt.Name = sp.Name;

					foreach (SchemaEntityTypeProperty typeProperty in sp.Property)
					{
						EntityDefinitionProperty ep = new EntityDefinitionProperty();
						ep.IsNullable = Convert.ToBoolean(typeProperty.Nullable);
						ep.IsPrimaryKey = dt.PrimaryKeyFieldName == typeProperty.Name;
						ep.PropertyName = typeProperty.Name;


						if (typeProperty.Type != BINARY)
						{
							ep.PropertyType = u.GetDbType(Type.GetType("System." + typeProperty.Type));
						}
						else
						{
							ep.PropertyType = DbType.Binary;
						}

						dt.Fields.Add(ep);
					}

					list.Add(dt);
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}

			return list;
		}
	}
}