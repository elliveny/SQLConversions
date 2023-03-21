using System;
using System.Reflection;
using System.Text;

namespace SQLCreateTableScriptGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            string className = "YourClassName";

            Type classType = Type.GetType(className);

            if (classType == null)
            {
                Console.WriteLine($"Could not find class {className}");
                return;
            }

            PropertyInfo[] properties = classType.GetProperties();

            StringBuilder createTableBuilder = new StringBuilder();
            createTableBuilder.Append($"CREATE TABLE {className} (");

            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                Type dataType = property.PropertyType;
                bool isNullable = !dataType.IsValueType || Nullable.GetUnderlyingType(dataType) != null;
                int maxLength = 50;

                createTableBuilder.Append($"{propertyName} {GetSqlType(dataType, maxLength)}");

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
