using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using voidsoft.efbog;

namespace voidsoft
{
	public partial class WebClassGenerator
	{
		private const int TEXT_FIELD_MAX_LENGTH = 300;
		private const int DEFAULT_VALUE_FOR_MULTILINE_TEXT = 500;

		private GeneratorContext context;

		public WebClassGenerator(GeneratorContext ctx)
		{
			context = ctx;
		}

		public string GenerateWebEditMarkup(EntityData e)
		{
			var b = new StringBuilder();

			b.Append("<%@ Page Language='C#' MasterPageFile='~/MasterPages/Admin.Master' AutoEventWireup='true' CodeBehind='" + e.Entity.Name + "Edit.aspx.cs' Inherits='" + context.UserSpecifiedNamespace + "." + e.Entity.Name + "Edit' %>");
			b.Append(Environment.NewLine);
			b.Append("<asp:Content ID='content' ContentPlaceHolderID='contentPlaceHolder' runat='server'>");
			b.Append(Environment.NewLine);
			b.Append(" <table width='100%'>");
			b.Append(Environment.NewLine);
			b.Append("<tr><td colspan='2' align='center'> <asp:label runat='server' id='labelTitle' CssClass='title'  Text='Create' /> <br/><br/> </td></tr>");
			b.Append(Environment.NewLine);

			foreach (var p in properties)
			{
				if (p.IsPrimaryKey)
				{
					continue;
				}

				b.Append("<tr><td valign='top' align='right' style='width:50%' >" + p.PropertyName + "</td>");
				b.Append(Environment.NewLine);
				b.Append("<td align='left'>" + GetMarkupForEdit(e, p) + "</td></tr>");
				b.Append(Environment.NewLine);
			}

			b.Append(Environment.NewLine);

			b.Append(GenerateMarkupForRelations(e));

			b.Append(Environment.NewLine);

			b.Append("<tr><td colspan='2'></td></tr>");
			b.Append(Environment.NewLine);
			b.Append("<tr><td colspan='2' align='center'>");
			b.Append(Environment.NewLine);
			b.Append("<asp:Button runat='server' Text='Create' id='buttonOk' CssClass='commandButton' OnClick='buttonOk_OnClick' />");
			b.Append(Environment.NewLine);
			b.Append("<asp:Button runat='server' Text='Cancel' id='buttonCancel' CssClass='commandButton' OnClick='buttonCancel_OnClick' />");
			b.Append(Environment.NewLine);
			b.Append("</td></tr>");
			b.Append(Environment.NewLine);
			b.Append("</table>");
			b.Append(Environment.NewLine);
			b.Append("</asp:content>");

			return b.ToString();
		}

		public string GenerateEditDesignerCode(EntityData e)
		{
			var b = new StringBuilder();

			b.Append("namespace " + context.UserSpecifiedNamespace);
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);

			b.Append("  public partial class " + e.Entity.Name + "Edit");
			b.Append(Environment.NewLine);

			b.Append("  {");
			b.Append(Environment.NewLine);

			b.Append("     protected global::System.Web.UI.WebControls.Label labelTitle;");
			b.Append(Environment.NewLine);

			b.Append("     protected global::System.Web.UI.WebControls.ObjectDataSource objectDataSource" + e.Entity.Name + " ;");
			b.Append(Environment.NewLine);

			foreach (var p in properties)
			{
				if (p.IsPrimaryKey)
				{
					continue;
				}

				b.Append(" protected global::System.Web.UI.WebControls." + GetControlType(e, p) + " " + GetControlName(e, p) + ";");
				b.Append(Environment.NewLine);
			}

			List<EntityRelationship> relationships = e.Relationships.FindAll(er => er.AssociationType == RelationshipType.Child);

			foreach (EntityRelationship r in relationships)
			{
				b.Append(" protected global::System.Web.UI.WebControls.DropDownList " + "dropDownList" + r.RelatedEntityName + ";");
				b.Append(Environment.NewLine);
			}

			b.Append(Environment.NewLine);
			b.Append("     protected global::System.Web.UI.WebControls.Button buttonOk;");

			b.Append(Environment.NewLine);
			b.Append("}}");

			return b.ToString();
		}

