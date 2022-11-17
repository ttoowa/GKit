#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    public enum GLoopCycle {
        None = 0,
        EveryFrame = 1,
        High = 2,
        Normal = 8,
        Low = 16,
        VeryLow = 60
    }
}