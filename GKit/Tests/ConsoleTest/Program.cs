using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;
using GKit;
using GKit.SQLite;

namespace ConsoleTest {
	public class Program {
		static void Main(string[] args) {
			SQLiteTester tester = new SQLiteTester();

			Console.ReadLine();
		}
	}
	public class SQLiteTester {
		const string TestFilename = @"X:\Temp\TestSQLite.sqlite";
		private SQLiteClient dbClient;

		public SQLiteTester() {
			//SQLiteClient.CreateDB(TestFilename);
			dbClient = new SQLiteClient();
			dbClient.Open(TestFilename);

			if(!dbClient.ExistTable("Member")) {
				GDebug.Log(dbClient.CreateTable("Member",
					new SQLiteFieldAffinity(TypeAffinity.Text, "ID"),
					new SQLiteFieldAffinity(TypeAffinity.Text, "PW")));
			}

			var resultDataSet = dbClient.ExecuteDataset("SELECT * FROM Member WHERE PW LIKE '%345%'");
			GDebug.Log("Result " + resultDataSet.Tables[0].Rows[1][0]);
			object result = dbClient.ExecuteScalar("SELECT PW FROM Member WHERE ID='isg1153'");
			if (result != null) {
				string foundPw = result as string;
				GDebug.Log(foundPw);
			} else {
				GDebug.Log(dbClient.InsertRow("Member",
						new SQLiteField("ID", DbType.String, "isg1153"),
						new SQLiteField("PW", DbType.String, "123456")));
			}

			dbClient.Close();
		}
	}

}
