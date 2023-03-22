using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StoredProcedureToCSharpClass
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "YourConnectionStringHere";
            string storedProcedureName = "YourStoredProcedureNameHere";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(storedProcedureName, connection);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                connection.Open();
                SqlCommandBuilder.DeriveParameters(command);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("public class " + storedProcedureName + "Params");
                sb.AppendLine("{");

                foreach (SqlParameter parameter in command.Parameters)
                {
                    string parameterName = parameter.ParameterName.TrimStart('@');
                    string parameterType = GetCSharpType(parameter.SqlDbType);

                    sb.AppendLine("\tpublic " + parameterType + " " + parameterName + " { get; set; }");
                }

                sb.AppendLine("}");

                Console.WriteLine(sb.ToString());
            }
        }

        static string GetCSharpType(SqlDbType sqlType)
        {
            switch (sqlType)
            {
                case SqlDbType.BigInt:
                    return "long";
                case SqlDbType.Binary:
                case SqlDbType.Image:
                case SqlDbType.Timestamp:
                case SqlDbType.VarBinary:
                    return "byte[]";
                case SqlDbType.Bit:
                    return "bool";
                case SqlDbType.Char:
                case SqlDbType.NChar:
                case SqlDbType.NText:
                case SqlDbType.NVarChar:
                case SqlDbType.Text:
                case SqlDbType.VarChar:
                case SqlDbType.Xml:
                    return "string";
                case SqlDbType.DateTime:
                case SqlDbType.SmallDateTime:
                case SqlDbType.Date:
                case SqlDbType.Time:
                case SqlDbType.DateTime2:
                    return "DateTime";
                case SqlDbType.Decimal:
                case SqlDbType.Money:
                case SqlDbType.SmallMoney:
                    return "decimal";
                case SqlDbType.Float:
                    return "double";
                case SqlDbType.Int:
                    return "int";
                case SqlDbType.Real:
                    return "float";
                case SqlDbType.UniqueIdentifier:
                    return "Guid";
                case SqlDbType.SmallInt:
                    return "short";
                case SqlDbType.TinyInt:
                    return "byte";
                case SqlDbType.Variant:
                case SqlDbType.Udt:
                case SqlDbType.Structured:
                case SqlDbType.DateOffset:
                case SqlDbType.TimeStampOffset:
                case SqlDbType.DateTimeOffset:
                default:
                    throw new NotSupportedException("Unsupported SQL type: " + sqlType);
            }
        }
    }
}
