#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	/// <summary>
	/// Init() 초기화 함수 호출이 필요한 클래스입니다.
	/// </summary>
	public interface INeedInitialization {
		void Init();
	}
}
