#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
    public interface IPoolable {
        public static IPoolable CreateInstance() {
            return null;
        }
    }
}