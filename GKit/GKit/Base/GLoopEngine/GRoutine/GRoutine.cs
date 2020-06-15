using System;
using System.Collections;
#if OnUnity
using UnityEngine;
using GKitForUnity.Core;
#elif OnWPF
using GKitForWPF.Core;
#else
using GKit.Core;
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
	/// 여러 프레임에 걸쳐 실행하는 루틴 객체입니다. IEnumerator를 LoopCore에 추가해 얻을 수 있습니다.
	/// <para>yield return 뒤에 오는 대기 명령 클래스는 다음과 같습니다.</para>
	///<para>null //한 프레임을 대기합니다.</para>
	///<para>Routine_WaitFrame(int frame) //[frame]프레임을 대기합니다.</para>
	///<para>Routine_WaitSeconds(float seconds) //[seconds]초를 대기합니다.</para>
	/// </summary>
	public class GRoutine {
		private GLoopEngine ownerCore;
		private IEnumerator routine;
		private GRoutine delayRoutine;
		private float delayMillisec;
		private bool firstRun = true;
		private int delayFrame;
		public bool IsComplete;
		public event Action OnComplete;
		internal int currentFrame;

		internal GRoutine(GLoopEngine ownerCore, IEnumerator routine) {
			this.ownerCore = ownerCore;
			this.routine = routine;
			this.delayMillisec = 0f;
		}
		public void Stop() {
			ownerCore.RemoveGRoutine(this);
		}
		internal bool Run(float deltaMillisec) {
			try {
				if (UpdateTime(deltaMillisec)) {
					return true;
				}
				if (delayMillisec <= 0f) {
					bool hasValue = routine.MoveNext();
					if (hasValue) {
						object returnObject = routine.Current;
						GWait waitOrder = CastWaitOrder(returnObject);
						if (waitOrder != null) {
							switch (waitOrder.unit) {
								case GTimeUnit.Frame:
									delayFrame += (int)waitOrder.time;
									break;
								case GTimeUnit.Millisecond:
									delayMillisec += waitOrder.time;
									break;
								case GTimeUnit.Second:
									delayMillisec += waitOrder.time * 1000f;
									break;
							}
						}
					} else {
						Complete();
					}
					return hasValue;
				}
				return true;
			} catch (Exception ex) {
				GDebug.Log(ex.ToString(), GLogLevel.Error);
				Complete();
				return false;
			}
		}
		private bool UpdateTime(float deltaMillisec) {
			if (delayFrame > 0) {
				--delayFrame;
				return true;
			} else if (delayRoutine != null) {
				if (delayRoutine.IsComplete) {
					delayRoutine = null;
				} else {
					return true;
				}
			} else {
				if (firstRun) {
					firstRun = false;
				} else {
					delayMillisec = Mathf.Max(0f, delayMillisec - deltaMillisec);
				}
			}
			return false;
		}
		private GWait CastWaitOrder(object returnObject) {
			GWait waitOrder = null;
			if (returnObject == null) {
				waitOrder = new GWait(GTimeUnit.Frame, 1);
			} else {
				if (returnObject is GWait) {
					waitOrder = (GWait)returnObject;

				} else if (returnObject is IEnumerator) {
					IEnumerator coroutine = (IEnumerator)returnObject;
					GRoutine targetRoutine = ownerCore.AddGRoutine(coroutine);
					delayRoutine = targetRoutine;

				} else if (routine.Current is GRoutine) {
					GRoutine targetRoutine = (GRoutine)returnObject;
					delayRoutine = targetRoutine;

				} else {
					GDebug.Log("지원되지 않는 반환 형식입니다. 'GWait', 'GRoutine', 'IEnumerator' 클래스를 반환하세요.", GLogLevel.Error);
					waitOrder = new GWait(GTimeUnit.Frame, 1);
				}
			}
			return waitOrder;
		}
		private void Complete() {
			if (IsComplete)
				return;

			IsComplete = true;
			OnComplete.TryInvoke();
		}
	}
	public static class CoreRoutineUtility {
		public static GRoutine Invoke(this IEnumerator routine, GLoopEngine core) {
			return core.AddGRoutine(routine);
		}
		//public static IEnumerator Clone(this IEnumerator source) {
		//	Type sourceType = source.GetType().UnderlyingSystemType;
		//	ConstructorInfo sourceTypeConstructor = sourceType.GetConstructors()[0];
		//	IEnumerator newInstance = sourceTypeConstructor.Invoke(null) as IEnumerator;

		//	FieldInfo[] nonPublicFields = source.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance);
		//	FieldInfo[] publicFields = source.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
		//	foreach (FieldInfo field in nonPublicFields) {
		//		var value = field.GetValue(source);
		//		field.SetValue(newInstance, value);
		//	}
		//	foreach (FieldInfo field in publicFields) {
		//		var value = field.GetValue(source);
		//		field.SetValue(newInstance, value);
		//	}
		//	return newInstance;
		//}
	}
}
