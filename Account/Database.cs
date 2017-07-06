using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading;
using System.Linq;
using MySql.Data.MySqlClient;
using log4net;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data.SqlClient;

namespace AuthME
{
	public class xConnection : IDisposable
	{
		public readonly MySqlConnection Connection;

		public xConnection()
		{
			Connection = new MySqlConnection("server=127.0.0.1;" +
				"database=cristalix;uid=root;" +
				"pwd=naruto4815;" +
				"Pooling=true;" +
				"Min Pool Size=10;" +
				"Max Pool Size=100;");
		}

		public void Dispose()
		{
			Connection.Close();
		}
	}

	public class Database
	{
		static ILog Log = LogManager.GetLogger(typeof(Database));

		private MySqlConnection mysql_connect;

		public static string Connect;

		private Timer PingTimer;

		private Account Auth;

		public Database(Account a)
		{
			Connect = "server=127.0.0.1;" +
				"database=cristalix;uid=root;" +
				"pwd=8904525;" +
				"Pooling=true;" +
				"Min Pool Size=10;" +
				"Max Pool Size=50;";// +
									//"Connection Lifetime=0";
			Auth = a;
		}

		public void Open()
		{
			try
			{
				using (var db = new xConnection())
				{
					db.Connection.Open();
					Log.Info("Connection Open ! ");
				}
			}
			catch (Exception ex)
			{
				Log.Info("Can not open connection ! ");
			}
		}

		public void CloseAsync()
		{
			Log.Info("Connection Close ! ");
		}

		public void Insert(string query)
		{
			using (var db = new xConnection())
			{
				db.Connection.Open();
				using (var cmd = db.Connection.CreateCommand())
				{
					cmd.CommandText = @query;
					cmd.ExecuteNonQuery();
				}
			}
		}

		public async void InsertAsync(string query)
		{
			using (var db = new xConnection())
			{
				await db.Connection.OpenAsync();
				using (var cmd = db.Connection.CreateCommand())
				{
					cmd.CommandText = @query;
					await cmd.ExecuteNonQueryAsync();
				}
			}
		}

		public void InsertThreadPool(string query)
		{
			Task.Run(() =>
			{
				using (var db = new xConnection())
				{
					db.Connection.Open();
					using (var cmd = db.Connection.CreateCommand())
					{
						cmd.CommandText = @query;
						cmd.ExecuteNonQuery();
					}
				}
			});
		}

		/*public List<object[]> ExecuteQuery(string query)
        {
            List<object[]> rows = new List<object[]>();
            try
            {
                MySqlCommand cmd = new MySqlCommand(query, mysql_connect);

                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    object[] row = new object[reader.FieldCount];
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        row[i] = reader[i];
                    }
                    rows.Add(row);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.ToString());
            }
            return rows;            
        }

		//Select statement
		public List< string >[] Select(string query)
		{

			//Create a list to store the result

			MySqlCommand cmd = new MySqlCommand(query, mysql_connect);
			MySqlDataReader dataReader = cmd.ExecuteReader();
			List< string >[] rows = new List< string >[dataReader.FieldCount];
			while (dataReader.Read())
			{
				for (int i = 0; i < dataReader.FieldCount; i++)
				{
					rows[i].Add(dataReader[i] + "");

				}
			}

			dataReader.Close();
			return rows;
		}*/

		public DataTable MultipleQuery(string query, string nametable, bool isQuery = true)
		{
			using (var db = new xConnection())
			{
				db.Connection.Open();
				if (isQuery)
				{
					using (var cmd = db.Connection.CreateCommand())
					{
						cmd.CommandText = query;
						try
						{
							using (MySqlDataAdapter data = new MySqlDataAdapter(cmd))
							{
								DataSet ds = new DataSet();
								data.Fill(ds, nametable);
								DataTable dt = ds.Tables[nametable];
								return dt;
							}
							return null;
						}
						catch (MySqlException e)
						{
							Log.Error("MYSQL Error select:" + e.ToString());
							return null;
						}
					}
				}
				else {
					try
					{
						using (var cmd = db.Connection.CreateCommand())
						{
							cmd.CommandText = @query;
							cmd.ExecuteNonQuery();
						}
					}
					catch (MySqlException e)
					{
						Log.Error("MYSQL Error:" + e.ToString());
					}
					return null;
				}
			}
		}

