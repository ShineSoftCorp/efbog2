using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using voidsoft.efbog;

namespace voidsoft
{
	public partial class WebClassGenerator
	{
		//private const string TEXT_BOX = "TextBox";
		//private const string DROP_DOWN_LIST = "DropDownList";
		//private const string FILE_UPLOAD = "FileUpload";
		//private const string CHECK_BOX = "CheckBox";
		//private const string CALENDAR = "Calendar";
		//protected static EntityDefinitionProperty[] definitionProperties;
		//protected static string primaryKeyFieldName;

		//public void Generate()
		//{
		//	foreach (EntityDefinition t in context.Entities)
		//	{
		//		try
		//		{
		//			definitionProperties = (new EntityFrameworkTypeReflector(this.context)).GetProperties(t.Entity);
		//			primaryKeyFieldName = EntityFrameworkTypeReflector.GetPrimaryKeyName(t.Entity);

		//			string codeView = GenerateWebViewMarkup(t);
		//			string codeBehindClass = GenerateWebViewCodeBehindClass(t);
		//			string codeDesigner = GenerateWebViewDesignerClass(t);
		//			string codeViewPresenter = GenerateWebViewPresentationService(t);

		//			//edit page
		//			string codeEditMarkup = GenerateWebEditMarkup(t);
		//			string codeEditBehind = GenerateEditCodeBehindClass(t);
		//			string codeEditDesigner = GenerateEditDesignerCode(t);
		//			string codeEditPresenter = GenerateWebEditPresentationService(t);

		//			GenerateFile(context.Path + @"\\output\Views\" + t.Entity.Name + "View.aspx", codeView);
		//			GenerateFile(context.Path + @"\\output\Views\" + t.Entity.Name + "View.aspx.cs", codeBehindClass);
		//			GenerateFile(context.Path + @"\\output\Views\" + t.Entity.Name + "View.aspx.designer.cs", codeDesigner);
		//			GenerateFile(context.Path + @"\\output\Presenters\" + t.Entity.Name + "ViewPresentationService.cs", codeViewPresenter);

		//			GenerateFile(context.Path + @"\\output\Views\" + t.Entity.Name + "Edit.aspx", codeEditMarkup);
		//			GenerateFile(context.Path + @"\\output\Views\" + t.Entity.Name + "Edit.aspx.cs", codeEditBehind);
		//			GenerateFile(context.Path + @"\\output\Views\" + t.Entity.Name + "Edit.aspx.designer.cs", codeEditDesigner);
		//			GenerateFile(context.Path + @"\\output\Presenters\" + t.Entity.Name + "EditPresentationService.cs", codeEditPresenter);
		//		}
		//		catch (Exception e)
		//		{
		//			Console.WriteLine(e.Message);
		//			continue;
		//		}
		//	}
		//}

		//private  string GenerateWebViewMarkup(EntityDefinition e)
		//{
		//	StringBuilder b = new StringBuilder();

		//	b.Append("<%@ Page Language='C#' MasterPageFile='~/MasterPages/Admin.Master' AutoEventWireup='true' CodeBehind='" + e.Entity.Name + "View.aspx.cs' Inherits='" + context.UserSpecifiedNamespace + "." + e.Entity.Name + "View' %>");

		//	b.Append("<%@ Register Assembly='voidsoft.Zinc' Namespace='voidsoft.Zinc' TagPrefix='uc' %>");

		//	b.Append(Environment.NewLine);
		//	b.Append("<asp:Content ID='Content1' ContentPlaceHolderID='contentPlaceHolder' runat='server'>");

