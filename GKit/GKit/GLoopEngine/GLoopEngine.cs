using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Diagnostics;
using GKit.Core.Component;
#if UNITY
using Debug = UnityEngine.Debug;
using UnityEngine;
#endif

namespace GKit {
	/// <summary>
	/// 입력 체계와 사용자 추가 작업을 실행하는 무한루프 코어 클래스입니다.
	/// </summary>
	public class GLoopEngine
#if UNITY
	: MonoBehaviour
#elif WPF
	: IDisposable
#endif
	{
		public static GLoopEngine MainEngine {
			get; internal set;
		}
		public bool IsRunning {
			get; private set;
		}
		public int FPS {
			get {
				return fps;
			}
			set {
				fps = value;
			}
		}
		public float FPSInvert {
			get {
				return 1f / fps;
			}
		}
		public float DeltaMillisec => deltaMillisec;
		public float ElapsedMillisec => loopElapsedMillisec;
		public int MaxOverlapFrame = 10;
		public float TimeScale = 1f;
		public int CurrentFrame => currentFrame;
		public event SingleDelegate<string> OnException;

		//작업 리스트
		private GLoopGroup[] GLoopGroups = new GLoopGroup[(int)GWhen.NUM];
		private List<GRoutine> GRoutineList = new List<GRoutine>();
		private Stopwatch frameWatch = new Stopwatch();
		private Stopwatch loopWatch = new Stopwatch();
		//쓰기 스택
		private Queue<GLoopAction> loopActionAddQueue = new Queue<GLoopAction>();
		private Queue<GLoopAction> loopActionRemoveQueue = new Queue<GLoopAction>();
		private Queue<GRoutine> GRoutineAddQueue = new Queue<GRoutine>();
		private Queue<GRoutine> GRoutineRemoveQueue = new Queue<GRoutine>();
		private object loopActionWriteLock = new object();
		private object GRoutineWriteLock = new object();
		//시간
		private float deltaMillisec;
		private float loopElapsedMillisec;
		private float targetDelayMillisec;
		private float remainDelayMillisec;
		Stopwatch elapsedStepWatch = new Stopwatch();
		internal int currentFrame;
		//잡매니저
		private JobManager jobManager;


#if UNITY
		[Header("시작 시 코어 가동")]
		[SerializeField]
		public bool isMainCore = true;
		[SerializeField]
		public bool autoStart = true;
		[Header("시작 시 입력 시스템 추가 여부")]
		[SerializeField]
		public bool registInput = true;
		[Header("초당 반복 횟수")]
		[SerializeField]
		private int fps = 60;
		[Header("자동 업데이트")]
		public bool autoUpdate = true;
		[Header("물리엔진과 동기화")]
		public bool syncFixedUpdate;
#endif
#if WPF
		private Task mainLoop;
		private int fps = 60;

		public GLoopEngine(int FPS = 60, bool registInput = true)
		{
			this.fps = FPS;

			Init(registInput);
		}
		public void Dispose() {
			StopLoop();
		}
		private async Task LoopInvoker()
		{
			Stopwatch stepWatch = new Stopwatch();
			for (; ; )
			{
				stepWatch.Stop();
				fps = BMath.Clamp(fps, 1, 1000);

				//지연 시간 체크
				float timeStepMillisec = Mathf.Max(0.001f, 1000f / fps);

				float elapsedFrameMillisec = stepWatch.GetElapsedMilliseconds() + this.ElapsedMillisec;
				float overTime = elapsedFrameMillisec - targetDelayMillisec;
				int overCount = (int)(overTime / timeStepMillisec);
				targetDelayMillisec = Mathf.Max(1f, timeStepMillisec - (overTime - timeStepMillisec * overCount));
				//지연 시간에 따라 반복 실행
				if (IsRunning)
				{
					int loopCount = Mathf.Min(1 + overCount, MaxOverlapFrame);
					for (int i = 0; i < loopCount; ++i)
					{
						UpdateLoop();
					}
				}

				stepWatch.Restart();
				await Task.Delay((int)targetDelayMillisec);
			}
		}
		private void Init(bool registInput)
		{
			jobManager = new JobManager();
			AddLoopAction(jobManager.ExecuteJob);

			for (int i = 0; i < GLoopGroups.Length; i++)
			{
				GLoopGroups[i] = new GLoopGroup(this, (GWhen)i);
			}
			if (registInput)
			{
				RegistInput();
			}
		}
#endif
#if UNITY
		private void Awake()
		{
			Init();
		}
		private void Start()
		{
			if (isMainCore)
			{
				mainCore = this;
			}
			if (registInput)
			{
				RegistInput();
			}
			if (autoStart)
			{
				StartLoop();
			}
		}
		private void Init()
		{
			jobManager = new JobManager();
			AddLoopAction(jobManager.ExecuteJob);

			for (int i = 0; i < GLoopGroups.Length; i++)
			{
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
		public void RemoveTask(GLoopAction task) {
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
#if UNITY
		private void FixedUpdate() {
			if (!autoUpdate)
				return;

			if (syncFixedUpdate) {
				elapsedStepWatch.Stop();

				UpdateLoop();

				elapsedStepWatch.Restart();
			} else {
				if (remainDelayMillisec > 0f) {
					remainDelayMillisec -= Time.fixedDeltaTime * 0.001f;
				}

				elapsedStepWatch.Stop();

				fps = Mathf.Clamp(fps, 1, 1000);

				//지연 시간 체크
				float timeStepMillisec = Mathf.Max(0.001f, 1000f / fps);

				float elapsedFrameMillisec = elapsedStepWatch.GetElapsedMilliseconds() + this.ElapsedMillisec;
				float overTime = elapsedFrameMillisec - targetDelayMillisec;
				int overCount = (int)(overTime / timeStepMillisec);
				targetDelayMillisec = Mathf.Max(1f, timeStepMillisec - (overTime - timeStepMillisec * overCount));
				//지연 시간에 따라 반복 실행
				if (IsRunning) {
					int loopCount = Mathf.Min(1 + overCount, MaxOverlapFrame);
					for (int i = 0; i < loopCount; ++i) {
						UpdateLoop();
					}
				}

				elapsedStepWatch.Restart();
				remainDelayMillisec += targetDelayMillisec;
			}
		}
		private IEnumerator UpdateInvoker() {
			for(; ;) {
				elapsedStepWatch.Stop();
				
				fps = Mathf.Clamp(fps, 1, 1000);

				//지연 시간 체크
				float timeStepMillisec = Mathf.Max(0.001f, 1000f / fps);

				float elapsedFrameMillisec = elapsedStepWatch.GetElapsedMilliseconds() + this.ElapsedMillisec;
				float overTime = elapsedFrameMillisec - targetDelayMillisec;
				int overCount = (int)(overTime / timeStepMillisec);
				targetDelayMillisec = Mathf.Max(1f, timeStepMillisec - (overTime - timeStepMillisec * overCount));
				//지연 시간에 따라 반복 실행
				if (IsRunning) {
					int loopCount = Mathf.Min(1 + overCount, MaxOverlapFrame);
					for (int i = 0; i < loopCount; ++i) {
						UpdateLoop();
					}
				}

				elapsedStepWatch.Restart();
				yield return new WaitForSeconds(targetDelayMillisec * 0.001f);
			}
		}
#endif
		public void UpdateLoop() {
			if (!IsRunning) {
				return;
			}
			loopWatch.Restart();
			frameWatch.Stop();
			deltaMillisec = frameWatch.GetElapsedMilliseconds();
			frameWatch.Restart();

			++currentFrame;

			try {
				Execute:

				//실행
				//Run Task
				int arrayLength = GLoopGroups.Length;
				for (int arrayIndex = 0; arrayIndex < arrayLength; ++arrayIndex) {
					GLoopGroup taskGroup = GetLoopGroup((GWhen)arrayIndex);

					switch (taskGroup.RemoveCondition) {
						case GWhen.MouseUpRemove:
							if (MouseInput.LeftUp) {
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
				//Run CoreRoutine
				int routineCount = GRoutineList.Count;
				for(int i=0; i<routineCount; ++i) {
					GRoutine routine = GRoutineList[i];
					if (currentFrame == routine.currentFrame)
						continue;

					bool result = routine.Run(deltaMillisec);
					//Sync Frame
					routine.currentFrame = currentFrame;

					if (!result) {
						RemoveGRoutine(routine);
					}
				}

				//쓰기 작업 : 제거-추가
				bool hasNewTask = false;
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
				
				if(hasNewTask) {
					goto Execute;
				}
			} catch (Exception ex) {
				ex.ToString().Log();
				OnException?.Invoke(ex.ToString());
			}

			loopWatch.Stop();

			loopElapsedMillisec = loopWatch.GetElapsedMilliseconds();
		}


#if UNITY
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
#elif WPF
		public void StartLoop() {
			if (IsRunning)
				return;

			IsRunning = true;
			mainLoop = LoopInvoker();
		}
		public async void StopLoop(bool clearTask = false) {
			if (!IsRunning)
				return;

			IsRunning = false;

			if (mainLoop != null) {
				await mainLoop;
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
		private void RegistInput() {
			KeyInput.SetCore(this);
			AddLoopAction(MouseInput.Update);
#if UNITY
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