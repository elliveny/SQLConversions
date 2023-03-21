using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SQLCreateTableScriptGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Data Source=your_server_name;Initial Catalog=your_database_name;Integrated Security=True";
            string tableName = "your_table_name";

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                // Retrieve the table schema
                string sql = $"SELECT TOP 0 * FROM {tableName}";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        DataTable schemaTable = reader.GetSchemaTable();

                        // Generate the create table statement
                        StringBuilder createTableBuilder = new StringBuilder();

                        createTableBuilder.Append($"CREATE TABLE {tableName} (");

                        foreach (DataRow row in schemaTable.Rows)
                        {
                            string columnName = (string)row["ColumnName"];
                            Type dataType = (Type)row["DataType"];
                            bool isNullable = (bool)row["AllowDBNull"];
                            int maxLength = (int)row["ColumnSize"];

                            createTableBuilder.Append($"{columnName} {GetSqlType(dataType, maxLength)}");

                            if (!isNullable)
                            {
                                createTableBuilder.Append(" NOT NULL");
                            }

                            createTableBuilder.Append(", ");
                        }

                        createTableBuilder.Remove(createTableBuilder.Length - 2, 2);
                        createTableBuilder.Append(");");

                        string createTableScript = createTableBuilder.ToString();
                        Console.WriteLine("CREATE TABLE script:");
                        Console.WriteLine(createTableScript);
                    }
                }
            }

            Console.ReadKey();
        }

        private static string GetSqlType(Type dataType, int maxLength)
        {
            switch (dataType.Name)
            {
                case "String":
                    return $"VARCHAR({(maxLength == -1 ? "MAX" : maxLength.ToString())})";
                case "Int16":
                case "Int32":
                    return "INT";
                case "Int64":
                    return "BIGINT";
                case "DateTime":
                    return "DATETIME";
                case "Decimal":
                    return "DECIMAL(18,2)";
                case "Single":
                    return "FLOAT";
                case "Double":
                    return "DOUBLE";
                case "Byte":
                    return "TINYINT";
                case "Boolean":
                    return "BIT";
                default:
                    throw new NotImplementedException($"Unhandled data type: {dataType.Name}");
            }
        }
    }
}
