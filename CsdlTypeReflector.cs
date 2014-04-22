using System;
using System.Collections.Generic;

namespace voidsoft.efbog
{
	internal class CsdlTypeReflector
	{
		public List<EntityDefinition> CreateCodeGenStructure(Schema s)
		{
			Utils u = new Utils();

			List<EntityDefinition> list = new List<EntityDefinition>();


			foreach (SchemaEntityType sp in s.EntityType)
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
					ep.PropertyType = u.GetDbType(Type.GetType("System." + typeProperty.Type));

					dt.Fields.Add(ep);
				}


				list.Add(dt);
			}

			return list;
		}


		


	}
}