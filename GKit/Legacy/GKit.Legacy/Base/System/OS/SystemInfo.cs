using System.Linq;
using System.Net.NetworkInformation;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	public static class SystemInfo {
		public static string MacAddress {
			get {
				return NetworkInterface
				.GetAllNetworkInterfaces()
				.Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
				.Select(nic => nic.GetPhysicalAddress().ToString())
				.FirstOrDefault();
			}
		}

	}
}
