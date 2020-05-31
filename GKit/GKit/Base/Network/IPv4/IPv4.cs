using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit.Network {
	public class IPv4 {
		public const string BaseN_e = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ!?/()<>{}[]~+=-_@#$%^&*.,'\"";
		public const string BaseN_k = "가거고구그기나너노누느니다더도두드디라러로루르리마머모무므미바버보부브비사서소수스시아어오우으이자저조주즈지차처초츠치카커코쿠크키타터토투트티파퍼포푸프피하허호후흐히" +
			"각걱곡국극긱간건곤군근긴감검곰굼금김갓것곳굿긋깃강겅공궁긍깅낙넉녹눅늑닉난넌논눈는닌남넘놈눔늠님낫넛놋눗늣닛낭넝농눙능닝닥덕독둑득딕단던돈둔든딘담덤돔둠듬딤닷덧돗둣듯딧당덩동둥등딩막먹목묵믁믹만먼몬문믄민맘멈몸뭄믐밈맛멋못뭇믓밋망멍몽뭉믕밍박벅복북븍빅반번본분븐빈밤범봄붐븜빔밧벗봇붓븟빗방벙봉붕븡빙삭석속숙슥식산선손순슨신삼섬솜숨슴심삿섯솟숫슷싯상성송숭승싱악억옥욱윽익안언온운은인암엄옴움음임앗엇옷웃읏잇앙엉옹웅응잉작적족죽즉직잔전존준즌진잠점좀줌즘짐잣젓좃줏즛짓장정종중증징칵컥콕쿡큭킥칸컨콘쿤큰킨캄컴콤쿰큼킴캇컷콧쿳큿킷캉컹콩쿵킁킹학헉혹훅흑힉한헌혼훈흔힌함험홈훔흠힘핫헛홋훗흣힛항헝홍훙흥힝";
		public static int BaseN_eLength {
			get; private set;
		}
		public static int BaseN_kLength {
			get; private set;
		}
		public const int IPv4Length = 12;

		public string IPAddress {
			get; private set;
		}
		public string eIPAddress {
			get; private set;
		}
		public string kIPAddress {
			get; private set;
		}
		public uint NumAddress {
			get; private set;
		}

		public string DebugText => IPAddress + " -> " + NumAddress + " -> " + kIPAddress;

		static IPv4() {
			BaseN_eLength = BaseN_e.Length;
			BaseN_kLength = BaseN_k.Length;
		}
		public IPv4(string IPAddress, IPv4Type ipType = IPv4Type.IPAddress) {
			switch (ipType) {
				default:
				case IPv4Type.IPAddress:
					this.IPAddress = IPAddress;
					eIPAddress = IP2sIP(IPAddress, IPv4Type.eIPAddress);
					kIPAddress = IP2sIP(IPAddress, IPv4Type.kIPAddress);
					break;
				case IPv4Type.eIPAddress:
					eIPAddress = IPAddress;
					IPAddress = sIP2IP(eIPAddress, ipType);
					kIPAddress = IP2sIP(IPAddress, IPv4Type.kIPAddress);
					break;
				case IPv4Type.kIPAddress:
					kIPAddress = IPAddress;
					IPAddress = sIP2IP(kIPAddress, IPv4Type.kIPAddress);
					eIPAddress = IP2sIP(IPAddress, IPv4Type.eIPAddress);
					break;
			}
			NumAddress = IP2Num(IPAddress);
		}
		public static uint IP2Num(string IP) {
			string[] blocks = IP.Split('.');
			byte[] bytes = new byte[4];
			for (int i = 0; i < bytes.Length; ++i) {
				bytes[i] = byte.Parse(blocks[i]);
			}
			return bytes.ToLocalUInt32();
		}
		public static string Num2IP(uint numAddress) {
			byte[] bytes = numAddress.ToNetBytes();
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < bytes.Length; ++i) {
				if (i != 0) {
					builder.Append('.');
				}
				builder.Append(bytes[i].ToString());
			}
			return builder.ToString();
		}

		public static string IP2sIP(string IP, IPv4Type sIPType) {

			return BMath.Base10ToBaseN(IP2Num(IP), GetBaseN(sIPType));
		}
		public static string sIP2IP(string sIP, IPv4Type sIPType) {
			uint numAddress = (uint)BMath.BaseNToBase10(sIP, GetBaseN(sIPType));
			return Num2IP(numAddress);
		}
		private static string GetBaseN(IPv4Type type) {
			string baseN;
			switch (type) {
				default:
				case IPv4Type.eIPAddress:
					baseN = BaseN_e;
					break;
				case IPv4Type.kIPAddress:
					baseN = BaseN_k;
					break;
			}
			return baseN;
		}
	}
}
