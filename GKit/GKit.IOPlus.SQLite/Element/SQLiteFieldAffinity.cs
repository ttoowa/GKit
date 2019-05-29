using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;

namespace GKit.IOPlus.SQLite {
	public struct SQLiteFieldAffinity {
		public TypeAffinity type;
		public string columnName;

		public SQLiteFieldAffinity(TypeAffinity type, string columnName) {
			this.type = type;
			this.columnName = columnName;
		}
	}
}
