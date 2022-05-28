using System.Data;
using System.Data.SQLite;
using System.Text;

namespace GKit.SQLite {
	public class SQLiteClient {

		public bool IsConnected => connection != null;
		public SQLiteConnection Connection => connection;
		private SQLiteConnection connection;

		public static void CreateDB(string filename) {
			SQLiteConnection.CreateFile(filename);
		}

		public SQLiteClient() {
		}
		public bool Open(string filename) {
			if (IsConnected)
				return false;
			SQLiteConnectionStringBuilder option = new SQLiteConnectionStringBuilder();
			option.DataSource = filename;

			connection = new SQLiteConnection(option.ConnectionString);
			connection.Open();
			return true;
		}
		public bool Open(SQLiteConnectionStringBuilder connOption) {
			if (IsConnected)
				return false;

			connection = new SQLiteConnection(connOption.ConnectionString);
			connection.Open();
			return true;
		}
		public void Close() {
			if (!IsConnected)
				return;

			connection.Close();
		}

		public void ChangePassword(string newPassword) {
			connection.ChangePassword(newPassword);
		}

		public int CreateTable(string tableName, params SQLiteFieldAffinity[] fieldAffinities) {
			StringBuilder cmdBuilder = new StringBuilder();
			cmdBuilder.Append("CREATE TABLE ");
			cmdBuilder.Append(tableName);
			cmdBuilder.Append(" (");
			for (int i = 0; i < fieldAffinities.Length; ++i) {
				SQLiteFieldAffinity fieldAffinity = fieldAffinities[i];

				cmdBuilder.Append($"{fieldAffinity.columnName} {fieldAffinity.type}");
				if (i < fieldAffinities.Length - 1) {
					cmdBuilder.Append(",");
				}
			}
			cmdBuilder.Append(");");

			return ExecuteNonQuery(cmdBuilder.ToString());
		}
		public int DropTable(string tableName) {
			string cmdText = $"DROP TABLE {tableName};";
			return ExecuteNonQuery(cmdText);
		}
		public int RenameTable(string tableOldName, string tableNewName) {
			string cmdText = $"ALTER TABLE {tableOldName} RENAME TO {tableNewName};";
			return ExecuteNonQuery(cmdText);
		}
		public bool ExistTable(string tableName) {
			return ExecuteNonQuery($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'") != 0;
		}

		public int CreateColumn(string tableName, SQLiteFieldAffinity memberInfo) {
			string cmdText = $"ALTER TABLE {tableName} ADD COLUMN {memberInfo.columnName} {memberInfo.type.ToString()};";
			return ExecuteNonQuery(cmdText);
		}

		public int CreateIndex(string tableName, string indexName, string fieldName, bool setUnique = true, SQLiteSortOrder sortOrder = SQLiteSortOrder.ASC) {
			StringBuilder cmdBuilder = new StringBuilder();
			cmdBuilder.Append("CREATE ");
			if (setUnique) {
				cmdBuilder.Append("UNIQUE ");
			}
			cmdBuilder.Append($"INDEX {indexName} ON {tableName} ({fieldName} {sortOrder.ToString()});");

			return ExecuteNonQuery(cmdBuilder.ToString());
		}
		public int DropIndex(string indexName) {
			string cmdText = $"DROP INDEX {indexName};";
			return ExecuteNonQuery(cmdText);
		}

		public int InsertRow(string tableName, params SQLiteField[] fields) {
			StringBuilder cmdBuilder = new StringBuilder();
			cmdBuilder.Append($"INSERT INTO {tableName} (");
			StringBuilder valueRefBuilder = new StringBuilder();
			valueRefBuilder.Append(" VALUES (");

			SQLiteCommand cmd = new SQLiteCommand(connection);
			for (int i = 0; i < fields.Length; ++i) {
				SQLiteField field = fields[i];

				cmdBuilder.Append($"{field.columnName}");
				valueRefBuilder.Append("?");
				cmd.Parameters.Add(field.value);

				if (i < fields.Length - 1) {
					cmdBuilder.Append(",");
					valueRefBuilder.Append(",");
				}
			}
			cmdBuilder.Append(")");
			valueRefBuilder.Append(")");
			cmdBuilder.Append(valueRefBuilder.ToString());
			cmdBuilder.Append(";");

			cmd.CommandText = cmdBuilder.ToString();
			return cmd.ExecuteNonQuery();
		}
		public int UpdateRow(string tableName, string whereCondition, params SQLiteField[] fields) {
			SQLiteCommand cmd = new SQLiteCommand(connection);
			StringBuilder cmdBuilder = new StringBuilder();
			cmdBuilder.Append($"UPDATE {tableName} SET ");
			for (int i = 0; i < fields.Length; ++i) {
				SQLiteField field = fields[i];
				cmdBuilder.Append($"{field.columnName}=?");
				cmd.Parameters.Add(field.value);
			}
			if (!string.IsNullOrEmpty(whereCondition)) {
				cmdBuilder.Append($"WHERE {whereCondition}");
			}
			cmdBuilder.Append(";");

			cmd.CommandText = cmdBuilder.ToString();
			return cmd.ExecuteNonQuery();
		}
		public int DeleteRow(string tableName, string whereCondition) {
			SQLiteCommand cmd = new SQLiteCommand(connection);
			StringBuilder cmdBuilder = new StringBuilder();
			cmdBuilder.Append($"DELETE FROM {tableName}");
			if (!string.IsNullOrEmpty(whereCondition)) {
				cmdBuilder.Append($" WHERE {whereCondition}");
			}
			cmdBuilder.Append(";");

			cmd.CommandText = cmdBuilder.ToString();
			return cmd.ExecuteNonQuery();
		}

		public int ExecuteNonQuery(string cmdText) {
			SQLiteCommand cmd = new SQLiteCommand(cmdText, connection);
			return cmd.ExecuteNonQuery();
		}
		public SQLiteDataReader ExecuteReader(string cmdText) {
			SQLiteCommand cmd = new SQLiteCommand(cmdText, connection);
			SQLiteDataReader reader = cmd.ExecuteReader();

			return reader;
		}
		public DataSet ExecuteDataset(string cmdText) {
			SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmdText, connection);
			DataSet dataSet = new DataSet();
			adapter.Fill(dataSet);

			return dataSet;
		}
		public object ExecuteScalar(string cmdText) {
			SQLiteCommand cmd = new SQLiteCommand(cmdText, connection);
			return cmd.ExecuteScalar();
		}
		public T ExecuteScalar<T>(string cmdText) {
			SQLiteCommand cmd = new SQLiteCommand(cmdText, connection);
			return (T)cmd.ExecuteScalar();
		}
	}
}
