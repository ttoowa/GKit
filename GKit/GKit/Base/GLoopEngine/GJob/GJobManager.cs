using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
#if OnUnity
using UnityEngine;
using Debug = UnityEngine.Debug;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    public class GJobManager {
        /// <summary>
        ///     한 프레임 당 작업을 실행할 수 있는 최대 경과 시간 (기본 : 4)
        /// </summary>
        public float MaxWorkMillisec {
            get => maxWorkMillisec;
            set {
                if (value == 0f) {
                    maxWorkMillisec = 100000f;
                } else {
                    maxWorkMillisec = Mathf.Max(1f, value);
                }
            }
        }

        public int JobCount => jobList.Count;

        public float ElapsedMillisec { get; private set; }
        private readonly List<GJob> jobList = new();
        private readonly Stopwatch jobWatch = new();

        private float maxWorkMillisec;

        public GJobManager() {
            MaxWorkMillisec = 0f;
        }

        public void AddJob(Action action, float delaySec = 0f) {
            Task pushTask = Task.Factory.StartNew(() => {
                lock (jobList) {
                    jobList.Add(new GJob(this, action, delaySec * 1000f));
                }
            });
        }

        public void ExecuteJob() {
            jobWatch.Start();

            GJob job;
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
            ElapsedMillisec = jobWatch.GetElapsedMilliseconds();
            jobWatch.Reset();
        }

        private class GJob {
            public float DeltaMillisec => watch.GetElapsedMilliseconds();
            public readonly Action action;
            public GJobManager ownerJobManager;
            public float delayMillisec;
            public Stopwatch watch;

            public GJob(GJobManager ownerJobManager, Action action, float delayMillisec = 0f) {
                this.ownerJobManager = ownerJobManager;
                this.action = action;
                this.delayMillisec = delayMillisec;
                watch = new Stopwatch();
                if (delayMillisec > 0f) {
                    watch.Start();
                }
            }

            public bool Execute() {
                if (delayMillisec <= 0f) {
                    try {
                        action();
                    } catch (Exception ex) {
                        GDebug.Log($"{nameof(GJobManager)} ::{Environment.NewLine}{ex.ToString()}");
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