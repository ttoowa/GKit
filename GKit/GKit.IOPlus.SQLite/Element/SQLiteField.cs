using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace GKit.IOPlus.SQLite {
	public struct SQLiteField {
		public string columnName;
		public SQLiteParameter value;

		public SQLiteField(string columnName, DbType valueType, object value) {
			this.columnName = columnName;
			this.value = new SQLiteParameter(valueType, value);
		}
	}
}
