using System;
using System.Data.SqlClient;
using System.Text;

namespace SQLToCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=your_server_name;Initial Catalog=your_database_name;Integrated Security=True";
            string viewName = "your_view_name";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string sql = "SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @ViewName ORDER BY ORDINAL_POSITION";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ViewName", viewName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        StringBuilder sb = new StringBuilder();

                        sb.AppendLine("public class " + viewName);
                        sb.AppendLine("{");

                        while (reader.Read())
                        {
                            string columnName = reader.GetString(0);
                            string dataType = reader.GetString(1);
                            bool isNullable = reader.GetString(2) == "YES";

                            string csharpDataType = GetCSharpDataType(dataType, isNullable);

                            sb.AppendLine("\tpublic " + csharpDataType + " " + columnName + " { get; set; }");
                        }

                        sb.AppendLine("}");

                        Console.WriteLine(sb.ToString());
                    }
                }
            }

            Console.ReadKey();
        }

        static string GetCSharpDataType(string dataType, bool isNullable)
        {
            switch (dataType)
            {
                case "int":
                case "tinyint":
                case "smallint":
                case "bigint":
                    return isNullable ? "int?" : "int";
                case "nvarchar":
                case "varchar":
                case "char":
                case "text":
                case "ntext":
                    return "string";
                case "datetime":
                case "smalldatetime":
                    return isNullable ? "DateTime?" : "DateTime";
                case "bit":
                    return isNullable ? "bool?" : "bool";
                case "decimal":
                case "numeric":
                case "money":
                case "smallmoney":
                    return isNullable ? "decimal?" : "decimal";
                case "float":
                    return isNullable ? "double?" : "double";
                default:
                    return "object";
            }
        }
    }
}
