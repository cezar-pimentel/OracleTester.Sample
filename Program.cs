using Oracle.ManagedDataAccess.Client;
using OracleTester.Sample.Helpers;
using System;
using System.Data;

namespace OracleTester.Sample
{
	internal class Program
	{
		private static void Main()
		{
			//ExecuteScalar : OK
			Console.ForegroundColor = ConsoleColor.Cyan;
			using (var sql = new OracleHelper())
			{
				sql.Add("select 1 from dual");
				var ret = Convert.ToString(sql.ExecuteScalar());
				Console.WriteLine($"ExecuteScalar runned ok. Selected: [{ret}]");
			}

			//ExecuteNonQuery returning rowsAffected : OK
			Console.ForegroundColor = ConsoleColor.Magenta;
			using (var sql = new OracleHelper())
			{
				sql.Add("insert into table_name (col1) values (:col1)");
				sql.Add(":col1", $"COL1 {new Random().Next(1, 100)}");
				var rowsAffected = sql.ExecuteNonQuery();
				Console.WriteLine($"ExecuteNonQuery runned ok. Rows Affected: [{rowsAffected}]");
			}

			//ExecuteNonQuery returning id: OK
			Console.ForegroundColor = ConsoleColor.Green;
			using (var sql = new OracleHelper())
			{
				sql.Add("insert into table_name (col1) values (:col1) RETURNING id INTO :id");
				sql.Add(":col1", "COL1 TEST");
				sql.Add(":id", null, OracleDbType.Int32, ParameterDirection.ReturnValue);
				sql.ExecuteNonQuery();
				var ret = sql.GetParameter(":id");
				Console.WriteLine($"ExecuteNonQuery with return data runned ok. Returned id: [{ret}]");
			}

			//ExecuteNonQuery: OK
			for (var i = 1; i <= 100; i++)
			{
				using (var sql = new OracleHelper())
				{
					sql.Add("insert into table_name (col1, col2, col3, col4, col5, col6) values (:col1, :col2, :col3, :col4, :col5, :col6)");
					sql.Add(":col1", i);
					sql.Add(":col2", $"COL2 {Convert.ToString(i).PadLeft(3, '0')}");
					sql.Add(":col3", DateTime.Now);
					sql.Add(":col4", (i * 100));
					sql.Add(":col5", (i * 200));
					sql.Add(":col6", (i * 300));
					sql.ExecuteNonQuery();
				}
			}

			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("FINISHED!");
			Console.ReadLine();
		}
	}
}