		//	b.Append(" <table width='100%'>");
		//	b.Append(Environment.NewLine);
		//	b.Append("<tr> <td align='center'><h3>");
		//	b.Append(e.Entity.Name + "</h3> </td></tr><tr>");
		//	b.Append(Environment.NewLine);
		//	b.Append("<td align='center'>");
		//	b.Append(Environment.NewLine);
		//	b.Append("<asp:GridView SkinID='gridViewSkin' OnRowCommand='gridView_RowCommand' ID='gridView' runat='server' AllowSorting='True' AllowPaging='True' AutoGenerateColumns='False' DataSourceID='objectDataSource" + e.Entity.Name + "'>");
		//	b.Append(" <Columns>");
		//	b.Append(Environment.NewLine);
		//	b.Append("<asp:TemplateField ItemStyle-HorizontalAlign='Left'>");
		//	b.Append(Environment.NewLine);
		//	b.Append("<ItemTemplate>");
		//	b.Append(Environment.NewLine);
		//	b.Append("<uc:ConfirmationButton ID='buttonDelete' CssClass='button' CommandArgument='<%# DataBinder.Eval(Container.DataItem, " + Constants.QUOTE + e.PrimaryKeyFieldName + Constants.QUOTE + ") %>' Text='Delete' runat='server' RequiresConfirmation='true' CommandName='linkDelete' ConfirmationMessage='Are you sure you want to delete this ?' /> ");
		//	b.Append("  </ItemTemplate>");
		//	b.Append(Environment.NewLine);
		//	b.Append("</asp:TemplateField>");

		//	//generate bound fields
		//	EntityDefinitionProperty linkDefinitionProperty = GetLinkField(e, definitionProperties);

		//	if (linkDefinitionProperty != null)
		//	{
		//		b.Append(Environment.NewLine);
		//		b.Append("<asp:TemplateField ItemStyle-HorizontalAlign='Left' HeaderText='" + linkDefinitionProperty.PropertyName + "'>");
		//		b.Append(Environment.NewLine);
		//		b.Append("<ItemTemplate>");
		//		b.Append(Environment.NewLine);
		//		b.Append("<asp:LinkButton CommandArgument='<%#DataBinder.Eval(Container.DataItem, " + Constants.QUOTE + primaryKeyFieldName + Constants.QUOTE + ")%>' CommandName='linkSelect' runat='server' Text='<%#DataBinder.Eval(Container.DataItem, " + Constants.QUOTE + linkDefinitionProperty.PropertyName + Constants.QUOTE + ")%>'></asp:LinkButton>");
		//		b.Append(Environment.NewLine);
		//		b.Append("</ItemTemplate>");
		//		b.Append(Environment.NewLine);
		//		b.Append("</asp:TemplateField>");
		//		b.Append(Environment.NewLine);
		//	}

		//	//check for foreign key
		//	List<EntityRelationship> childRelationships = e.Relationships.FindAll(re => re.AssociationType == RelationshipType.Child);

		//	foreach (var p in definitionProperties)
		//	{
		//		if (p.IsPrimaryKey)
		//		{
		//			continue;
		//		}

		//		EntityRelationship r = childRelationships.Find(er => er.RelatedEntityPrimaryKey == p.PropertyName);

		//		if (string.IsNullOrEmpty(r.RelatedEntityName))
		//		{
		//			continue;
		//		}

		//		if (p.PropertyName != primaryKeyFieldName)
		//		{
		//			if (linkDefinitionProperty != null)
		//			{
		//				if (p.PropertyName == linkDefinitionProperty.PropertyName)
		//				{
		//					continue;
		//				}
		//			}

		//			b.Append("<asp:BoundField DataField='" + p.PropertyName + "' HeaderText='" + p.PropertyName + "' ItemStyle-HorizontalAlign='Center' />");
		//		}
		//	}

		//	b.Append(" </Columns>");
		//	b.Append(Environment.NewLine);
		//	b.Append("</asp:GridView>");
		//	b.Append(Environment.NewLine);
		//	b.Append("<asp:ObjectDataSource id='objectDataSource" + e.Entity.Name + "' runat='server' TypeName=" + context.UserSpecifiedNamespace + ".PresentationServices." + e.Entity.Name + "ViewPresentationService  SelectMethod='Get" + e.Entity.Name + "' /> ");

		//	b.Append("    </td></tr><tr> <td align='center'>   <br /><br />");
		//	b.Append("<asp:Button runat='server' OnClick='buttonNew_Click' CssClass='commandButton' ID='buttonNew' Text='New' />");
		//	b.Append("  </td></tr></table></asp:Content>");

		//	return b.ToString();
		//}

		//private string GenerateWebViewCodeBehindClass(EntityDefinition e)
		//{
		//	var b = new StringBuilder();

