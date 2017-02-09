using System;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace CSharp6MagicNoSqlInjectionMethod
{
    /// <summary>
    /// hard core version of this on Jon Skeet's GitHub account
    /// http://go.beeming.net/2k8kpFW
    /// </summary>
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
            Console.WriteLine();
            for (int index = 0; index < theCommand.Parameters.Count; index++)
            {
                Console.WriteLine($"{theCommand.Parameters[index].ParameterName} = {theCommand.Parameters[index].Value}");
            }
            Console.WriteLine();
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
            for (int index = 0; index < args.Length; index++)
            {
                command.Parameters.AddWithValue($"@p{index}", args[index]);
            }
            return command;
        }
    }
}
