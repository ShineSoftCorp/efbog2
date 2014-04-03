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

			GeneratorContext.UserSpecifiedNamespace = namespaceGenerator;
			GeneratorContext.Path = Application.StartupPath;

			//generate output folders
			if (!Directory.Exists(GeneratorContext.Path + @"\output"))
			{
				Directory.CreateDirectory(GeneratorContext.Path + @"\output");

				if (!Directory.Exists(GeneratorContext.Path + @"\output\BusinessObjects"))
				{
					Directory.CreateDirectory(GeneratorContext.Path + @"\output\BusinessObjects\");
				}

				if (!Directory.Exists(GeneratorContext.Path + @"\output\Presenters"))
				{
					Directory.CreateDirectory(GeneratorContext.Path + @"\output\Presenters\");
				}

				if (!Directory.Exists(GeneratorContext.Path + @"\output\Views"))
				{
					Directory.CreateDirectory(GeneratorContext.Path + @"\output\Views\");
				}
			}

			try
			{
				EntityFrameworkTypeReflector.ReflectAndGenerate(path);
				Console.WriteLine("Finished code generation");
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message + e.StackTrace);
			}
		}
	}
}