		//	b.Append("using System;");
		//	b.Append(Environment.NewLine);
		//	b.Append("using System.Web.UI;");
		//	b.Append(Environment.NewLine);
		//	b.Append("using System.Web.UI.WebControls;");
		//	b.Append(Environment.NewLine);
		//	b.Append("using voidsoft.MicroRuntime;");
		//	b.Append(Environment.NewLine);
		//	b.Append("using " + context.UserSpecifiedNamespace + ".PresentationServices;");
		//	b.Append(Environment.NewLine);
		//	b.Append("namespace " + context.UserSpecifiedNamespace);
		//	b.Append(Environment.NewLine);
		//	b.Append("{");
		//	b.Append(Environment.NewLine);
		//	b.Append("public partial class " + e.Entity.Name + "View : Page");
		//	b.Append(Environment.NewLine);
		//	b.Append("{");
		//	b.Append(Environment.NewLine);

		//	b.Append("   private " + e.Entity.Name + "ViewPresentationService presenter = new " + e.Entity.Name + "ViewPresentationService();");
		//	b.Append(Environment.NewLine);
		//	b.Append("protected void Page_Load(object sender, EventArgs e)");
		//	b.Append(Environment.NewLine);
		//	b.Append("{");
		//	b.Append(Environment.NewLine);
		//	b.Append("}");
		//	b.Append(Environment.NewLine);

		//	b.Append("protected void gridView_RowCommand(object sender, GridViewCommandEventArgs e){");
		//	b.Append(Environment.NewLine);

		//	b.Append(" try{");
		//	b.Append(Environment.NewLine);

		//	b.Append("    string id = e.CommandArgument.ToString();");
		//	b.Append(Environment.NewLine);

		//	b.Append("if (e.CommandName == " + Constants.QUOTE + "linkSelect" + Constants.QUOTE + ")");
		//	b.Append(Environment.NewLine);

		//	b.Append("{");
		//	b.Append(Environment.NewLine);

		//	b.Append("        Response.Redirect(" + Constants.QUOTE + e.Entity.Name + "Edit.aspx?rid=" + Constants.QUOTE + " + id, true);");
		//	b.Append(Environment.NewLine);

		//	b.Append("}");
		//	b.Append(Environment.NewLine);

		//	b.Append("else if(e.CommandName == " + Constants.QUOTE + "linkDelete" + Constants.QUOTE + ")");
		//	b.Append(Environment.NewLine);

		//	b.Append("{");
		//	b.Append(Environment.NewLine);

		//	b.Append("        presenter.Delete(Convert.ToInt32(id));");
		//	b.Append(Environment.NewLine);
		//	b.Append("        Response.Redirect(Request.RawUrl, true);");
		//	b.Append(Environment.NewLine);

		//	b.Append("}}");
		//	b.Append(Environment.NewLine);

		//	b.Append("catch(Exception ex)");
		//	b.Append(Environment.NewLine);
		//	b.Append("{");
		//	b.Append(Environment.NewLine);

		//	b.Append(" Log.WriteTraceOutput(ex);");
		//	b.Append("}");
		//	b.Append(Environment.NewLine);

		//	b.Append("}");
		//	b.Append(Environment.NewLine);

		//	b.Append(" protected void buttonNew_Click(object sender, EventArgs e)");
		//	b.Append(Environment.NewLine);
		//	b.Append("{");
		//	b.Append(Environment.NewLine);
		//	b.Append("    Response.Redirect(" + Constants.QUOTE + e.Entity.Name + "Edit.aspx" + Constants.QUOTE + ", true);");
		//	b.Append("}");

		//	b.Append("}}");

		//	return b.ToString();
		//}

		//private  string GenerateWebViewDesignerClass(EntityDefinition e)
		//{
		//	var b = new StringBuilder();

		//	b.Append("namespace " + context.UserSpecifiedNamespace);
		//	b.Append(Environment.NewLine);
		//	b.Append("{");
		//	b.Append(Environment.NewLine);

		//	b.Append("  public partial class " + e.Entity.Name + "View");
		//	b.Append(Environment.NewLine);

		//	b.Append("  {");
		//	b.Append(Environment.NewLine);

		//	b.Append("     protected global::System.Web.UI.WebControls.GridView gridView;");
		//	b.Append(Environment.NewLine);

		//	b.Append("     protected global::System.Web.UI.WebControls.ObjectDataSource objectDataSource" + e.Entity.Name + " ;");
		//	b.Append(Environment.NewLine);

