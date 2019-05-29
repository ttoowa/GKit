using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace GKit.MultiThread {
	public static class ThreadChannel {
		private static Dictionary<string, Channel> channelDict;
		private static Stack<Channel> channelPool;

		static ThreadChannel() {
			channelDict = new Dictionary<string, Channel>();
			channelPool = new Stack<Channel>();

			AppDomain currentApp = AppDomain.CurrentDomain;
			currentApp.ProcessExit += OnExit;

			//MainThread
			mainJobQueue = new Queue<Action>();
			mainSwitch = new ManualResetEvent(false);
			MainLoop();
		}
		public static void Clear(bool abort = false) {
			lock (channelDict) {
				Channel channel;
				foreach (KeyValuePair<string, Channel> channelPair in channelDict) {
					channel = channelPair.Value;
					channel.Dispose(abort);
				}
				channelDict.Clear();
			}
			lock (channelPool) {
				int count = channelPool.Count;
				for (int i = 0; i < count; ++i) {
					channelPool.Pop().Dispose(abort);
				}
			}
		}
		private static void OnExit(object sender, EventArgs e) {
			Clear();
		}

		/// <summary>
		/// 해당 스레드 채널에서 작업을 실행합니다.
		/// </summary>
		/// <param name="job">실행할 작업</param>
		/// <param name="channelName">스레드 채널명 [null : 메인 스레드]</param>
		/// <param name="canAbort">프로그램 종료 시 작업이 중단되어도 되는지의 여부</param>
		public static void RunInThread(this Action job, string channelName, bool canAbort = true) {
			if (string.IsNullOrEmpty(channelName)) {
				AddMainJob(job);
			} else {
				for (; ; ) {
					Channel channel = GetChannel(channelName);
					lock (channel) {
						if (channel.Available) {
							channel.AddJob(job, canAbort);
							break;
						} else {
							continue;
						}
					}
				}
			}
		}

		private static Channel GetChannel(string channelName) {
			Channel channel;
			//Get
			lock (channelDict) {
				if (channelDict.ContainsKey(channelName)) {
					return channelDict[channelName];
				}
				//Regist
				lock (channelPool) {
					if (channelPool.Count > 0) {
						channel = channelPool.Pop();
					} else {
						channel = new Channel(channelName);
					}
					lock (channel) {
						channel.SetAvailable(true);
					}
				}

				channel.SetName(channelName);
				channelDict.Add(channelName, channel);
			}
			return channel;
		}
		private static void ReturnChannel(Channel channel) {
			lock (channelDict) {
				channelDict.Remove(channel.Name);
			}
			lock (channelPool) {
				channelPool.Push(channel);
			}
		}

		//MainThread
		private static Queue<Action> mainJobQueue;
		private static ManualResetEvent mainSwitch;

		private static async Task MainWaitOne() {
			Task task = new Task(() => {
				mainSwitch.WaitOne();
				mainSwitch.Reset();
			});
			task.Start();
			await task;
		}
		private static async void MainLoop() {
			for (; ; ) {
				await MainWaitOne();

				int count = mainJobQueue.Count;
				if (count > 0) {
					for (int i = 0; i < count; ++i) {
						try {
							mainJobQueue.Dequeue()();
						} catch (Exception ex) {
							GDebug.Log($"{nameof(ThreadChannel)} ::{Environment.NewLine}{ex.ToString()}", GLogLevel.Warnning);
						}
					}
				}
			}
		}
		private static void AddMainJob(Action job) {
			mainJobQueue.Enqueue(job);

			mainSwitch.Set();
		}


		//Channel
		private class Channel {
			private const int TimeOutSec = 3;

			public string Name {
				get {
					return name;
				}
			}
			public bool Available {
				get {
					return available;
				}
			}
			public bool CanAbort {
				get {
					return canAbort;
				}
			}
			private string name;
			private Thread thread;
			private Queue<Action> jobQueue;
			private ManualResetEvent threadSwitch;
			private bool available;
			private bool canAbort;
			private bool disposeFlag;

			public Channel(string name) {
				SetName(name);
				jobQueue = new Queue<Action>();
				threadSwitch = new ManualResetEvent(false);
				canAbort = true;

				thread = new Thread(ThreadLoop);
				thread.Name = "ManagedChannel";
				thread.Start();
			}
			public void Dispose(bool abort) {
				disposeFlag = true;
				threadSwitch.Set();

				if (abort) {
					thread.Abort();
				}
			}
			public void AddJob(Action job, bool canAbort) {
				available = true;
				jobQueue.Enqueue(job);
				if (this.canAbort) {
					SetAbortable(canAbort);
				}
				threadSwitch.Set();
			}
			public void SetName(string name) {
				this.name = name;
			}
			public void SetAvailable(bool available) {
				this.available = available;
			}

			private void SetAbortable(bool canAbort) {
				this.canAbort = canAbort;
				thread.IsBackground = canAbort;
			}
			private void ThreadLoop() {
				Action job = null;

				for (; ; ) {
					threadSwitch.WaitOne();
					threadSwitch.Reset();
					if (disposeFlag) {
						return;
					}
					for(; ;) {
						try {
							lock (this) {
								if (jobQueue.Count > 0) {
									job = jobQueue.Dequeue();
								} else {
									//Complete
									SetAvailable(false);
									ReturnChannel(this);
									if (!canAbort) {
										SetAbortable(true);
									}
									break;
								}
							}
							job?.Invoke();
						} catch (Exception ex) {
							GDebug.Log($"ThreadChannel '{name}' :: {Environment.NewLine}{ex.ToString()}");
						}
					}
				}
			}
		}
	}
}