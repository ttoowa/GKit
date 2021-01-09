using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
#if OnUnity
using Debug = UnityEngine.Debug;
using UnityEngine;
using GKitForUnity.Core.Component;
#elif OnWPF
using GKitForWPF.Core.Component;
#else
using GKit.Core.Component;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	/// <summary>
	/// 일정시간 간격으로 함수를 반복호출하는 클래스입니다.
	/// </summary>
	public class GLoopEngine
#if OnUnity
	: MonoBehaviour
#else
	: IDisposable
#endif
	{
		public static GLoopEngine MainEngine {
			get; internal set;
		}
		public bool IsRunning {
			get; private set;
		}
#if OnUnity
		public int FPS {
			get {
				return Application.targetFrameRate;
			}
			set {
				Application.targetFrameRate = value;
			}
		}
#else
		public int FPS {
			get {
				return fps;
			}
			set {
				fps = value;
			}
		}
#endif
		public float LoopElapsedMilliseconds => singleLoopMs;
		public float LoopElapsedSeconds => LoopElapsedMilliseconds * 0.001f;
		public float DeltaMilliseconds => sleepMs + globalLoopMs;
		public float DeltaSeconds => DeltaMilliseconds * 0.001f;
		public int CurrentFrame => currentFrame;
		public int MaxOverlapFrame { get; set; } = 1;

		//TaskList
		private GLoopGroup[] GLoopGroups = new GLoopGroup[(int)GWhen.NUM];
		private List<GRoutine> GRoutineList = new List<GRoutine>();
		//Write
		private Queue<GLoopAction> loopActionAddQueue = new Queue<GLoopAction>();
		private Queue<GLoopAction> loopActionRemoveQueue = new Queue<GLoopAction>();
		private Queue<GRoutine> GRoutineAddQueue = new Queue<GRoutine>();
		private Queue<GRoutine> GRoutineRemoveQueue = new Queue<GRoutine>();
		private object loopActionWriteLock = new object();
		private object GRoutineWriteLock = new object();
		//Time
		private Stopwatch globalLoopWatch = new Stopwatch();
		private Stopwatch singleLoopWatch = new Stopwatch();
		private Stopwatch sleepWatch = new Stopwatch();
		internal int currentFrame;
		private float singleLoopMs;
		private float globalLoopMs;
		private float sleepMs;
		//InvokerParam
		private float tickMs;
		private float overTime;
		//Job
		private GJobManager jobManager;


#if OnUnity
		public GLoopSyncMode syncMode;
		public bool registerInput = true;
		public bool autoStart = true;
#else
		public Task MainLoopTask {
			get; private set;
		}
		private int fps = 60;

		public GLoopEngine(int FPS = 60, bool registInput = true) {
			this.fps = FPS;

			Init(registInput);
		}
		public void Dispose() {
			StopLoop();
		}
		private void Init(bool registInput) {
			jobManager = new GJobManager();
			AddLoopAction(jobManager.ExecuteJob);

			for (int i = 0; i < GLoopGroups.Length; i++) {
				GLoopGroups[i] = new GLoopGroup(this, (GWhen)i);
			}
			if (registInput) {
				RegistInput();
			}
		}
#endif

		//Add tasks
#if OnUnity
		private void Awake() {
			Init();
		}
		private void Start() {
			if (registerInput) {
				RegistInput();
			}
			if (autoStart) {
				StartLoop();
			}
		}
		private void Init() {
			jobManager = new GJobManager();
			AddLoopAction(jobManager.ExecuteJob);

			for (int i = 0; i < GLoopGroups.Length; i++) {
				GLoopGroups[i] = new GLoopGroup(this, (GWhen)i);
			}

		}
#endif
		public GRoutine AddGRoutine(IEnumerator routine) {
			//메인 스레드에서 같은 락이 두 번 걸릴 수 있어 이렇게 처리했다.
			//아래 함수들에서도 같은 이유.
			GRoutine coroutine = new GRoutine(this, routine);
			Task pushTask = Task.Factory.StartNew(() => {
				lock (GRoutineWriteLock) {
					GRoutineAddQueue.Enqueue(coroutine);
				}
			});
			pushTask.Wait();
			return coroutine;
		}
		public void RemoveGRoutine(GRoutine routince) {
			Task pushTask = Task.Factory.StartNew(() => {
				lock (GRoutineWriteLock) {
					GRoutineRemoveQueue.Enqueue(routince);
				}
			});
			pushTask.Wait();
		}
		public GLoopAction AddLoopAction(Action action, GLoopCycle cycle = GLoopCycle.EveryFrame, GWhen removeCondition = GWhen.None) {
			return AddLoopAction(action, (int)cycle, removeCondition);
		}
		public GLoopAction AddLoopAction(Action action, int cycleDelay, GWhen removeCondition = GWhen.None) {
			GLoopAction task = new GLoopAction(this, action, cycleDelay, removeCondition);
			Task pushTask = Task.Factory.StartNew(() => {
				lock (loopActionWriteLock) {
					loopActionAddQueue.Enqueue(task);
				}
			});
			pushTask.Wait();
			return task;
		}
		public void RemoveLoopAction(GLoopAction task) {
			Task pushTask = Task.Factory.StartNew(() => {
				lock (loopActionWriteLock) {
					loopActionRemoveQueue.Enqueue(task);
				}
			});
			pushTask.Wait();
		}

		public void AddJob(Action action, float delaySec = 0f) {
			jobManager.AddJob(action, delaySec);
		}

		//Start
#if OnUnity
		public void StartLoop() {
			if (IsRunning)
				return;

			IsRunning = true;
		}
		public void StopLoop(bool clearTask = true) {
			if (!IsRunning)
				return;

			IsRunning = false;

			if (clearTask) {
				ClearTask();
				loopActionAddQueue.Clear();
				loopActionRemoveQueue.Clear();
				GRoutineAddQueue.Clear();
				GRoutineRemoveQueue.Clear();
			}
		}
#else
		public void StartLoop() {
			if (IsRunning)
				return;

			IsRunning = true;
			MainLoopTask = LoopInvokeRoutine();
		}
		public async void StopLoop(bool clearTask = false) {
			if (!IsRunning)
				return;

			IsRunning = false;

			if (MainLoopTask != null) {
				await MainLoopTask;
			}
			if (clearTask) {
				ClearTask();
				loopActionAddQueue.Clear();
				loopActionRemoveQueue.Clear();
				GRoutineAddQueue.Clear();
				GRoutineRemoveQueue.Clear();
			}
		}
#endif

		//Update
		public void UpdateLoopManual() {
			UpdateLoop();
		}
#if OnUnity
		private void Update() {
			if (syncMode != GLoopSyncMode.Update)
				return;

			LoopInvoke(false);
		}
		private void FixedUpdate() {
			if (syncMode != GLoopSyncMode.FixedUpdate)
				return;

			LoopInvoke(false);
		}
		private void LoopInvoke(bool useAutoSync = true) {
			sleepWatch.Stop();
			sleepMs = sleepWatch.GetElapsedMilliseconds();

			UpdateTimeInfo();

#if OnUnity
			int loopCount = 1;
#else
			int loopCount = GetLoopCount(overTime, globalLoopWatch.GetElapsedMilliseconds(), sleepMs);
#endif
			ExecLoops(loopCount);

			sleepWatch.Restart();
		}
#else
		private async Task LoopInvokeRoutine() {
			for (; ; )
			{
				UpdateTimeInfo();
				int loopCount = GetLoopCount(overTime, globalLoopWatch.GetElapsedMilliseconds(), sleepMs);
				ExecLoops(loopCount);

				sleepWatch.Restart();
				await Task.Delay((int)Mathf.Max(1f, tickMs - overTime));
				sleepWatch.Stop();
				sleepMs = sleepWatch.GetElapsedMilliseconds();
			}
		}
#endif
		private void UpdateTimeInfo() {
#if OnUnity
			tickMs = 1000f / Application.targetFrameRate;
#else
			fps = GMath.Clamp(fps, 1, 1000);
			tickMs = Mathf.Max(0.001f, 1000f / fps);
#endif
		}
#if !OnUnity
		private int GetLoopCount(float leftOverTime, float globalLoopMs, float sleepMs) {
			overTime = Mathf.Max(0f, leftOverTime + globalLoopMs + sleepMs - tickMs);
			int loopCount;
			if (overTime > tickMs) {
				loopCount = Mathf.Min(1 + (int)(overTime / tickMs), MaxOverlapFrame);
				overTime -= (loopCount - 1) * tickMs;
			} else {
				loopCount = 1;
			}
			return loopCount;
		}
#endif
		private void ExecLoops(int loopCount) {
			globalLoopWatch.Restart();
			if (IsRunning) {
				for (int i = 0; i < loopCount; ++i) {
					UpdateLoop();
				}
			}
			globalLoopWatch.Stop();
			globalLoopMs = globalLoopWatch.GetElapsedMilliseconds();
		}

		private void UpdateLoop() {
			if (!IsRunning) {
				return;
			}
			singleLoopWatch.Restart();

			++currentFrame;

			Execute:
			bool hasNewTask = false;

			RunLoopAction();
			RunGRoutine();
			ExecWriteTask();

			void RunLoopAction() {
				int arrayLength = GLoopGroups.Length;
				for (int arrayIndex = 0; arrayIndex < arrayLength; ++arrayIndex) {
					GLoopGroup taskGroup = GetLoopGroup((GWhen)arrayIndex);

					switch (taskGroup.RemoveCondition) {
						case GWhen.MouseUpRemove:
							if (MouseInput.Left.IsUp) {
								goto InvokeAndClear;
							}
							break;

						InvokeAndClear:
							taskGroup.RunImmediately();
							taskGroup.Clear();
							continue;
					}

					taskGroup.RunWithTimer();
					//Sync Frame
					taskGroup.SyncFrame();
				}
			}
			void RunGRoutine() {
				int routineCount = GRoutineList.Count;
				for (int i = 0; i < routineCount; ++i) {
					GRoutine routine = GRoutineList[i];
					if (currentFrame == routine.currentFrame)
						continue;

					bool result = routine.Run(DeltaMilliseconds);
					//Sync Frame
					routine.currentFrame = currentFrame;

					if (!result) {
						RemoveGRoutine(routine);
					}
				}
			}
			void ExecWriteTask() {

				int loopCount;
				lock (GRoutineWriteLock) {
					hasNewTask = GRoutineAddQueue.Count > 0;

					loopCount = GRoutineRemoveQueue.Count;
					for (int i = 0; i < loopCount; ++i) {
						GRoutine routine = GRoutineRemoveQueue.Dequeue();
						GRoutineList.Remove(routine);
					}
					loopCount = GRoutineAddQueue.Count;
					for (int i = 0; i < loopCount; ++i) {
						GRoutine routine = GRoutineAddQueue.Dequeue();
						routine.currentFrame = currentFrame - 1;
						GRoutineList.Add(routine);
					}
				}
				lock (loopActionWriteLock) {
					hasNewTask = hasNewTask || (loopActionAddQueue.Count > 0);

					loopCount = loopActionRemoveQueue.Count;
					for (int i = 0; i < loopCount; ++i) {
						GLoopAction task = loopActionRemoveQueue.Dequeue();
						GetLoopGroup(task.TaskEvent).TaskList.Remove(task);
					}
					loopCount = loopActionAddQueue.Count;
					for (int i = 0; i < loopCount; ++i) {
						GLoopAction task = loopActionAddQueue.Dequeue();
						task.currentFrame = currentFrame - 1;
						GetLoopGroup(task.TaskEvent).TaskList.Add(task);
					}
				}
			}

			if (hasNewTask) {
				goto Execute;
			}

			singleLoopWatch.Stop();

			singleLoopMs = singleLoopWatch.GetElapsedMilliseconds();
		}

		private void RegistInput() {
			KeyInput.SetCore(this);
			AddLoopAction(MouseInput.Update);
#if OnUnity
			AddLoopAction(InputManager.Update);
#endif
		}

		private GLoopGroup GetLoopGroup(GWhen taskEvent) {
			return GLoopGroups[(int)taskEvent];
		}
		private void ClearTask() {
			for (int i = 0; i < GLoopGroups.Length; i++) {
				GLoopGroups[i].Clear();
			}
			GRoutineList.Clear();
		}
	}
}