		//	b.Append("     protected global::System.Web.UI.WebControls.Button buttonNew;");
		//	//b.Append(
		//	//        "public new voidsoft.Allegretto.AdminMaster Master {get {return ((voidsoft.Allegretto.AdminMaster)(base.Master));");
		//	//}
		//	b.Append("}}");

		//	return b.ToString();
		//}

		//private string GenerateWebViewPresentationService(EntityDefinition e)
		//{
		//	var b = new StringBuilder();

		//	b.Append("using System;");
		//	b.Append(Environment.NewLine);
		//	b.Append("using " + context.UserSpecifiedNamespace + ";");
		//	b.Append(Environment.NewLine);
		//	b.Append("using " + context.EntitiesNamespaceName + ";");
		//	b.Append(Environment.NewLine);

		//	b.Append("namespace " + context.UserSpecifiedNamespace + ".PresentationServices");

		//	b.Append(Environment.NewLine);
		//	b.Append("{");
		//	b.Append(Environment.NewLine);

		//	b.Append("public class " + e.Entity.Name + "ViewPresentationService");
		//	b.Append(Environment.NewLine);
		//	b.Append("{");
		//	b.Append(" private " + e.Entity.Name + "BusinessObject  businessObject = new " + e.Entity.Name + "BusinessObject();");
		//	b.Append(Environment.NewLine);
		//	b.Append(Environment.NewLine);
		//	b.Append(" public " + e.Entity.Name + "[] Get" + e.Entity.Name + "()");
		//	b.Append(Environment.NewLine);
		//	b.Append(" {");
		//	b.Append(Environment.NewLine);
		//	b.Append("    return businessObject.Get" + e.Entity.Name + "();");
		//	b.Append(Environment.NewLine);
		//	b.Append(" }");
		//	b.Append(Environment.NewLine);
		//	b.Append(" public void Delete(int id)");
		//	b.Append(Environment.NewLine);
		//	b.Append("{");
		//	b.Append("     businessObject.Delete(id); ");
		//	b.Append("}");
		//	b.Append("}}");

		//	return b.ToString();
		//}

		//#region control related

		//private static void GenerateFile(string filePath, string content)
		//{
		//	try
		//	{
		//		if (File.Exists(filePath))
		//		{
		//			Console.WriteLine("File " + filePath + " already exists. Skipped code generation for this.");
		//			return;
		//		}

		//		File.WriteAllText(filePath, content);
		//	}
		//	catch (Exception ex)
		//	{
		//		Console.WriteLine(ex.Message);
		//	}
		//}

		//private EntityDefinitionProperty GetLinkField(EntityDefinition e, EntityDefinitionProperty[] fields)
		//{
		//	var foreignKeyLinks = new List<string>();

		//	foreach (EntityRelationship entity in e.Relationships)
		//	{
		//		EntityDefinition d = context.Entities.Find(f => f.Entity.Name == entity.RelatedEntityName);
		//		foreignKeyLinks.Add(d.PrimaryKeyFieldName);
		//	}

		//	foreach (EntityDefinitionProperty property in fields)
		//	{
		//		//also look for 

		//		if (property.IsPrimaryKey == false && !foreignKeyLinks.Contains(property.PropertyName))
		//		{
		//			return property;
		//		}
		//	}
		//	return fields[0];
		//}

		//private string GetControlType(EntityDefinition e, EntityDefinitionProperty p)
		//{
		//	bool isFileUpload = false;

		//	string metadata = GetTemplateBasedOnMetadata(e, p, ref isFileUpload);

		//	if (!string.IsNullOrEmpty(metadata))
		//	{
		//		if (isFileUpload)
		//		{
		//			return FILE_UPLOAD;
		//		}
		//		return DROP_DOWN_LIST;
		//	}

		//	switch (p.PropertyType)
		//	{
		//		case DbType.AnsiString:
		//		case DbType.AnsiStringFixedLength:
		//		case DbType.String:
		//			return TEXT_BOX;

		//		case DbType.DateTime2:
		//		case DbType.DateTime:
		//		case DbType.DateTimeOffset:
		//			return CALENDAR;

		//		case DbType.Boolean:
		//			return CHECK_BOX;

		//		case DbType.Binary:
		//			break;

