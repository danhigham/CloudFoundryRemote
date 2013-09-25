using System;
using System.Linq;
using SQLite;
using CloudFoundryRemote.Helpers;

namespace CloudFoundryRemote.Data.Models
{
	public class Connection
	{
		[PrimaryKey, AutoIncrement]
		public int Id { get; set; }

		public string Endpoint { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public bool TrustAll { get; set; }

		public static Connection getAllAs(int id)
		{
			var db = DataHelper.GetConnection();
			var circuit = db.Table<Connection> ().Where (c => c.Id == id).FirstOrDefault();

			db.Close ();
			db.Dispose ();

			return circuit;
		}

		public static int Count() 
		{
			var db = DataHelper.GetConnection();
			var count = db.Table<Connection> ().Count();

			db.Close ();
			db.Dispose ();

			return count;
		}

		public static Connection First() 
		{
			var db = DataHelper.GetConnection();
			var connection = db.Table<Connection> ().First();

			db.Close ();
			db.Dispose ();

			return connection;
		}

		public static void CreateOrUpdateConnection(string endpoint, string username, string password, bool trustAll) 
		{
			var db = DataHelper.GetConnection();
			var connection = db.Table<Connection> ().Where (c => c.Username == username && c.Endpoint == endpoint).FirstOrDefault();

			if (connection == null) {
				db.Insert (new Connection () { Username = username, Endpoint = endpoint, Password = password, TrustAll = trustAll });
			} else {
				connection.Password = password;
				connection.TrustAll = trustAll;
				db.Update (connection);
			}

			db.Close ();
			db.Dispose ();
		}

		public static ConnectionPickerViewModel ConnectionsForPicker()
		{
			var db = DataHelper.GetConnection();

			var connections = db.Table<Connection> ().ToList ();

			db.Close ();
			db.Dispose ();

			return new ConnectionPickerViewModel (connections);
		}
	}
}

