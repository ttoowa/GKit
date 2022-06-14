using System;

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

		event Action PoolDisposing;

		void PoolInit(object[] args);
		void Dispose();
	}
}