using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Interfaces
{
	/// <summary>
	/// Interface ISQLite
	/// </summary>
	public interface ISQLite
	{
		/// <summary>
		/// Gets the connection.
		/// </summary>
		/// <returns>SQLiteConnection.</returns>
		SQLiteConnection GetConnection();

		/// <summary>
		/// Gets the asynchronous connection.
		/// </summary>
		/// <returns>SQLiteAsyncConnection.</returns>
		SQLiteAsyncConnection GetAsyncConnection();
	}
}