		public async Task<DataTable> MultipleQueryAsync(string query, string nametable, bool isQuery = true)
		{
			using (var db = new xConnection())
			{
				await db.Connection.OpenAsync();
				if (isQuery)
				{
					using (var cmd = db.Connection.CreateCommand())
					{
						cmd.CommandText = @query;

						try
						{
							using (MySqlDataAdapter data = new MySqlDataAdapter(cmd))
							{
								DataSet ds = new DataSet();
								data.Fill(ds, nametable);
								DataTable dt = ds.Tables[nametable];
								return dt;
							}
							return null;
						}
						catch (MySqlException e)
						{
							Log.Error("MYSQL Error select:" + e.ToString());
							return null;
						}
					}
				}
				else {
					try
					{
						using (var cmd = db.Connection.CreateCommand())
						{
							cmd.CommandText = @query;
							await cmd.ExecuteNonQueryAsync();
						}
					}
					catch (MySqlException e)
					{
						Log.Error("MYSQL Error:" + e.ToString());
					}
					return null;
				}
			}
		}

		public DataTable Query(string query, bool isQuery = true)
		{
			using (var db = new xConnection())
			{
				db.Connection.Open();
				if (isQuery)
				{
					DataTable dt = new DataTable();
					using (var cmd = db.Connection.CreateCommand())
					{
						cmd.CommandText = @query;
						try
						{
							using (DbDataReader dr = cmd.ExecuteReader())
							{
								if (dr.HasRows)
								{
									dt.Load(dr);
									return dt;
								}
								dr.Close();
							}
							return null;
						}
						catch (MySqlException e)
						{
							Log.Error("MYSQL Error select:" + e.ToString());
							return null;
						}
					}
				}
				else {
					try
					{
						using (var cmd = db.Connection.CreateCommand())
						{
							cmd.CommandText = @query;
							cmd.ExecuteNonQuery();
						}
					}
					catch (MySqlException e)
					{
						Log.Error("MYSQL Error:" + e.ToString());
					}
					return null;
				}
			}
		}

		public async Task<DataTable> QueryAsync(string query, bool isQuery = true)
		{
			using (var db = new xConnection())
			{
				await db.Connection.OpenAsync();
				if (isQuery)
				{
					using (var cmd = db.Connection.CreateCommand())
					{
						cmd.CommandText = @query;
						try
						{
							using (DbDataReader dr = await cmd.ExecuteReaderAsync())
							{
								if (dr.HasRows)
								{
									DataTable dt = new DataTable();
									dt.Load(dr);
									return dt;
								}
							}
							return null;
						}
						catch (MySqlException e)
						{
							Log.Error("MYSQL Error query:" + e.ToString());
							return null;
						}
					}
				}
				else {
					try
					{
						using (var cmd = db.Connection.CreateCommand())
						{
							cmd.CommandText = @query;
							await cmd.ExecuteNonQueryAsync();
						}
						return null;
					}
					catch (MySqlException e)
					{
						Log.Error("MYSQL Error:" + e.ToString());
						return null;
					}
				}
			}
		}

		public void Update(string query)
		{
			Task.Run(() =>
			{
				using (var db = new xConnection())
				{
					db.Connection.Open();
					using (var cmd = db.Connection.CreateCommand())
					{
						cmd.CommandText = @query;
						cmd.ExecuteNonQuery();
					}
				}
			});
		}

		public void UpdateNoQueue(string query)
		{
			using (var db = new xConnection())
			{
				db.Connection.Open();
				using (var cmd = db.Connection.CreateCommand())
				{
					cmd.CommandText = @query;
					cmd.ExecuteNonQuery();
				}
			}
		}

		public async Task UpdateAsync(string query)
		{
			using (var db = new xConnection())
			{
				await db.Connection.OpenAsync();
				using (var cmd = db.Connection.CreateCommand())
				{
					cmd.CommandText = @query;
					await cmd.ExecuteNonQueryAsync();
				}
			}
		}

		public static int UnixTime()
		{

			int unixtime = Convert.ToInt32((DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds);

			return unixtime;
		}
	}
}

