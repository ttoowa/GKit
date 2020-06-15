using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
#if OnUnity
using UnityEngine;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.MultiThread {
	/// <param name="startIndex">for문을 시작하는 인덱스로 사용하세요.</param>
	/// <param name="endIndex">for문을 끝내는 조건의 인덱스로 사용하세요.</param>
	public delegate void LoopDelegate(int startIndex, int endIndex);

	/// <summary>
	/// for문을 병렬로 실행해 주는 클래스입니다. Run()으로 실행하고 Wait()으로 대기하십시오.
	/// </summary>
	public class ParallelLoop {
		public static int
			ThreadCount_Low,
			ThreadCount_Normal,
			ThreadCount_High,
			ThreadCount_Full;

		public int ThreadCount {
			get; private set;
		}
		public int StartIndex {
			get; private set;
		}
		public int EndIndex {
			get; private set;
		}
		public int LoopRange {
			get {
				return EndIndex - StartIndex;
			}
		}
		private LoopDelegate func;
		private SingleTask[] singleTasks;
		private Task[] taskArray;
		
		static ParallelLoop() {
			int coreCount = Environment.ProcessorCount;

			ThreadCount_Low = coreCount / 8;
			ThreadCount_Normal = coreCount / 4;
			ThreadCount_High = coreCount / 2;
			ThreadCount_Full = coreCount;
		}
		public ParallelLoop(int fromInclusive, int toExclusive, ParallelPriolity priolity, LoopDelegate parallelFunction) {
			this.StartIndex = fromInclusive;
			this.EndIndex = toExclusive;
			this.func = parallelFunction;

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
				case ParallelPriolity.Full:
					ThreadCount = ThreadCount_Full;
					break;
			}

			Init();
		}
		/// <summary>
		/// 스레드를 분할해 함수를 병렬로 실행합니다.
		/// </summary>
		/// <param name="parallelFunction">void (int startIndex, int endIndex) 형식의 병렬 함수</param>
		/// <param name="LoopRange">총 루프 횟수</param>
		/// <param name="threadCountFactor">코어 개수에 곱한 만큼의 스레드 생성</param>
		public ParallelLoop(int fromInclusive, int toExclusive, float threadCountFactor, LoopDelegate parallelFunction) : this(fromInclusive, toExclusive, (int)(Environment.ProcessorCount * threadCountFactor), parallelFunction) {

		}
		/// <summary>
		/// 스레드를 분할해 함수를 병렬로 실행합니다.
		/// </summary>
		/// <param name="parallelFunction">void (int startIndex, int endIndex) 형식의 병렬 함수</param>
		/// <param name="LoopRange">총 루프 횟수</param>
		/// <param name="priolity">분할 갯수</param>
		public ParallelLoop(int fromInclusive, int toExclusive, int threadCount, LoopDelegate parallelFunction, bool clipProcessorCount = true) {
			this.func = parallelFunction;
			this.StartIndex = fromInclusive;
			this.EndIndex = toExclusive;
			int coreCount = Environment.ProcessorCount;

			ThreadCount = clipProcessorCount ? Mathf.Clamp(threadCount, 1, coreCount) : threadCount;

			Init();
		}
		private void Init() {
			if(LoopRange < ThreadCount) {
				ThreadCount = LoopRange;
			}

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
		public static void PreloadBGThread() {
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
			public int LoopRange {
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
				return Task.Factory.StartNew(TaskFunc, CancellationToken.None, TaskCreationOptions.DenyChildAttach, TaskScheduler.Current);
			}
			public void TaskFunc() {
				func(StartIndex, EndIndex);
			}
			private void FindCheckRange() {
				int globalEndIndex = Owner.EndIndex;
				int globalLoopRange = Owner.LoopRange;

				int singleLoopRange = globalLoopRange / Owner.ThreadCount;
				int remainLoopRange = globalLoopRange % Owner.ThreadCount;

				if (remainLoopRange == 0) {
					//Job의 개수가 스레드에 나누어 떨어질 때
					StartIndex = singleLoopRange * ThreadIndex + Owner.StartIndex;
					EndIndex = singleLoopRange * (ThreadIndex + 1) + Owner.StartIndex;
				} else {
					int singleLoopRangePlus = singleLoopRange + 1;
					if(ThreadIndex < remainLoopRange) {
						StartIndex = singleLoopRangePlus * ThreadIndex + Owner.StartIndex;
						EndIndex = singleLoopRangePlus * (ThreadIndex + 1) + Owner.StartIndex;
					} else {
						StartIndex = singleLoopRange * ThreadIndex + Owner.StartIndex + remainLoopRange;
						EndIndex = singleLoopRange * (ThreadIndex + 1) + Owner.StartIndex + remainLoopRange;
					}
				}// else {
				//	//Job이 스레드의 2배 초과일 때
				//	StartIndex = (singleLoopRange + 1) * ThreadIndex + Owner.StartIndex;
				//	EndIndex = (singleLoopRange + 1) * (ThreadIndex + 1) + Owner.StartIndex;

				//	if (StartIndex >= globalEndIndex) {
				//		StartIndex = EndIndex = 0;
				//	}
				//}
				EndIndex = Mathf.Min(EndIndex, globalEndIndex);
			}
		}
	}
}
