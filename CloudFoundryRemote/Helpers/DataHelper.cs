using System;
using System.IO;
using SQLite;
using CloudFoundryRemote.Data.Models;

namespace CloudFoundryRemote.Helpers
{
	public static class DataHelper
	{
		public static SQLiteConnection GetConnection() 
		{
			var documents = Environment.GetFolderPath (
				Environment.SpecialFolder.Personal);

			string db = Path.Combine (documents, "connections.db");

			if (!File.Exists (db)) {
				File.Copy ("Data/connections.db", db);
				var conn = new SQLiteConnection (db); 
				MigrateDB (conn);
				conn.Close ();
				conn.Dispose ();
			}

			return new SQLiteConnection(db);
		}

		public static void MigrateDB(SQLiteConnection connection) 
		{
			// Drop tables
			connection.DropTable<Connection> ();

			// Create tables
			connection.CreateTable<Connection> ();
		}
	}
}

