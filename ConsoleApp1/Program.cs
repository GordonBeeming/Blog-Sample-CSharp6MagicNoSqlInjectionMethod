using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace CSharp6MagicNoSqlInjectionMethod
{
    class Program
    {
        static void Main(string[] args)
        {
            // assume these come from inputs to make it scary
            var name = "Gordon";
            var surname = "Beeming";

            var sqlConnection = new SqlConnection();
            var theCommand = ToSqlCommand($@"
SELECT * 
FROM dbo.tb_People 
WHERE Name = {name} AND Surname = {surname}", sqlConnection);
            Console.WriteLine(theCommand.CommandText);
        }

        public static SqlCommand ToSqlCommand(FormattableString formattableString, SqlConnection sqlConnection)
        {
            var args = formattableString.GetArguments();
            var sql = string.Format(CultureInfo.InvariantCulture, 
                                        formattableString.Format, 
                                        Enumerable.Range(0, args.Length)
                                                    .Select(index => $"@p{index}")
                                                    .ToArray());
            var command = new SqlCommand(sql, sqlConnection);
            for(int index = 0; index < args.Length;index++)
            {
                command.Parameters.AddWithValue($"@p{index}", args[index]);
            }
            return command;
        }
    }
}
