using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GKit.MultiThread;

namespace GKit {
	public struct FuncProfileResult {
		public Action action;
		public float elapsedMillisec;

		internal FuncProfileResult(Action action, float elapsedMillisec) {
			this.action = action;
			this.elapsedMillisec = elapsedMillisec;
		}
	}
	public struct FuncProfileResults {
		public FuncProfileResult[] profileInfos;

		internal FuncProfileResults(FuncProfileResult[] profileInfos) {
			this.profileInfos = profileInfos;
		}
		public override string ToString() {
			const string MillisecFormat = "0.0000ms";

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < profileInfos.Length; ++i) {
				FuncProfileResult info = profileInfos[i];
				builder.Append("Func");
				builder.Append(i);
				builder.Append(" (");
				if (info.action != null && info.action.Method != null) {
					builder.Append(info.action.Method.Name);
				} else {
					builder.Append("null");
				}
				builder.Append(")");
				builder.Append(" Elapsed : ");
				builder.AppendLine(profileInfos[i].elapsedMillisec.ToString(MillisecFormat));
			}
			return builder.ToString();
		}
	}
	public static class GProfiler {
		public static FuncProfileResults ProfileFunction(int repeatCount, bool useMultithread, params Action[] actions) {
			FuncProfileResult[] results = new FuncProfileResult[actions.Length];

			LoopDelegate profileAction = (int startI, int endI) => {
				for (int actionI = startI; actionI < endI; ++actionI) {
					Stopwatch watch = new Stopwatch();
					watch.Restart();

					Action action = actions[actionI];
					if (action != null) {
						for (int repeatI = 0; repeatI < repeatCount; ++repeatI) {
							action();
						}
					}

					watch.Stop();
					results[actionI] = new FuncProfileResult(action, (float)watch.GetElapsedMilliseconds());
				}
			};
			if (useMultithread) {
				ParallelLoop pLoop = new ParallelLoop(profileAction, 0, actions.Length, ParallelPriolity.High);
				pLoop.RunWait();
			} else {
				profileAction(0, actions.Length);
			}

			return new FuncProfileResults(results);
		}
	}
}
