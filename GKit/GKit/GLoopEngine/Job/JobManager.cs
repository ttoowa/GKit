using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
#if UNITY
using UnityEngine;
using Debug = UnityEngine.Debug;
#endif

namespace GKit {
	public class JobManager {
		/// <summary>
		/// 한 프레임 당 작업을 실행할 수 있는 최대 경과 시간 (기본 : 4)
		/// </summary>
		public float MaxWorkMillisec {
			get {
				return maxWorkMillisec;
			}
			set {
				if (value == 0f) {
					maxWorkMillisec = 100000f;
				} else {
					maxWorkMillisec = Mathf.Max(1f, value);
				}
			}
		}
		public int JobCount {
			get {
				return jobList.Count;
			}
		}
		public float ElapsedMillisec => elapsedMillisec;

		private float maxWorkMillisec;
		private float elapsedMillisec;
		private List<Job> jobList = new List<Job>();
		private Stopwatch jobWatch = new Stopwatch();

		public JobManager() {
			MaxWorkMillisec = 0f;
		}
		public void AddJob(Action action, float delaySec = 0f) {
			Task pushTask = Task.Factory.StartNew(() => {
				lock (jobList) {
					jobList.Add(new Job(this, action, delaySec * 1000f));
				}
			});
		}

		public void ExecuteJob() {
			jobWatch.Start();

			Job job;
			for (int i = 0; i < jobList.Count; ++i) {
				lock (jobList) {
					job = jobList[i];
					if (job.Execute()) {
						jobList.RemoveAt(i);
					}
				}
				if (jobWatch.GetElapsedMilliseconds() > maxWorkMillisec) {
					break;
				}
			}
			jobWatch.Stop();
			elapsedMillisec = jobWatch.GetElapsedMilliseconds();
			jobWatch.Reset();
		}

		private class Job {
			public JobManager ownerJobManager;
			public Action action;
			public float delayMillisec;
			public Stopwatch watch;
			public float DeltaMillisec => watch.GetElapsedMilliseconds();

			public Job(JobManager ownerJobManager, Action action, float delayMillisec = 0f) {
				this.ownerJobManager = ownerJobManager;
				this.action = action;
				this.delayMillisec = delayMillisec;
				this.watch = new Stopwatch();
				if (delayMillisec > 0f) {
					watch.Start();
				}
			}
			public bool Execute() {
				if (delayMillisec <= 0f) {
					try {
						action();
					} catch (Exception ex) {
						("Job 에서 예외 발생 : " + Environment.NewLine + ex.ToString()).Log();
					}

					watch.Stop();
					watch = null;
					return true;
				} else {
					delayMillisec -= DeltaMillisec;
					watch.Restart();
					return false;
				}
			}
		}
	}
}