		public string GenerateEditCodeBehindClass(EntityData e)
		{
			var b = new StringBuilder();

			b.Append("using System;");
			b.Append(Environment.NewLine);
			b.Append("using System.Web.UI;");
			b.Append(Environment.NewLine);
			b.Append("using System.Web.UI.WebControls;");
			b.Append(Environment.NewLine);
			b.Append("using System.Data;");
			b.Append(Environment.NewLine);
			b.Append("using voidsoft.MicroRuntime;");
			b.Append(Environment.NewLine);
			b.Append("using voidsoft.Zinc;");
			b.Append(Environment.NewLine);
			b.Append("using " + context.UserSpecifiedNamespace + ";");
			b.Append(Environment.NewLine);
			b.Append("using " + context.EntitiesNamespaceName + ";");
			b.Append(Environment.NewLine);
			b.Append("using " + context.UserSpecifiedNamespace + ".PresentationServices;");
			b.Append(Environment.NewLine);
			b.Append("namespace " + context.UserSpecifiedNamespace);
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);
			b.Append("  public partial class " + e.Entity.Name + "Edit : Page");
			b.Append(Environment.NewLine);
			b.Append("  {");
			b.Append(Environment.NewLine);
			b.Append("      private bool isEditMode = false;");
			b.Append(Environment.NewLine);
			b.Append("      private " + e.Entity.Name + "EditPresentationService presenter = new " + e.Entity.Name + "EditPresentationService();");
			b.Append(Environment.NewLine);
			b.Append("      protected void Page_Load(object sender, EventArgs e)");
			b.Append(Environment.NewLine);
			b.Append("      {");
			b.Append(Environment.NewLine);
			b.Append("          if(! this.IsPostBack)");
			b.Append(Environment.NewLine);
			b.Append("          {");
			b.Append(Environment.NewLine);

			b.Append("              if(!string.IsNullOrEmpty(Request.QueryString[" + Constants.QUOTE + "rid" + Constants.QUOTE + "]))");
			b.Append(Environment.NewLine);
			b.Append("              {");
			b.Append(Environment.NewLine);
			b.Append("                    isEditMode = true;");
			b.Append(Environment.NewLine);
			b.Append("                    SetPageContextForEditMode();");
			b.Append(Environment.NewLine);
			b.Append("              }");
			b.Append(Environment.NewLine);
			b.Append("          }");
			b.Append(Environment.NewLine);
			b.Append("      }");
			b.Append(Environment.NewLine);

