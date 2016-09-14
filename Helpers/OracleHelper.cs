using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace OracleTester.Sample.Helpers
{
	public class OracleDatabase
	{
		public OracleDatabase()
		{
			OracleConnection = CreateConnection();
			OracleCommand = new OracleCommand { Connection = OracleConnection };
		}

		public OracleCommand OracleCommand { get; set; }

		public OracleConnection OracleConnection { get; set; }

		public object GetParameter(string paramName)
		{
			return OracleCommand.Parameters.Contains(paramName) ? OracleCommand.Parameters[paramName].Value : null;
		}

		private static OracleConnection CreateConnection()
		{
			return new OracleConnection(System.Configuration.ConfigurationManager.ConnectionStrings["CNNAME"].ConnectionString);
		}
	}

	public class OracleDataTypes
	{
		public static bool DbBoolean(object valor)
		{
			return Convert.ToBoolean(valor);
		}

		public static bool? DbBoolean(object valor, bool isNullable)
		{
			if (isNullable)
			{
				if (valor != null && valor != DBNull.Value)
					return Convert.ToBoolean(valor);
			}
			else
				return DbBoolean(valor);
			return null;
		}

		public static DateTime DbDateTime(object valor)
		{
			return Convert.ToDateTime(valor);
		}

		public static DateTime? DbDateTime(object valor, bool isNullable)
		{
			if (isNullable)
			{
				if (valor != null && valor != DBNull.Value)
					return Convert.ToDateTime(valor);
			}
			else
				return DbDateTime(valor);
			return null;
		}

		public static decimal DbDecimal(object valor)
		{
			return Convert.ToDecimal(valor);
		}

		public static decimal? DbDecimal(object valor, bool isNullable)
		{
			if (isNullable)
			{
				if (valor != null && valor != DBNull.Value)
					return Convert.ToDecimal(valor);
			}
			else
				return DbDecimal(valor);
			return null;
		}

		public static decimal DbDecimalRounded(object valor)
		{
			return decimal.Round(Convert.ToDecimal(valor), 1);
		}

		public static decimal? DbDecimalRounded(object valor, bool isNullable)
		{
			if (isNullable)
			{
				if (valor != null && valor != DBNull.Value)
					return decimal.Round(Convert.ToDecimal(valor), 1);
			}
			else
				return decimal.Round(DbDecimal(valor), 1);
			return null;
		}

		public static int DbInt32(object valor)
		{
			return Convert.ToInt32(valor);
		}

		public static int? DbInt32(object valor, bool isNullable)
		{
			if (isNullable)
			{
				if (valor != null && valor != DBNull.Value)
					return Convert.ToInt32(valor);
			}
			else
				return DbInt32(valor);
			return null;
		}

		public static long DbInt64(object valor)
		{
			return Convert.ToInt64(valor);
		}

		public static long? DbInt64(object valor, bool isNullable)
		{
			if (isNullable)
			{
				if (valor != null && valor != DBNull.Value)
					return Convert.ToInt64(valor);
			}
			else
				return DbInt64(valor);
			return null;
		}

		public static string DbString(object valor)
		{
			return valor.ToString().Length <= 0 ? "" : valor.ToString();
		}

		public static string DbString(object valor, bool isNullable)
		{
			if (isNullable)
			{
				if (valor != null && valor != DBNull.Value)
					return Convert.ToString(valor);
			}
			else
				return DbString(valor);
			return null;
		}
	}

	public class OracleHelper : IDisposable
	{
		private readonly OracleDatabase _oracleDatabase;
		private readonly OracleParameters _parametros;
		private readonly StringBuilder _sql;

		public OracleHelper()
		{
			_sql = new StringBuilder();
			_parametros = new OracleParameters();
			_oracleDatabase = new OracleDatabase();
		}

		public void Add(string clausula)
		{
			if (!string.IsNullOrEmpty(clausula))
				_sql.Append(clausula);
		}

		public void Add(string variavel, object valor)
		{
			_parametros.Add(variavel, valor ?? DBNull.Value);
		}

		public void Add(string variavel, object valor, OracleDbType dbtype, ParameterDirection parameterDirection)
		{
			_parametros.Add(variavel, valor ?? DBNull.Value, dbtype, parameterDirection);
		}

		public void Dispose()
		{
			if (_oracleDatabase.OracleConnection.State != ConnectionState.Closed)
				_oracleDatabase.OracleConnection.Close();

			GC.SuppressFinalize(this);
		}

		public int ExecuteNonQuery(CommandType commandType = CommandType.Text)
		{
			_oracleDatabase.OracleCommand.CommandText = _sql.ToString();
			_oracleDatabase.OracleCommand.CommandType = commandType;
			_parametros.PopulaParametros(_oracleDatabase);
			_oracleDatabase.OracleConnection.Open();
			return _oracleDatabase.OracleCommand.ExecuteNonQuery();
		}

		public OracleDataReader ExecuteReader(CommandType commandType = CommandType.Text)
		{
			_oracleDatabase.OracleCommand.CommandText = _sql.ToString();
			_oracleDatabase.OracleCommand.CommandType = commandType;
			_parametros.PopulaParametros(_oracleDatabase);
			_oracleDatabase.OracleConnection.Open();
			var reader = _oracleDatabase.OracleCommand.ExecuteReader();
			return reader;
		}

		public object ExecuteScalar(CommandType commandType = CommandType.Text)
		{
			_oracleDatabase.OracleCommand.CommandText = _sql.ToString();
			_oracleDatabase.OracleCommand.CommandType = commandType;
			_parametros.PopulaParametros(_oracleDatabase);
			_oracleDatabase.OracleConnection.Open();
			return _oracleDatabase.OracleCommand.ExecuteScalar();
		}

		public object GetParameter(string paramName)
		{
			return _oracleDatabase.GetParameter(paramName);
		}
	}

	internal class OracleParameters
	{
		private readonly List<OracleParameterValues> _paramValues;

		public OracleParameters()
		{
			_paramValues = new List<OracleParameterValues>();
		}

		public void Add(string nomeParametro, object valorParametro)
		{
			foreach (var item in _paramValues.Where(item => item.NomeParametro == nomeParametro))
			{
				item.ValorParametro = valorParametro;
				return;
			}
			_paramValues.Add(new OracleParameterValues(nomeParametro, valorParametro));
		}

		public void Add(string nomeParametro, object valorParametro, OracleDbType dbType, ParameterDirection parameterDirection)
		{
			foreach (var item in _paramValues.Where(item => item.NomeParametro == nomeParametro))
			{
				item.ValorParametro = valorParametro;
				return;
			}
			_paramValues.Add(new OracleParameterValues(nomeParametro, valorParametro, dbType, parameterDirection));
		}

		public void PopulaParametros(OracleDatabase sqlDatabase)
		{
			foreach (var item in _paramValues)
			{
				if (item.DbType.HasValue && item.ParameterDirection.HasValue)
				{
					sqlDatabase.OracleCommand.Parameters.Add(item.NomeParametro, item.DbType.Value, item.ParameterDirection.Value);
					sqlDatabase.OracleCommand.Parameters[item.NomeParametro].Value = item.ValorParametro;
				}
				else
				{
					sqlDatabase.OracleCommand.Parameters.Add(item.NomeParametro, item.ValorParametro);
				}
			}
		}
	}

	internal class OracleParameterValues
	{
		public OracleParameterValues(string nomeParametro, object valorParametro)
		{
			NomeParametro = nomeParametro;
			ValorParametro = valorParametro;
		}

		public OracleParameterValues(string nomeParametro, object valorParametro, OracleDbType? dbType, ParameterDirection? parameterDirection)
		{
			NomeParametro = nomeParametro;
			ValorParametro = valorParametro;
			DbType = dbType;
			ParameterDirection = parameterDirection;
		}

		public string NomeParametro { get; set; }
		public object ValorParametro { get; set; }
		public OracleDbType? DbType { get; set; }
		public ParameterDirection? ParameterDirection { get; set; }
	}
}