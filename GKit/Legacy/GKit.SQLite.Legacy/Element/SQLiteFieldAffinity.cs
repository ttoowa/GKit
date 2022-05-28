using System.Data.SQLite;

namespace GKit.SQLite {
	public struct SQLiteFieldAffinity {
		public TypeAffinity type;
		public string columnName;

		public SQLiteFieldAffinity(TypeAffinity type, string columnName) {
			this.type = type;
			this.columnName = columnName;
		}
	}
}
