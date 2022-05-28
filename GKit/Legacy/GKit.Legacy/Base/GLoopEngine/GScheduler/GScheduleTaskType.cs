#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Core.Scheduler {
	public enum GScheduleTaskType {
		CoreTask,
		CoreRoutine,
	}
}
