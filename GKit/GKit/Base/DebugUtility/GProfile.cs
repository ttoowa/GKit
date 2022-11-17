using System;
using System.Diagnostics;
using System.Text;
#if OnUnity
using GKitForUnity.MultiThread;
#elif OnWPF
using GKitForWPF.MultiThread;
#else
using GKit.MultiThread;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
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

            StringBuilder builder = new();
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

    public static class GProfile {
        public static FuncProfileResults ProfileFunction(int repeatCount, bool useMultithread, params Action[] actions) {
            FuncProfileResult[] results = new FuncProfileResult[actions.Length];

            LoopDelegate profileAction = (int startI, int endI) => {
                for (int actionI = startI; actionI < endI; ++actionI) {
                    Stopwatch watch = new();
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
                ParallelLoop pLoop = new(0, actions.Length, ParallelPriolity.Full, profileAction);
                pLoop.RunWait();
            } else {
                profileAction(0, actions.Length);
            }

            return new FuncProfileResults(results);
        }
    }
}