using System;
using System.IO;
using System.Text;
using voidsoft.efbog;

namespace voidsoft.Generators
{
	public class WebGridViewWithEntityDataSourceGenerator
	{

		private GeneratorContext context;

		public WebGridViewWithEntityDataSourceGenerator(GeneratorContext ctx)
		{
			context = ctx;
		}

		public void Generate()
		{
			foreach (EntityDefinition t in context.Entities)
			{
				try
				{
					string codeView = GenerateMarkup(t);
					GenerateFile(context.Path + @"\\output\views\" + t.Name + "ViewES.aspx", codeView);
				}
				catch (Exception e)
				{
					Console.WriteLine(e.Message);
					continue;
				}
			}
		}

		private string GenerateMarkup(EntityDefinition definition)
		{
			StringBuilder b = new StringBuilder();

			//EntityDefinitionProperty[] definitionProperties = (new EntityFrameworkTypeReflector(context)).GetProperties(definition.Entity);

			//string primaryKeyFieldName = EntityFrameworkTypeReflector.GetPrimaryKeyName(definition.Entity);

			//b.Append("<%@ Page Language='C#' MasterPageFile='~/MasterPages/Master.Master' AutoEventWireup='true' CodeBehind='" + definition.Entity.Name + "View.aspx.cs' Inherits='" + context.UserSpecifiedNamespace + "." + definition.Entity.Name + "View' %>");

			//b.Append(Environment.NewLine);
			//b.Append("<asp:Content ID='content' ContentPlaceHolderID='contentPlaceholder' runat='server'>");

			//b.Append(" <table width='100%'>");
			//b.Append(Environment.NewLine);
			//b.Append("<tr> <td align='center'><h3>");
			//b.Append(definition.Entity.Name + "</h3> </td></tr><tr>");
			//b.Append(Environment.NewLine);
			//b.Append("<td align='center'>");
			//b.Append(Environment.NewLine);
			//b.Append("<asp:GridView SkinID='gridViewSkin' OnRowCommand='gridView_RowCommand' ID='gridView' runat='server' AllowSorting='True' AllowPaging='True' AutoGenerateColumns='False' DataSourceID='entityDataSource" + definition.Entity.Name + "'>");
			//b.Append(" <Columns>");
			//b.Append(Environment.NewLine);
			//b.Append("<asp:TemplateField ItemStyle-HorizontalAlign='Left'>");
			//b.Append(Environment.NewLine);
			//b.Append("<ItemTemplate>");
			//b.Append("<uc:ConfirmationButton ID='buttonDelete' CssClass='button' CommandArgument='<%# DataBinder.Eval(Container.DataItem, '" + definition.PrimaryKeyFieldName + "') %>' Text='Delete' runat='server' RequiresConfirmation='true' CommandName='linkDelete' ConfirmationMessage='Are you sure you want to delete this ?' /> ");
			//b.Append("  </ItemTemplate></asp:TemplateField>");

			//foreach (var p in definitionProperties)
			//{
			//	if (p.PropertyName != primaryKeyFieldName)
			//	{
			//		b.Append("<asp:BoundField DataField='" + p.PropertyName + "' HeaderText=" + p.PropertyName + "' ItemStyle-HorizontalAlign='Center' />");
			//	}
			//}

			//b.Append(" </Columns></asp:GridView>");
			//b.Append(Environment.NewLine);
			//b.Append("<asp:EntityDataSource ID=" + Constants.QUOTE + "entityDataSource" + definition.Entity.Name + Constants.QUOTE + " runat='server' " + "ConnectionString=" + Constants.QUOTE + "name=" + context.ContextName + Constants.QUOTE + " DefaultContainerName='" + context.ContextName + "' " + " EntitySetName='" + definition.Entity.Name + "' " + " Select='");

			////generate the select for entity definition source
			//var builder = new StringBuilder();

			//for (int i = 0; i < definitionProperties.Length; i++)
			//{
			//	builder.Append("it.[" + definitionProperties[i].PropertyName + "]" + (i != definitionProperties.Length - 1 ? "," : ""));
			//}

			//b.Append(builder + "' />");
			//b.Append(Environment.NewLine);

			//b.Append("    </td></tr><tr> <td align='center'>   <br /><br />");
			//b.Append(Environment.NewLine);

			//b.Append("<asp:Button runat='server' OnClick='buttonNew_Click' ID='buttonNew' Text='New' />");
			//b.Append(Environment.NewLine);

			//b.Append("  </td></tr></table></asp:Content>");

			return b.ToString();
		}

		private string GenerateWebViewCodeBehindClass(EntityDefinition e)
		{
			var b = new StringBuilder();

			b.Append("using System;");
			b.Append(Environment.NewLine);
			b.Append("using System.Web.UI;");
			b.Append(Environment.NewLine);
			b.Append("using System.Web.UI.WebControls;");
			b.Append(Environment.NewLine);
			b.Append("using voidsoft.MicroRuntime;");
			b.Append(Environment.NewLine);
			b.Append("using " + context.UserSpecifiedNamespace + ".PresentationServices;");
			b.Append(Environment.NewLine);
			b.Append("namespace " + context.UserSpecifiedNamespace);
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);
			b.Append("public partial class " + e.Name + "View : Page");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);

			b.Append("   private " + e.Name + "ViewPresentationService presenter = new " + e.Name + "ViewPresentationService();");
			b.Append(Environment.NewLine);
			b.Append("protected void Page_Load(object sender, EventArgs e)");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);
			b.Append("}");
			b.Append(Environment.NewLine);

			b.Append("protected void gridView_RowCommand(object sender, GridViewCommandEventArgs e){");
			b.Append(Environment.NewLine);

			b.Append(" try{");
			b.Append(Environment.NewLine);

			b.Append("    string id = e.CommandArgument.ToString();");
			b.Append(Environment.NewLine);

			b.Append("if (e.CommandName == " + Constants.QUOTE + "linkSelect" + Constants.QUOTE + ")");
			b.Append(Environment.NewLine);

			b.Append("{");
			b.Append(Environment.NewLine);

			b.Append("        Response.Redirect(" + Constants.QUOTE + e.Name + "Edit.aspx?rid=" + Constants.QUOTE + " + id, true);");
			b.Append(Environment.NewLine);

			b.Append("}");
			b.Append(Environment.NewLine);

			b.Append("else if(e.CommandName == " + Constants.QUOTE + "linkDelete" + Constants.QUOTE + ")");
			b.Append(Environment.NewLine);

			b.Append("{");
			b.Append(Environment.NewLine);

			b.Append("        presenter.Delete(Convert.ToInt32(id));");
			b.Append(Environment.NewLine);
			b.Append("        Response.Redirect(Request.RawUrl, true);");
			b.Append(Environment.NewLine);

			b.Append("}}");
			b.Append(Environment.NewLine);

			b.Append("catch(Exception ex)");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);

			b.Append(" Log.WriteTraceOutput(ex);");
			b.Append("}");
			b.Append(Environment.NewLine);

			b.Append("}");
			b.Append(Environment.NewLine);

			b.Append(" protected void buttonNew_Click(object sender, EventArgs e)");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);
			b.Append("    Response.Redirect(" + Constants.QUOTE + e.Name + "Edit.aspx" + Constants.QUOTE + ", true);");
			b.Append("}");

			b.Append("}}");

			return b.ToString();
		}

		private static void GenerateFile(string filePath, string content)
		{
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
	}
}