using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AABC.Mobile.AppServices.Extensions
{
	/// <summary>
	/// Class SqliteTableStructure.
	/// </summary>
	public class SqliteTableStructure
	{
		/// <summary>
		/// Gets or sets the column identifier.
		/// </summary>
		/// <value>The column identifier.</value>
		[ColumnAttribute("cid")]
		public int ColumnId { get; set; }

		/// <summary>
		/// Gets or sets the name.
		/// </summary>
		/// <value>The name.</value>
		[ColumnAttribute("name")]
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the type.
		/// </summary>
		/// <value>The type.</value>
		[ColumnAttribute("type")]
		public string Type { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [not null].
		/// </summary>
		/// <value><c>true</c> if [not null]; otherwise, <c>false</c>.</value>
		[ColumnAttribute("notnull")]
		public bool NotNull { get; set; }

		/// <summary>
		/// Gets or sets the default value.
		/// </summary>
		/// <value>The default value.</value>
		[ColumnAttribute("dflt_value")]
		public string DefaultValue { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether [primary key].
		/// </summary>
		/// <value><c>true</c> if [primary key]; otherwise, <c>false</c>.</value>
		[ColumnAttribute("pk")]
		public bool PrimaryKey { get; set; }
	}



	/// <summary>
	/// Class SqliteTableStructureExtensions.
	/// </summary>
	public static class SqliteTableStructureExtensions
	{
		/// <summary>
		/// Gets the table structure.
		/// </summary>
		/// <param name="sqliteConnection">The sqlite connection.</param>
		/// <param name="tableName">Name of the table.</param>
		/// <returns>List&lt;SqliteTableStructure&gt;.</returns>
		static public List<SqliteTableStructure> GetTableStructure(this SQLiteConnection sqliteConnection, string tableName)
		{
			try
			{
				return sqliteConnection.Query<SqliteTableStructure>($"PRAGMA table_info('{tableName}');");
			}
			catch (Exception)
			{
				return null;
			}
		}
	}

}
