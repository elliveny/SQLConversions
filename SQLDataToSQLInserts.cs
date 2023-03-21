using System;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SQLInsertScriptGenerator
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

                // Retrieve the table data
                string sql = $"SELECT * FROM {tableName}";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        // Generate the insert statements
                        StringBuilder insertBuilder = new StringBuilder();

                        while (reader.Read())
                        {
                            insertBuilder.Append($"INSERT INTO {tableName} (");

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                insertBuilder.Append($"{reader.GetName(i)}, ");
                            }

                            insertBuilder.Remove(insertBuilder.Length - 2, 2);
                            insertBuilder.Append(") VALUES (");

                            for (int i = 0; i < reader.FieldCount; i++)
                            {
                                if (reader.IsDBNull(i))
                                {
                                    insertBuilder.Append("NULL, ");
                                }
                                else if (reader.GetFieldType(i) == typeof(string))
                                {
                                    string value = reader.GetString(i);
                                    insertBuilder.Append($"'{value.Replace("'", "''")}', ");
                                }
                                else if (reader.GetFieldType(i) == typeof(DateTime))
                                {
                                    DateTime value = reader.GetDateTime(i);
                                    insertBuilder.Append($"'{value.ToString("yyyy-MM-dd HH:mm:ss")}', ");
                                }
                                else
                                {
                                    object value = reader.GetValue(i);
                                    insertBuilder.Append($"{value}, ");
                                }
                            }

                            insertBuilder.Remove(insertBuilder.Length - 2, 2);
                            insertBuilder.AppendLine(");");
                        }

                        string insertScript = insertBuilder.ToString();
                        Console.WriteLine("INSERT script:");
                        Console.WriteLine(insertScript);
                    }
                }
            }

            Console.ReadKey();
        }
    }
}
