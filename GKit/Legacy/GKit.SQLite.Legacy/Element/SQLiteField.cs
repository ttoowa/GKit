using System.Data;
using System.Data.SQLite;

namespace GKit.SQLite {
	public struct SQLiteField {
		public string columnName;
		public SQLiteParameter value;

		public SQLiteField(string columnName, DbType valueType, object value) {
			this.columnName = columnName;
			this.value = new SQLiteParameter(valueType, value);
		}
	}
}
