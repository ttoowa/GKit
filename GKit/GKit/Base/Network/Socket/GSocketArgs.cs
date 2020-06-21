using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GKit.Base.Network.Socket {
	public class GSocketArgs {
		/// <param name="noDelay">패킷을 여러 개 모아 전송할 지 여부</param>
		/// <param name="useKeepAlive">연결 유지 사용</param>
		/// <param name="keepAliveTime">연결 유지 시간제한 (밀리초)</param>
		/// <param name="keepAliveInterval">연결 유지 간격 (밀리초)</param>
		/// <param name="useLinger">연결 끊김 시 남은 버퍼 전송 여부</param>
		/// <param name="lingerTime">남은 버퍼 전송 대기시간 (초)</param>
		/// 
		public bool noDelay = false;

		public bool useKeepAlive = true;
		public int keepAliveTime = 3000;
		public int keepAliveInterval = 1000;

		public bool useLinger = false;
		public int lingerTime = 3;
	}
}
