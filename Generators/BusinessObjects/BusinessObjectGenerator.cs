using System;
using System.IO;
using System.Text;

namespace voidsoft.efbog
{
	public class BusinessObjectGenerator
	{
		private GeneratorContext context;

		public BusinessObjectGenerator(GeneratorContext context)
		{
			this.context = context;
		}

		public void Generate()
		{
			foreach (EntityDefinition t in context.Entities)
			{
				try
				{
					string code = GenerateBusinessObject(t);

					Console.WriteLine("Generated code for entity " + t.Name);

					GenerateFile(context.Path + @"\output\BusinessObjects\" + t.Name + "BusinessObject.cs", code);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
				}
			}
		}

		private static void GenerateFile(string filePath, string content)
		{
			FileStream fs = null;
			StreamWriter writer = null;

			try
			{
				if (File.Exists(filePath))
				{
					Console.WriteLine("File " + filePath + " already exists. Skipped code generation for this.");
					return;
				}

				File.WriteAllText(filePath, content);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		private static string GetEscapedQuote()
		{
			return "\"";
		}

		private string GenerateBusinessObject(EntityDefinition t)
		{
			StringBuilder builder = new StringBuilder();

			builder.Append("using System;" + Environment.NewLine);
			builder.Append("using System.Linq;" + Environment.NewLine);
			builder.Append("using " + context.EntitiesNamespaceName + ";" + Environment.NewLine);
			builder.Append("using System.Collections.Generic;" + Environment.NewLine);
			builder.Append("using System.Data.Entity.Core.Objects;  " + Environment.NewLine);
			builder.Append("using voidsoft.MicroRuntime.EntityFramework;  " + Environment.NewLine);
			builder.Append("using System.Data.Entity.Infrastructure;  " + Environment.NewLine);

			builder.Append("" + Environment.NewLine);
			builder.Append("" + Environment.NewLine);
			builder.Append("" + Environment.NewLine);

			builder.Append("namespace " + context.UserSpecifiedNamespace + Environment.NewLine);
			builder.Append("{" + Environment.NewLine);
			builder.Append("" + Environment.NewLine);
			builder.Append("public class " + t.Name + "BusinessObject" + Environment.NewLine);
			builder.Append("{" + Environment.NewLine);
			//builder.Append("            private bool disposeContextAfterRunningQuery = false;" + Environment.NewLine);
			builder.Append("            private " + context.ContextName + " context = null;   " + Environment.NewLine);
			builder.Append("            QueryRunner qr = new QueryRunner();" + Environment.NewLine);
			builder.Append("" + Environment.NewLine);

			//constructor
			builder.Append("            public " + t.Name + "BusinessObject" + "(" + context.ContextName + " c)" + Environment.NewLine);
			builder.Append("            {" + Environment.NewLine);
			builder.Append("                this.context = c; " + Environment.NewLine);
			builder.Append("            }" + Environment.NewLine);
			builder.Append("            " + Environment.NewLine);

			//second constructor
			builder.Append("            public " + t.Name + "BusinessObject" + "()" + Environment.NewLine);
			builder.Append("            {" + Environment.NewLine);
			builder.Append("                 context = GetContext();" + Environment.NewLine);
			builder.Append("            }" + Environment.NewLine);
			builder.Append("" + Environment.NewLine);

			//get context
			builder.Append("            private " + context.ContextName + " GetContext()" + Environment.NewLine);
			builder.Append("            {" + Environment.NewLine);
			builder.Append("                        if (context != null)" + Environment.NewLine);
			builder.Append("                        {" + Environment.NewLine);
			builder.Append("                            return context;" + Environment.NewLine);
			builder.Append("                        }" + Environment.NewLine);
			builder.Append("                        return Activator.CreateInstance<" + context.ContextName + ">();" + Environment.NewLine);
			builder.Append("            }" + Environment.NewLine);
			builder.Append("" + Environment.NewLine);

			//add entity
			builder.Append("            public void Create(" + t.Name + " entity)" + Environment.NewLine);
			builder.Append("            {" + Environment.NewLine);
			builder.Append("                         context." + t.Name + ".Add(entity);" + Environment.NewLine);
			builder.Append("                         context.SaveChanges();" + Environment.NewLine);
			builder.Append("            }" + Environment.NewLine);
			builder.Append("" + Environment.NewLine);

			//delete entity
			builder.Append("            public void Delete(" + t.Name + " entity)" + Environment.NewLine);
			builder.Append("            {" + Environment.NewLine);
			builder.Append("                         context." + t.Name + ".Remove(entity);" + Environment.NewLine);
			builder.Append("                         context.SaveChanges();" + Environment.NewLine);
			builder.Append("            }" + Environment.NewLine);
			builder.Append("" + Environment.NewLine);

			//delete by key
			builder.Append("            public void Delete(int key)" + Environment.NewLine);
			builder.Append("            {" + Environment.NewLine);
			builder.Append("                  " + t.Name + " entity =  Get" + t.Name + "(key);" + Environment.NewLine);
			builder.Append("                         if(entity == null)" + Environment.NewLine);
			builder.Append("                         {" + Environment.NewLine);
			builder.Append("                            throw new ArgumentException(" + GetEscapedQuote() + "Invalid key" + GetEscapedQuote() + ");" + Environment.NewLine);
			builder.Append("                         }" + Environment.NewLine);
			builder.Append("                 Delete(entity);   " + Environment.NewLine);
			builder.Append("            }" + Environment.NewLine);
			builder.Append("" + Environment.NewLine);

			//update
			builder.Append("            public void Update()" + Environment.NewLine);
			builder.Append("            {" + Environment.NewLine);
			builder.Append("                 context.SaveChanges(); " + Environment.NewLine);
			builder.Append("            }" + Environment.NewLine);

			//get all entities
			builder.Append("            public " + t.Name + "[] Get" + t.Name + "()" + Environment.NewLine);
			builder.Append("            {" + Environment.NewLine);
			builder.Append("                var query = from currentEntity in context." + t.Name + " select currentEntity;" + Environment.NewLine);
			builder.Append(t.Name + "[] entities = qr.Execute<" + context.ContextName + "," + t.Name + ">(context, (DbQuery<" + t.Name + ">)query, false);" + Environment.NewLine);
			builder.Append("                 return entities;" + Environment.NewLine);
			builder.Append("  }" + Environment.NewLine);

			if (t.Relationships.Count > 0)
			{
				//get entity including
				builder.Append("            public " + t.Name + "[] Get" + t.Name + "IncludingRelations()" + Environment.NewLine);
				builder.Append("            {" + Environment.NewLine);
				builder.Append("               var query = from entity in context." + t.Name);

				StringBuilder includeQueryBuilder = new StringBuilder();

				foreach (EntityRelationship relation in t.Relationships)
				{
					includeQueryBuilder.Append(".Include(" + GetEscapedQuote() + relation.RelatedEntityName + GetEscapedQuote() + ")");
				}

				builder.Append(includeQueryBuilder);

				builder.Append(" select entity;" + Environment.NewLine);

				builder.Append("             " + t.Name + "[] entities = qr.Execute<" + context.ContextName + "," + t.Name + ">(context,(DbQuery<" + t.Name + ">)query, false);" + Environment.NewLine);

				builder.Append("                return entities;" + Environment.NewLine);
				builder.Append("            }" + Environment.NewLine);
			}

			////get entity by key
			//builder.Append("            public " + t.Entity.Name + " Get" + t.Entity.Name + "(int key)" + Environment.NewLine);
			//builder.Append("            {" + Environment.NewLine);
			//builder.Append("                " + t.Entity.Name + " entity = Get" + t.Entity.Name + "(key, queryContext);" + Environment.NewLine);
			//builder.Append("                if(entity != null)" + Environment.NewLine);
			//builder.Append("                {" + Environment.NewLine);
			//builder.Append("                    return entity;" + Environment.NewLine);
			//builder.Append("                }" + Environment.NewLine);
			//builder.Append("                throw new ArgumentException(" + GetEscapedQuote() + "Invalid key" + GetEscapedQuote() + "); " + Environment.NewLine);
			//builder.Append("            }   " + Environment.NewLine);
			//builder.Append("            " + Environment.NewLine);

			//private get entity by key
			builder.Append("            public " + t.Name + " Get" + t.Name + "(int key)" + Environment.NewLine);
			builder.Append("            {" + Environment.NewLine);
			builder.Append("" + Environment.NewLine);
			builder.Append("                var query = from currentEntity in context." + t.Name + " where currentEntity." + t.PrimaryKeyFieldName + " == key select currentEntity;" + Environment.NewLine);
			builder.Append("            " + t.Name + "[] entities = qr.Execute<" + context.ContextName + "," + t.Name + ">(context, (DbQuery<" + t.Name + ">)query, false);" + Environment.NewLine);
			builder.Append("                if(entities.Length > 0)" + Environment.NewLine);
			builder.Append("                {");
			builder.Append("                     return entities[0]; " + Environment.NewLine);
			builder.Append("                 }" + Environment.NewLine);

			builder.Append("               return null;" + Environment.NewLine);
			builder.Append("           }" + Environment.NewLine);

			////get the lookup data field
			//PropertyInfo[] properties = t.Entity.GetProperties();

			//string fieldName = null;

			//foreach (PropertyInfo info in properties)
			//{
			//	bool result = info.PropertyType.ToString() == "System.String";

			//	if (result)
			//	{
			//		fieldName = info.Name;
			//		break;
			//	}
			//}

			//if (!string.IsNullOrEmpty(fieldName))
			//{
			//	//get lookup data
			//	builder.Append("            public Dictionary<int,string> GetLookupData()" + Environment.NewLine);
			//	builder.Append("            {" + Environment.NewLine);
			//	builder.Append("                var query = from entity in context." + t.Entity.Name + " orderby entity." + fieldName + " ascending select new { entity." + t.PrimaryKeyFieldName + ", entity." + fieldName + " };" + Environment.NewLine);
			//	builder.Append("                Dictionary<int,string> result = qr.GetLookupData<" + context.ContextName + ">(context, (ObjectQuery)query," + GetEscapedQuote() + t.PrimaryKeyFieldName + GetEscapedQuote() + "," + GetEscapedQuote() + fieldName + GetEscapedQuote() + ", false);" + Environment.NewLine);
			//	builder.Append("                return result; " + Environment.NewLine);
			//	builder.Append("            }");
			//}

			builder.Append("    }" + Environment.NewLine);
			builder.Append("}" + Environment.NewLine);

			return builder.ToString();
		}
	}
}