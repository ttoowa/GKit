using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if UNITY
using UnityEngine;
#endif

namespace GKit.MultiThread {
	/// <param name="startIndex">for문을 시작하는 인덱스로 사용하세요.</param>
	/// <param name="endIndex">for문을 끝내는 조건의 인덱스로 사용하세요.</param>
	public delegate void LoopDelegate(int startIndex, int endIndex);

	/// <summary>
	/// for문을 병렬로 실행해 주는 클래스입니다. Run()으로 실행하고 Wait()으로 대기하십시오.
	/// </summary>
	public class ParallelLoop {
		public static int
			ThreadCount_High,
			ThreadCount_Normal,
			ThreadCount_Low;

		public int ThreadCount {
			get; private set;
		}
		public int StartIndex {
			get; private set;
		}
		public int EndIndex {
			get; private set;
		}
		private LoopDelegate func;
		private SingleTask[] singleTasks;
		private Task[] taskArray;
		
		static ParallelLoop() {
			int coreCount = Environment.ProcessorCount;

			if (coreCount >= 8) {
				ThreadCount_Low = coreCount / 2;
				ThreadCount_Normal = coreCount - 4;
				ThreadCount_High = coreCount - 2;
			} else if (coreCount >= 6) {
				ThreadCount_Low = coreCount - 4;
				ThreadCount_Normal = coreCount - 3;
				ThreadCount_High = coreCount - 1;
			} else if (coreCount >= 4) {
				ThreadCount_Low = coreCount - 3;
				ThreadCount_Normal = coreCount - 2;
				ThreadCount_High = coreCount - 1;
			} else {
				ThreadCount_Low = 1;
				ThreadCount_Normal = 1;
				ThreadCount_High = coreCount;
			}
		}
		public ParallelLoop(LoopDelegate parallelFunction, int startIndex, int endIndex, ParallelPriolity priolity) {
			this.func = parallelFunction;
			this.StartIndex = startIndex;
			this.EndIndex = endIndex;
			int coreCount = Environment.ProcessorCount;

			//코어16 : Low4, Normal8, High14
			//코어8 : Low2 Normal4 High6
			//코어6 : Low2 Normal3 High5
			//코어4 : Low1 Normal2 High3
			//코어2 : Low1 Normal1 High2
			switch (priolity) {
				case ParallelPriolity.Low:
					ThreadCount = ThreadCount_Low;
					break;
				case ParallelPriolity.Normal:
					ThreadCount = ThreadCount_Normal;
					break;
				case ParallelPriolity.High:
					ThreadCount = ThreadCount_High;
					break;
			}

			Init();
		}
		/// <summary>
		/// 스레드를 분할해 함수를 병렬로 실행합니다.
		/// </summary>
		/// <param name="parallelFunction">void (int startIndex, int endIndex) 형식의 병렬 함수</param>
		/// <param name="loopCount">총 루프 횟수</param>
		/// <param name="threadCountFactor">코어 개수에 곱한 만큼의 스레드 생성</param>
		public ParallelLoop(LoopDelegate parallelFunction, int startIndex, int endIndex, float threadCountFactor) : this(parallelFunction, startIndex, endIndex, (int)(Environment.ProcessorCount * threadCountFactor)) {

		}
		/// <summary>
		/// 스레드를 분할해 함수를 병렬로 실행합니다.
		/// </summary>
		/// <param name="parallelFunction">void (int startIndex, int endIndex) 형식의 병렬 함수</param>
		/// <param name="loopCount">총 루프 횟수</param>
		/// <param name="priolity">분할 갯수</param>
		public ParallelLoop(LoopDelegate parallelFunction, int startIndex, int endIndex, int threadCount) {
			this.func = parallelFunction;
			this.StartIndex = startIndex;
			this.EndIndex = endIndex;
			int coreCount = Environment.ProcessorCount;

			ThreadCount = Mathf.Clamp(threadCount, 1, coreCount);

			Init();
		}
		private void Init() {
			singleTasks = new SingleTask[ThreadCount];
			for(int i=0; i<ThreadCount; i++) {
				singleTasks[i] = new SingleTask(i, this, func);
			}
		}
		public void Run() {
			Queue<Task> taskQueue = new Queue<Task>();
			for (int i = 0; i < ThreadCount; i++) {
				taskQueue.Enqueue(singleTasks[i].Run());
			}
			taskArray = taskQueue.ToArray();
		}
		public void Wait() {
			if (taskArray == null)
				return;
			Task.WaitAll(taskArray);
		}
		public void RunWait() {
			Run();
			Wait();
		}
		public static void CreateBGThread() {
			int count = Environment.ProcessorCount;
			ManualResetEvent threadSwitch = new ManualResetEvent(false);
			ManualResetEvent mainSwitch = new ManualResetEvent(false);
			int remainCount = count;
			for (int i = 0; i < count; ++i) {
				Task.Factory.StartNew(() => {
					threadSwitch.WaitOne();
					if (--remainCount <= 0) {
						mainSwitch.Set();
					};
				}, TaskCreationOptions.DenyChildAttach);
			}
			threadSwitch.Set();
			mainSwitch.WaitOne();

			threadSwitch.Dispose();
			mainSwitch.Dispose();
		}

		private class SingleTask {
			private ParallelLoop Owner;

			private LoopDelegate func;
			public int ThreadIndex {
				get; private set;
			}
			public int StartIndex {
				get; private set;
			}
			public int EndIndex {
				get; private set;
			}
			public int LoopCount {
				get {
					return Mathf.Max(0, EndIndex - StartIndex);
				}
			}
			public Task Task {
				get; private set;
			}

			public SingleTask(int threadIndex, ParallelLoop owner, LoopDelegate func) {
				this.ThreadIndex = threadIndex;
				this.Owner = owner;
				this.func = func;

				FindCheckRange();
			}

			public Task Run() {
				return Task.Factory.StartNew(TaskFunc, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Default);
			}
			public void TaskFunc() {
				func(StartIndex, EndIndex);
			}
			private void FindCheckRange() {
				int globalEndIndex = Owner.EndIndex;
				int globalLoopCount = Owner.EndIndex - Owner.StartIndex;
				int singleLoopCount = globalLoopCount / Owner.ThreadCount + 1;
				StartIndex = singleLoopCount * ThreadIndex + Owner.StartIndex;
				EndIndex = singleLoopCount * (ThreadIndex + 1) + Owner.StartIndex;

				if (StartIndex >= globalEndIndex) {
					StartIndex = EndIndex = 0;
				} else if (EndIndex > globalEndIndex) {
					EndIndex = globalEndIndex;
				}
			}

		}
	}
}
