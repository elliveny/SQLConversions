using System;
using System.Data.SqlClient;
using System.Text;

namespace SQLScriptGenerator
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

                // Get column information for the table
                string sql = "SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = @TableName ORDER BY ORDINAL_POSITION";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@TableName", tableName);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Generate SELECT statement
                        StringBuilder selectBuilder = new StringBuilder();
                        selectBuilder.Append($"SELECT {GetColumnList(reader)} FROM {tableName}");
                        string selectScript = selectBuilder.ToString();
                        Console.WriteLine("SELECT script:");
                        Console.WriteLine(selectScript);

                        // Reset reader to get column information again
                        reader.Close();
                        reader.Read();

                        // Generate INSERT statement
                        StringBuilder insertBuilder = new StringBuilder();
                        insertBuilder.Append($"INSERT INTO {tableName} ({GetColumnList(reader)}) VALUES ({GetParameterList(reader)})");
                        string insertScript = insertBuilder.ToString();
                        Console.WriteLine("INSERT script:");
                        Console.WriteLine(insertScript);

                        // Reset reader to get column information again
                        reader.Close();
                        reader.Read();

                        // Generate UPDATE statement
                        StringBuilder updateBuilder = new StringBuilder();
                        updateBuilder.Append($"UPDATE {tableName} SET {GetUpdateList(reader)} WHERE {GetWhereClause(reader)}");
                        string updateScript = updateBuilder.ToString();
                        Console.WriteLine("UPDATE script:");
                        Console.WriteLine(updateScript);
                    }
                }
            }

            Console.ReadKey();
        }

        static string GetColumnList(SqlDataReader reader)
        {
            StringBuilder columnBuilder = new StringBuilder();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                columnBuilder.Append($"{reader.GetName(i)}, ");
            }

            return columnBuilder.ToString().TrimEnd(' ', ',');
        }

        static string GetParameterList(SqlDataReader reader)
        {
            StringBuilder parameterBuilder = new StringBuilder();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                parameterBuilder.Append($"@{reader.GetName(i)}, ");
            }

            return parameterBuilder.ToString().TrimEnd(' ', ',');
        }

        static string GetUpdateList(SqlDataReader reader)
        {
            StringBuilder updateBuilder = new StringBuilder();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                updateBuilder.Append($"{columnName} = @{columnName}, ");
            }

            return updateBuilder.ToString().TrimEnd(' ', ',');
        }

        static string GetWhereClause(SqlDataReader reader)
        {
            StringBuilder whereBuilder = new StringBuilder();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                whereBuilder.Append($"{columnName} = @{columnName} AND ");
            }

            return whereBuilder.ToString().TrimEnd(' ', 'A', 'N', 'D');
        }
    }
}
