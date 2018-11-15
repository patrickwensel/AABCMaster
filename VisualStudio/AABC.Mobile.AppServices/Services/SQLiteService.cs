using AABC.Mobile.AppServices.Interfaces;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Services
{
	/// <summary>
	/// Class SQLiteService.
	/// </summary>
	/// <seealso cref="AABC.Mobile.AppServices.Interfaces.ISQLite" />
	public class SQLiteService : ISQLite
	{
		/// <summary>
		/// The database name
		/// </summary>
		const string _databaseName = "AABC.db";

		/// <summary>
		/// Gets the asynchronous connection.
		/// </summary>
		/// <returns>SQLiteAsyncConnection.</returns>
		public SQLiteAsyncConnection GetAsyncConnection()
		{
			string databasePath = PCLStorage.PortablePath.Combine(PCLStorage.FileSystem.Current.LocalStorage.Path, _databaseName);
			return new SQLiteAsyncConnection(databasePath);
		}

		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <returns>SQLiteConnection.</returns>
		public SQLiteConnection GetConnection()
		{
			string databasePath = PCLStorage.PortablePath.Combine(PCLStorage.FileSystem.Current.LocalStorage.Path, _databaseName);
			return new SQLiteConnection(databasePath);
		}
	}
}
