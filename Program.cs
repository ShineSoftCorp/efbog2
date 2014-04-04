using System;
using System.IO;
using System.Windows.Forms;

namespace voidsoft.efbog
{
	internal static class Program
	{
		[STAThread]
		private static void Main(string[] args)
		{
			if (args.Length == 0)
			{
				Console.WriteLine("efbog [assemblypath] [n=namespacename]");
				Console.WriteLine(@"efbog c:\entities.dll voidsoft.BusinessObjects");
				return;
			}

			string path = args[0];
			string namespaceGenerator = args.Length == 1 ? "voidsoft" : args[1];


			if (!File.Exists(path))
			{
				Console.WriteLine("Invalid input assembly path. Please make sure that the path doesn't contain empty spaces");
				return;
			}


			GeneratorContext context = new GeneratorContext();
			context.UserSpecifiedNamespace = namespaceGenerator;
			context.Path = Application.StartupPath;

			//generate output folders
			if (!Directory.Exists(context.Path + @"\output"))
			{
				Directory.CreateDirectory(context.Path + @"\output");

				if (!Directory.Exists(context.Path + @"\output\BusinessObjects"))
				{
					Directory.CreateDirectory(context.Path + @"\output\BusinessObjects\");
				}

				if (!Directory.Exists(context.Path + @"\output\Presenters"))
				{
					Directory.CreateDirectory(context.Path + @"\output\Presenters\");
				}

				if (!Directory.Exists(context.Path + @"\output\Views"))
				{
					Directory.CreateDirectory(context.Path + @"\output\Views\");
				}
			}

			try
			{
				
				(new EntityFrameworkTypeReflector(context)).ReflectAndGenerate(path);

				Console.WriteLine("Finished code generation");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message + e.StackTrace);
			}
		}
	}
}