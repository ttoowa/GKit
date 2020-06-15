using System.Reflection;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public static class AssemblyInfo {
		public static string Name {
			get {
				return Assembly.GetEntryAssembly().GetName().Name;
			}
		}
		public static string DLLName {
			get {
				return Assembly.GetExecutingAssembly().GetName().Name;
			}
		}
	}
}
