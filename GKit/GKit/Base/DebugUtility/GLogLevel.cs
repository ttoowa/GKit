#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    public enum GLogLevel {
        Log,
        Warnning,
        Error
    }
}