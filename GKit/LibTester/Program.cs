using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BigLibrary;
using WintabDN;

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
		private LoopCore core;

		public Root() {
			core = new LoopCore(60, true);
			core.StartLoop();
			
				TestRandom();
		}
		private void TestWintab() {
			WintabInput wintab = new WintabInput(core);
			wintab.CaptureStart(WContextMode.System);


			Console.WriteLine("DisplaySize :	" + wintab.displaySize.ToString());
			Console.WriteLine("SysRect :	" + wintab.systemRect.ToString());
			Console.WriteLine("InputRect :	" + wintab.inputRect.ToString());
			Console.WriteLine("OutputRect :	" + wintab.outputRect.ToString());
			Console.WriteLine("NativeRect :	" + wintab.NativeRect.ToString());

			Console.WriteLine("if Return start log positions");
			Console.ReadLine();

			core.AddTask(UpdateFrame);

			Console.ReadLine();
			wintab.CaptureStop();
			Console.WriteLine("Capture Stopped");
			Console.WriteLine("if Return start log positions");
			Console.ReadLine();
			wintab.CaptureStart(WContextMode.System);

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
	}
#endif
}
