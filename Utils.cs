using System;
using System.Data;

namespace voidsoft
{
	public class Utils
	{
		public DbType GetDbType(Type t)
		{
			//check if it's nullable
			if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(Nullable<>))
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
 
	}
}