			b.Append("protected void buttonCancel_OnClick(object sender, EventArgs e)");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);
			b.Append("  Response.Redirect(" + Constants.QUOTE + e.Entity.Name + "View.aspx" + Constants.QUOTE + ",true); ");
			b.Append(Environment.NewLine);
			b.Append("}");

			b.Append(Environment.NewLine);

			b.Append(" protected void SetPageContextForEditMode()");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);
			b.Append("   " + e.Entity.Name + " entity = presenter.Get" + e.Entity.Name + "(Convert.ToInt32(Request.QueryString[" + Constants.QUOTE + "rid" + Constants.QUOTE + "]));");
			b.Append(Environment.NewLine);
			b.Append("  if(entity == null)");
			b.Append(Environment.NewLine);
			b.Append("  {");
			b.Append(Environment.NewLine);
			b.Append("      Response.Redirect(" + Constants.QUOTE + e.Entity.Name + "View.aspx" + Constants.QUOTE + ", true);");
			b.Append(Environment.NewLine);
			b.Append("   }");
			b.Append(Environment.NewLine);
			b.Append("    buttonOk.Text = " + Constants.QUOTE + "Update" + Constants.QUOTE + ";");
			b.Append(Environment.NewLine);
			b.Append("    labelTitle.Text =" + Constants.QUOTE + " Edit " + e.Entity.Name + Constants.QUOTE + ";");
			b.Append(Environment.NewLine);

			ColumnAnnotation ca = new ColumnAnnotation(this.context);

			foreach (EntityProperty p in properties)
			{
				if (p.IsPrimaryKey)
				{
					continue;
				}

				b.Append(Environment.NewLine);

				//check if it allows null
				if (p.IsNullable)
				{
					if (p.PropertyType == DbType.String)
					{
						//generate check for null code
						b.Append("if(! string.IsNullOrEmpty(entity." + p.PropertyName + "))");
						b.Append(Environment.NewLine);
					}
					else
					{
						b.Append("if(entity." + p.PropertyName + ".HasValue)");
						b.Append(Environment.NewLine);
					}

					b.Append("{ ");
					b.Append(Environment.NewLine);
					b.Append(GetControlName(e, p) + "." + GetControlReadValueProperty(p) + "= " + GenerateConversionFromNullableToDisplay(p) + ";");
					b.Append(" }");
				}
				else
				{
					if (ca.IsFileType(e, p))
					{
						continue;
					}

					b.Append(GetControlName(e, p) + "." + GetControlReadValueProperty(p) + "= entity." + p.PropertyName + ";");
				}
			}

			b.Append(Environment.NewLine);

			//set value for relations
			//check for FK relationships
			List<EntityRelationship> relationships = e.Relationships.FindAll(er => er.AssociationType == RelationshipType.Child);

			if (relationships.Count > 0)
			{
				b.Append(" DropDownListHelper helper = new DropDownListHelper(); ");

				b.Append(Environment.NewLine);

				b.Append("try");
				b.Append(Environment.NewLine);
				b.Append("{");

				foreach (EntityRelationship r in relationships)
				{
					//bind it first
					b.Append("dropDown" + r.RelatedEntityName + ".DataBind();");
					b.Append(Environment.NewLine);

					b.Append("helper.SelectItemByValue(" + "dropDown" + r.RelatedEntityName + "," + "entity." + r.RelatedEntityName + "Reference.EntityKey.EntityKeyValues[0].Value.ToString()" + ");");
					b.Append(Environment.NewLine);
				}

				b.Append(Environment.NewLine);
				b.Append("}");
				b.Append("catch(Exception ex)");
				b.Append(Environment.NewLine);
				b.Append("{");
				b.Append(Environment.NewLine);
				b.Append("  ");
				b.Append("}");
			}

			b.Append(" }");
			b.Append(Environment.NewLine);
			b.Append(Environment.NewLine);

			b.Append("  protected void buttonOk_OnClick(object sender, EventArgs e)");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);
			b.Append(" if(this.IsValid)");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);
			b.Append("  DoAction();");
			b.Append(Environment.NewLine);
			b.Append("}}");
			b.Append(Environment.NewLine);

			b.Append("private void DoAction()");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);

			b.Append("  try");
			b.Append(Environment.NewLine);
			b.Append("   {");

			b.Append("   bool isNew = (Request.QueryString[" + Constants.QUOTE + "rid" + Constants.QUOTE + "] == null);");
			b.Append(Environment.NewLine);
			b.Append("    " + e.Entity.Name + " entity = null;");
			b.Append(Environment.NewLine);
			b.Append("  if(isNew)");
			b.Append(Environment.NewLine);
			b.Append("  {");
			b.Append(Environment.NewLine);
			b.Append("      entity = new " + e.Entity.Name + "();");
			b.Append(Environment.NewLine);
			b.Append("  }");
			b.Append(Environment.NewLine);
			b.Append("  else   ");
			b.Append(Environment.NewLine);
			b.Append("  {  ");
			b.Append(Environment.NewLine);
			b.Append("      entity = presenter.Get" + e.Entity.Name + "(Convert.ToInt32(Request.QueryString[" + Constants.QUOTE + "rid" + Constants.QUOTE + "]));");
			b.Append(Environment.NewLine);
			b.Append("  }");
			b.Append(Environment.NewLine);

			foreach (var p in properties)
			{
				if (p.IsPrimaryKey)
				{
					continue;
				}

				//checkif allows null

				if (p.IsNullable)
				{
					bool hasConversion = false;
					string conversionCode = GenerateConversionForReading(p, ref hasConversion);
					string emptyCheckCode = GenerateEmptyCheck(e, p);

					if (hasConversion)
					{
						b.Append(emptyCheckCode);
						b.Append("{");
						b.Append("entity." + p.PropertyName + "=" + conversionCode + GetControlName(e, p) + "." + GetControlReadValueProperty(p) + ");");

						b.Append("}");
					}
					else
					{
						b.Append("entity." + p.PropertyName + "=" + GetControlName(e, p) + "." + GetControlReadValueProperty(p) + ";");
					}
				}
				else
				{
					//
					bool isFileType = (new ColumnAnnotation(this.context)).IsFileType(e, p);

					if (isFileType)
					{
						b.Append(" string path =  new FileUploaderHelper().UploadToFolder(ref  this.fileUpload" + p.PropertyName + ", ApplicationContext.FileUploadPath);");
						b.Append(Environment.NewLine);
						b.Append("entity." + p.PropertyName + " = path;");
					}
					else
					{
						b.Append("entity." + p.PropertyName + "=" + GetControlName(e, p) + "." + GetControlReadValueProperty(p) + ";");
					}
				}

				b.Append(Environment.NewLine);
			}

			//set the FK on relationships
			List<EntityRelationship> relations = e.Relationships.FindAll(er => er.AssociationType == RelationshipType.Child);

			foreach (EntityRelationship r in relations)
			{
				b.Append("entity." + r.RelatedEntityName + "Reference.EntityKey = new EntityKey(" + Constants.QUOTE + context.ContextName + "." + r.RelatedEntityName + Constants.QUOTE + "," + Constants.QUOTE + r.RelatedEntityPrimaryKey + Constants.QUOTE + "," + " Convert.ToInt32(dropDown" + r.RelatedEntityName + ".SelectedValue));");

				b.Append(Environment.NewLine);
			}

			b.Append(Environment.NewLine);

			b.Append("if(isNew)");
			b.Append(Environment.NewLine);
			b.Append(" {");
			b.Append(Environment.NewLine);
			b.Append("    presenter.Create(entity);");
			b.Append(Environment.NewLine);
			b.Append("  }");
			b.Append(Environment.NewLine);
			b.Append(" else");
			b.Append(Environment.NewLine);
			b.Append(" {");
			b.Append(Environment.NewLine);
			b.Append("   presenter.Update(entity);");
			b.Append(Environment.NewLine);
			b.Append("  }");
			b.Append(Environment.NewLine);
			b.Append("  Response.Redirect(" + Constants.QUOTE + e.Entity.Name + "View.aspx" + Constants.QUOTE + ", true);");
			b.Append(Environment.NewLine);
			b.Append("  }");
			b.Append("catch(Exception ex)");
			b.Append("{  Log.WriteTraceOutput(ex);}");
			b.Append(" }");
			b.Append(" }}");

			return b.ToString();
		}

		private  string GenerateWebEditPresentationService(EntityData e)
		{
			StringBuilder b = new StringBuilder();

			b.Append("using System;");
			b.Append(Environment.NewLine);
			b.Append("using System.Collections.Generic;");
			b.Append(Environment.NewLine);
			b.Append("using " + context.UserSpecifiedNamespace + ";");
			b.Append(Environment.NewLine);
			b.Append("using " + context.EntitiesNamespaceName + ";");
			b.Append(Environment.NewLine);

			b.Append("namespace " + context.UserSpecifiedNamespace + ".PresentationServices");

			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);

			b.Append("public class " + e.Entity.Name + "EditPresentationService");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);
			b.Append(e.Entity.Name + "BusinessObject  businessObject = new " + e.Entity.Name + "BusinessObject();");
			b.Append(Environment.NewLine);
			b.Append(" public " + e.Entity.Name + " Get" + e.Entity.Name + "(int id)");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append(Environment.NewLine);
			b.Append("return businessObject.Get" + e.Entity.Name + "(id);");
			b.Append(Environment.NewLine);
			b.Append("}");

			//generate lookup code for entities
			//check for FK relationships
			List<EntityRelationship> relationships = e.Relationships.FindAll(er => er.AssociationType == RelationshipType.Child);

			foreach (EntityRelationship r in relationships)
			{
				b.Append("public Dictionary<int,string> GetLookupDataFor" + r.RelatedEntityName + "()");
				b.Append(Environment.NewLine);
				b.Append("{");
				b.Append(Environment.NewLine);
				b.Append("  return new " + r.RelatedEntityName + "BusinessObject().GetLookupData();");
				b.Append(Environment.NewLine);
				b.Append("}");
				b.Append(Environment.NewLine);
			}

			b.Append(Environment.NewLine);

			b.Append(" public void Update(" + e.Entity.Name + " e )");
			b.Append(Environment.NewLine);
			b.Append(" {");

			b.Append(Environment.NewLine);
			b.Append("  businessObject.Update();");
			b.Append(Environment.NewLine);
			b.Append("}");
			b.Append(Environment.NewLine);
			b.Append(Environment.NewLine);
			b.Append(" public void Create(" + e.Entity.Name + " e )");
			b.Append(Environment.NewLine);
			b.Append("{");
			b.Append("   businessObject.Create(e); ");
			b.Append("}");
			b.Append("}}");

			return b.ToString();
		}

		private string GenerateMarkupForRelations(EntityData e)
		{
			var fkBuilder = new StringBuilder();
			//check for FK relationships
			List<EntityRelationship> relationships = e.Relationships.FindAll(er => er.AssociationType == RelationshipType.Child);

			foreach (EntityRelationship r in relationships)
			{
				if (!string.IsNullOrEmpty(r.RelatedEntityName))
				{
					fkBuilder.Append("<tr><td valign='top' align='right' style='width:50%' >" + r.RelatedEntityName + "</td>");
					fkBuilder.Append(Environment.NewLine);
					fkBuilder.Append("<td align='left'>"); // + GetMarkupForEdit(e, p) + "</td></tr>");
					fkBuilder.Append(Environment.NewLine);
					fkBuilder.Append("<asp:DropDownList runat='server' DataValueField='Key' DataTextField='Value' DataSourceId='objectSource" + r.RelatedEntityName + "'  id='dropDown" + r.RelatedEntityName + "' />");
					fkBuilder.Append(Environment.NewLine);
					fkBuilder.Append("<br/>");
					fkBuilder.Append(Environment.NewLine);
					fkBuilder.Append("<asp:ObjectDataSource runat='server' id='objectSource" + r.RelatedEntityName + "' TypeName='" + context.UserSpecifiedNamespace + ".PresentationServices." + e.Entity.Name + "EditPresentationService' SelectMethod='GetLookupDataFor" + r.RelatedEntityName + "' ></asp:ObjectDataSource>");
					fkBuilder.Append(Environment.NewLine);
					fkBuilder.Append("</td></tr>");
				}
			}

			return fkBuilder.ToString();
		}

		private string GetMarkupForEdit(EntityData e, EntityProperty p)
		{
			StringBuilder b = new StringBuilder();

			bool isFileUploadcontrol = false;
			string metadataBased = GetTemplateBasedOnMetadata(e, p, ref isFileUploadcontrol);
			if (!string.IsNullOrEmpty(metadataBased))
			{
				return metadataBased;
			}

			switch (p.PropertyType)
			{
				case DbType.AnsiString:
				case DbType.AnsiStringFixedLength:
				case DbType.String:

					int size = (new ColumnAnnotation(context)).GetFieldLength(e, p);

					b.Append("<asp:TextBox runat='server'  SkinId='textBoxSkin' id='" + GetControlName(e, p) + "'"); //" />");

					if (size == -1)
					{
						//this means the max length has not been defined so set the const value
						b.Append("  MaxLength='" + TEXT_FIELD_MAX_LENGTH + "' />");
					}
					else if (size > DEFAULT_VALUE_FOR_MULTILINE_TEXT)
					{
						b.Append(" width='200px' height='100px' TextMode='Multiline'  MaxLength='" + size + "'  />");
					}
					else
					{
						b.Append("  MaxLength='" + size + "' />");
					}

					if (!p.IsNullable)
					{
						b.Append(Environment.NewLine);
						b.Append("<asp:RequiredFieldValidator runat='server' id='requiredFieldValidator" + p.PropertyName + "' ControlToValidate='" + GetControlName(e, p) + "' Text='Required field'  />");
					}
					break;

				case DbType.DateTime2:
				case DbType.DateTime:
				case DbType.DateTimeOffset:
					b.Append("<asp:Calendar runat='server' id='calendar" + p.PropertyName + "'/>");
					break;

				case DbType.Boolean:
					b.Append("<asp:CheckBox runat='server' id='checkBox" + GetControlName(e, p) + "' />");
					break;

				case DbType.Binary:
					break;

				case DbType.Byte:
				case DbType.Currency:
				case DbType.Decimal:
				case DbType.Double:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.UInt16:
				case DbType.UInt64:
				case DbType.UInt32:
					b.Append("<asp:TextBox runat='server' MaxLength='15' id='" + GetControlName(e, p) + "' />");
					if (!p.IsNullable)
					{
						b.Append(Environment.NewLine);
						b.Append("<asp:RequiredFieldValidator runat='server' id='requiredFieldValidator" + p.PropertyName + "' ControlToValidate='" + GetControlName(e, p) + "' Text='Required field'  />");
					}
					break;
			}

			return b.ToString();
		}

		private string GenerateEmptyCheck(EntityData e, EntityProperty p)
		{
			switch (p.PropertyType)
			{
				case DbType.Byte:
				case DbType.Currency:
				case DbType.Decimal:
				case DbType.Double:
				case DbType.Int16:
				case DbType.Int32:
				case DbType.Int64:
				case DbType.SByte:
				case DbType.Single:
				case DbType.String:
				case DbType.UInt16:
				case DbType.UInt32:
				case DbType.UInt64:
					return "if(!string.IsNullOrEmpty(" + GetControlName(e, p) + ".Text))";
			}

			return string.Empty;
		}

		private string GenerateConversionForReading(EntityProperty e, ref bool hasConversion)
		{
			switch (e.PropertyType)
			{
				case DbType.Byte:
					hasConversion = true;
					return "Convert.ToByte(";

				case DbType.Double:
				case DbType.Currency:
					hasConversion = true;
					return "Convert.ToDouble(";

				case DbType.Decimal:
					hasConversion = true;
					return "Convert.ToDecimal(";

				case DbType.Guid:
					hasConversion = true;
					return " new Guid(";

				case DbType.Int16:
					hasConversion = true;
					return "Convert.ToInt16(";

				case DbType.Int32:
					hasConversion = true;
					return "Convert.ToInt32(";

				case DbType.Int64:
					hasConversion = true;
					return "Convert.ToInt64(";

				case DbType.SByte:
					hasConversion = true;
					return "Convert.ToSByte(";

				case DbType.Single:
					hasConversion = true;
					return "Convert.ToSingle(";

				case DbType.UInt16:
					hasConversion = true;
					return "Convert.ToUInt16(";

				case DbType.UInt32:
					hasConversion = true;
					return "Convert.ToUInt32(";

				case DbType.UInt64:
					hasConversion = true;
					return "Convert.ToUInt64";
			}

			return string.Empty;
		}

		private static string GenerateConversionFromNullableToDisplay(EntityProperty p)
		{
			switch (p.PropertyType)
			{
				case DbType.AnsiString:
				case DbType.String:
				case DbType.AnsiStringFixedLength:
				case DbType.StringFixedLength:
				case DbType.Xml:
					return "Convert.ToString(entity." + p.PropertyName + ")";

				case DbType.Binary:
					break;
				case DbType.Byte:
					return "Convert.ToString(" + p.PropertyName + ")";
				case DbType.Boolean:
					return "Convert.ToBoolean(entity." + p.PropertyName + ")";

				case DbType.Currency:
					break;
				case DbType.Date:
					return "Convert.ToDateTime(entity." + p.PropertyName + ")";
				case DbType.DateTime:
					return "Convert.ToDateTime(entity." + p.PropertyName + ")";
				case DbType.Decimal:
					return "Convert.ToString(entity." + p.PropertyName + ")";
				case DbType.Double:
					return "Convert.ToString(entity." + p.PropertyName + ")";
				case DbType.Guid:
					break;
				case DbType.Int16:
					return "Convert.ToString(entity." + p.PropertyName + ")";
				case DbType.Int32:
					return "Convert.ToString(entity." + p.PropertyName + ")";
				case DbType.Int64:
					return "Convert.ToString(entity." + p.PropertyName + ")";
				case DbType.Object:
					break;
				case DbType.SByte:
					break;
				case DbType.Single:
					break;

				case DbType.Time:
					return "Convert.ToDateTime(entity." + p.PropertyName + ")";
				case DbType.UInt16:
					return "Convert.ToString(entity." + p.PropertyName + ")";
				case DbType.UInt32:
					return "Convert.ToString(entity." + p.PropertyName + ")";
				case DbType.UInt64:
					return "Convert.ToString(entity." + p.PropertyName + ")";
				case DbType.VarNumeric:
					break;
				case DbType.DateTime2:
					return "Convert.ToDateTime(entity." + p.PropertyName + ")";
				case DbType.DateTimeOffset:
					return "Convert.ToDateTime(entity." + p.PropertyName + ")";
			}

			return "";
		}
	}
}