		//		case DbType.Byte:
		//		case DbType.Currency:
		//		case DbType.Decimal:
		//		case DbType.Double:
		//		case DbType.Int16:
		//		case DbType.Int32:
		//		case DbType.Int64:
		//		case DbType.UInt16:
		//		case DbType.UInt64:
		//		case DbType.UInt32:
		//			return TEXT_BOX;
		//	}

		//	return string.Empty;
		//}

		//private static string GetControlReadValueProperty(EntityDefinitionProperty e)
		//{
		//	switch (e.PropertyType)
		//	{
		//		case DbType.AnsiString:
		//		case DbType.AnsiStringFixedLength:
		//		case DbType.String:
		//			return "Text";

		//		case DbType.DateTime2:
		//		case DbType.DateTime:
		//		case DbType.DateTimeOffset:
		//			return "SelectedDate";

		//		case DbType.Boolean:
		//			return "Checked";

		//		case DbType.Binary:
		//			break;

		//		case DbType.Byte:
		//		case DbType.Currency:
		//		case DbType.Decimal:
		//		case DbType.Double:
		//		case DbType.Int16:
		//		case DbType.Int32:
		//		case DbType.Int64:
		//		case DbType.UInt16:
		//		case DbType.UInt64:
		//		case DbType.UInt32:
		//			return "Text";
		//	}

		//	return string.Empty;
		//}

		//private string GetControlName(EntityDefinition e, EntityDefinitionProperty p)
		//{
		//	bool hasFile = (new ColumnAnnotation(context)).IsFileType(e, p);

		//	if (hasFile)
		//	{
		//		return "fileUpload" + p.PropertyName;
		//	}

		//	var d = new Dictionary<int, string>();

		//	bool hasEnum = (new ColumnAnnotation(context)).IsEnumType(e, p, ref d);

		//	if (hasEnum)
		//	{
		//		return "dropDown" + p.PropertyName;
		//	}

		//	switch (p.PropertyType)
		//	{
		//		case DbType.AnsiString:
		//		case DbType.AnsiStringFixedLength:
		//		case DbType.String:
		//			return "textBox" + p.PropertyName;

		//		case DbType.DateTime2:
		//		case DbType.DateTime:
		//		case DbType.DateTimeOffset:
		//			return "calendar" + p.PropertyName;

		//		case DbType.Boolean:
		//			return "checkBox" + p.PropertyName;

		//		case DbType.Binary:
		//			break;

		//		case DbType.Byte:
		//		case DbType.Currency:
		//		case DbType.Decimal:
		//		case DbType.Double:
		//		case DbType.Int16:
		//		case DbType.Int32:
		//		case DbType.Int64:
		//		case DbType.UInt16:
		//		case DbType.UInt64:
		//		case DbType.UInt32:
		//			return "textBox" + p.PropertyName;
		//	}

		//	return string.Empty;
		//}

		///// <summary>
		/////     Gets the template based on metadata.
		///// </summary>
		///// <param name="e">The e.</param>
		///// <param name="p">The p.</param>
		///// <param name="isFileUpload">
		/////     if set to <c>true</c> [is file upload].
		///// </param>
		///// <returns></returns>
		//private string GetTemplateBasedOnMetadata(EntityDefinition e, EntityDefinitionProperty p, ref bool isFileUpload)
		//{
		//	bool hasFile = (new ColumnAnnotation(this.context)).IsFileType(e, p);

		//	var b = new StringBuilder();

		//	if (hasFile)
		//	{
		//		b.Append("<asp:FileUpload runat='server' id='fileUpload" + p.PropertyName + "'/>");

		//		return b.ToString();
		//	}

		//	Dictionary<int, string> enumValues = new Dictionary<int, string>();

		//	bool hasEnumValues = (new ColumnAnnotation(context)).IsEnumType(e, p, ref enumValues);

		//	if (hasEnumValues)
		//	{
		//		b.Append("<asp:DropDownList runat='server' id='dropDownList" + p.PropertyName + "' >");

		//		foreach (KeyValuePair<int, string> pair in enumValues)
		//		{
		//			b.Append("<asp:ListItem Text='" + pair.Value + "' Value='" + pair.Key + "' </asp:ListItem> ");
		//		}

		//		b.Append("</asp:DropDownList>");

		//		return b.ToString();
		//	}

		//	return b.ToString();
		//}

		//#endregion
	}
}