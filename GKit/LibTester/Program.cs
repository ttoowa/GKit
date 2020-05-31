using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GKit;
using GKit.MultiThread;

namespace LibTester {
	public class Program {
		static void Main(string[] args) {
#if WPF
			Root root = new Root();
#endif
			Console.ReadLine();
		}

	}
#if WPF
	public class Root {
		private GLoopEngine loopEngine;

		public Root() {
			loopEngine = new GLoopEngine();
			loopEngine.StartLoop();

			ParallelLoopTest();
		}
		private void TestWintab() {
			WintabInput wintab = new WintabInput(loopEngine);
			wintab.CaptureStart(WintabDN.WContextMode.System);


			Console.WriteLine("DisplaySize :	" + wintab.displaySize.ToString());
			Console.WriteLine("SysRect :	" + wintab.systemRect.ToString());
			Console.WriteLine("InputRect :	" + wintab.inputRect.ToString());
			Console.WriteLine("OutputRect :	" + wintab.outputRect.ToString());
			Console.WriteLine("NativeRect :	" + wintab.NativeRect.ToString());

			Console.WriteLine("if Return start log positions");
			Console.ReadLine();

			loopEngine.AddLoopAction(UpdateFrame);

			Console.ReadLine();
			wintab.CaptureStop();
			Console.WriteLine("Capture Stopped");
			Console.WriteLine("if Return start log positions");
			Console.ReadLine();
			wintab.CaptureStart(WintabDN.WContextMode.System);

			Console.ReadLine();
			wintab.CaptureStop();
			void UpdateFrame() {
				Console.WriteLine("Position : " + wintab.Position + " / " + wintab.Pressure);
			}
		}
		private void TestRandom() {
			Console.WriteLine(BRandom.RandomGauss(0f, 2f / Mathf.Sqrt(10)));
			Console.WriteLine(BRandom.RandomGauss(0f, 2f / Mathf.Sqrt(3)));
			Console.WriteLine(BRandom.RandomGauss(0f, 2f / Mathf.Sqrt(30)));
			Console.WriteLine(BRandom.RandomGauss(0f, 2f / Mathf.Sqrt(100)));
		}

		private struct PLoopResult {
			public int fromInclusive;
			public int toExclusive;
			public int threadID;

			public PLoopResult(int fromInclusive, int toExclusive, int threadID) {
				this.fromInclusive = fromInclusive;
				this.toExclusive = toExclusive;
				this.threadID = threadID;
			}
			public override string ToString() {
				return $"{fromInclusive}~{toExclusive} = {threadID}";
			}
		}
		private void ParallelLoopTest() {
			ConcurrentBag<PLoopResult> resultBag = new ConcurrentBag<PLoopResult>();
			ParallelLoop pLoop = new ParallelLoop(0, 180, ParallelPriolity.Full, LogThreadID);
			pLoop.RunWait();

			List<PLoopResult> resultList = resultBag.ToList().OrderBy(p => p.fromInclusive).ToList();
			for (int resultI = 0; resultI < resultList.Count; ++resultI) {
				Console.WriteLine(resultList[resultI]);
			}

			void LogThreadID(int fromInclusive, int toExclusive) {
				resultBag.Add(new PLoopResult(fromInclusive, toExclusive, Thread.CurrentThread.ManagedThreadId));
			}
		}
	}
#endif
}
