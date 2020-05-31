using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace GKit.Network {
	public abstract class ClientBase {
		public ClientState State {
			get {
				lock (stateLock)
					return state;
			}
		}
		public Socket Socket {
			get {
				lock (globalLock)
					return socket;
			}
		}
		public EndPoint ServerEndPoint {
			get {
				lock (globalLock)
					return serverEndPoint;
			}
		}

		#region Locks

		private object stateLock = new object();
		private object globalLock = new object();

		#endregion

		private ClientState state = ClientState.Disconnected;
		private NetProtocol protocol;
		private bool isSending;
		private bool noDelay;
		private bool useKeepAlive;
		private bool useLinger;
		private int keepAliveTime;
		private int keepAliveInterval;
		private int lingerTime;
		private IPEndPoint serverEndPoint;
		private Socket socket;
		private byte[] header;
		private SocketAsyncEventArgs connectEventArgs;
		private SocketAsyncEventArgs sendEventArgs;
		private SocketAsyncEventArgs receiveEventArgs;
		private Queue<byte[]> sendQueue = new Queue<byte[]>();

		//Event

		protected abstract void OnConnected();
		protected abstract void OnConnectFailed(Exception ex);
		protected abstract void OnDisconnectedByError(Exception ex);
		protected abstract void OnDisconnected();
		protected abstract void OnHeaderReceived(byte[] header);
		protected abstract void OnPacketReceived(byte[] packet);

		/// <param name="protocol">통신할 때 사용되는 규칙 (null : 기본값 int)</param>
		/// <param name="noDelay">패킷을 여러 개 모아 전송할 지 여부</param>
		/// <param name="useKeepAlive">연결 유지 사용</param>
		/// <param name="keepAliveTime">연결 유지 시간제한 (밀리초)</param>
		/// <param name="keepAliveInternal">연결 유지 간격 (밀리초)</param>
		/// <param name="useLinger">연결 끊김 시 남은 버퍼 전송 여부</param>
		/// <param name="lingerTime">남은 버퍼 전송 대기시간 (초)</param>
		public ClientBase(NetProtocol protocol = null, bool noDelay = false, bool useKeepAlive = true, int keepAliveTime = 3000, int keepAliveInternal = 1000, bool useLinger = false, int lingerTime = 3) {
			if (protocol == null) {
				protocol = new NetProtocol();
			}

			this.protocol = protocol;
			this.noDelay = noDelay;
			this.useKeepAlive = useKeepAlive;
			this.useLinger = useLinger;
			this.keepAliveTime = keepAliveTime;
			this.keepAliveInterval = keepAliveInternal;
			this.lingerTime = lingerTime;
			
			header = new byte[protocol.HeaderSize];
		}
		private void Reset() {
			connectEventArgs = new SocketAsyncEventArgs();
			sendEventArgs = new SocketAsyncEventArgs();
			receiveEventArgs = new SocketAsyncEventArgs();

			connectEventArgs.Completed += OnConnect;
			sendEventArgs.Completed += OnPacketSended;
		}

		public void ConnectTo(IPEndPoint serverEndPoint) {
			this.serverEndPoint = serverEndPoint;
			lock (stateLock) {
				if (state != ClientState.Disconnected)
					return;

				state = ClientState.Connecting;
			}

			Reset();
			Exception exception = null;

			lock (globalLock) {
				try {
					//소켓 설정
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.NoDelay = !noDelay;
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(useLinger, lingerTime));

					byte[] optionBuffer = new byte[12];
					Array.Copy(BitConverter.GetBytes(useKeepAlive ? 1 : 0), 0, optionBuffer, 0, sizeof(int));
					Array.Copy(BitConverter.GetBytes(keepAliveTime), 0, optionBuffer, sizeof(int), sizeof(int));
					Array.Copy(BitConverter.GetBytes(keepAliveInterval), 0, optionBuffer, sizeof(int) * 2, sizeof(int));

					try {
						socket.IOControl(IOControlCode.KeepAliveValues, optionBuffer, null);
					} catch {
						useKeepAlive = false;
					}

					connectEventArgs.RemoteEndPoint = serverEndPoint;
				
				} catch (Exception ex) {
					if (socket != null) {
						socket.Close();
						socket = null;
					}

					lock (stateLock)
						state = ClientState.Disconnected;

					exception = ex;
				}
			}

			if (exception != null) {
				OnConnectFailed(exception);
				return;
			} else {
				try {
					if (!socket.ConnectAsync(connectEventArgs))
						OnConnect(socket, connectEventArgs);
				} catch (Exception ex) {
					OnConnectFailed(ex);
				}
			}
		}
		public void Disconnect() {
			if (DisconnectedWork()) {

				OnDisconnected();
				serverEndPoint = null;
			}
		}

		public void Send(byte[] data) {
			if (state != ClientState.Connected || data == null)
				return;

			PacketBuilder builder = new PacketBuilder();
			builder.Append(protocol.Header2Bytes(data.Length));
			builder.Append(data);

			byte[] packet = builder.ToArray();

			lock (sendEventArgs) {
				if (isSending) {
					sendQueue.Enqueue(packet);

					return;
				} else {
					isSending = true;
				}
			}

			try {
				sendEventArgs.SetBuffer(packet, 0, packet.Length);
				if (!socket.SendAsync(sendEventArgs))
					OnPacketSended(socket, sendEventArgs);
			} catch (Exception ex) {
				DisconnectByError(ex);
			}
		}
		

		private void DisconnectByError(Exception ex) {
			if (DisconnectedWork()) {

				DisconnectByError(ex);
				serverEndPoint = null;
			}
		}
		private bool DisconnectedWork() {
			lock (stateLock) {
				if (state != ClientState.Connected) {
					return false;
				}

				state = ClientState.Disconnecting;
			}

			lock (globalLock) {
				try {
					socket.Shutdown(SocketShutdown.Both);
				} catch {
				}
				socket.Close();
				socket = null;

				lock (stateLock)
					state = ClientState.Disconnected;
			}
			return true;
		}
		private bool CheckAvailable(Socket socket, SocketAsyncEventArgs e, NetProtocol.SocketAsyncWorkDelegate tryMethod, NetProtocol.SocketAsyncEventDelegate repeatMethod) {
			if (e.SocketError != SocketError.TimedOut &&
					   e.SocketError != SocketError.ConnectionReset &&
					   e.SocketError != SocketError.Success) {
				DisconnectByError(new SocketException((int)e.SocketError));
			} else if (e.SocketError == SocketError.TimedOut) {
				Disconnect();
			} else if (e.SocketError == SocketError.ConnectionReset || e.BytesTransferred == 0) {
				Disconnect();
			} else if (e.Count - e.BytesTransferred != 0) {
				try {
					e.SetBuffer(e.Buffer, e.Offset + e.BytesTransferred, e.Count - e.BytesTransferred);
					if (!tryMethod(e))
						repeatMethod(socket, e);
				} catch (Exception ex) {
					DisconnectByError(ex);
				}
			} else {
				return true;
			}
			lock(sendEventArgs)
				isSending = false;
			return false;
		}
		//Event
		private void OnConnect(object sender, SocketAsyncEventArgs e) {
			if (e.SocketError != SocketError.Success) {
				if (socket != null) {
					socket.Close();
					socket = null;
				}

				lock (stateLock)
					state = ClientState.Disconnected;

				OnConnectFailed(new SocketException((int)e.SocketError));

				return;
			}

			lock (stateLock)
				state = ClientState.Connected;

			receiveEventArgs.SetBuffer(header, 0, header.Length);
			receiveEventArgs.Completed += OnHeaderReceived;

			OnConnected();

			try {
				if (!socket.ReceiveAsync(receiveEventArgs))
					OnHeaderReceived(socket, receiveEventArgs);
			} catch (Exception ex) {
				DisconnectByError(ex);
			}
		}
		private void OnPacketSended(object sender, SocketAsyncEventArgs e) {
			Socket socket = (Socket)sender;

			if (CheckAvailable(socket, e, socket.SendAsync, OnPacketSended)) {
				byte[] packet;
				lock (e) {
					if (sendQueue.Count == 0) {
						isSending = false;
						return;
					} else {
						for (; ; ) {
							packet = sendQueue.Dequeue();
							if (packet != null)
								break;
							if (sendQueue.Count == 0) {
								isSending = false;
								return;
							}
						}
					}
				}

				try {
					e.SetBuffer(packet, 0, packet.Length);
					if (!socket.SendAsync(e))
						OnPacketSended(socket, e);
				} catch (Exception ex) {
					DisconnectByError(ex);
				}
			}
		}
		private void OnHeaderReceived(object sender, SocketAsyncEventArgs e) {
			Socket socket = (Socket)sender;
			if (CheckAvailable(this.socket, e, socket.ReceiveAsync, this.OnHeaderReceived)) {
				int packetLength = protocol.Bytes2Header(e.Buffer);

				OnHeaderReceived(e.Buffer);

				if (packetLength <= 0) {
					DisconnectByError(new ArgumentOutOfRangeException("올바르지 않은 패킷 길이입니다."));
				} else {
					e.SetBuffer(new byte[packetLength], 0, packetLength);

					e.Completed -= OnHeaderReceived;
					e.Completed += OnPacketReceived;
					try {
						if (!socket.ReceiveAsync(e)) {
							OnPacketReceived(socket, e);
						}
					} catch (Exception ex) {
						DisconnectByError(ex);
					}
				}
			}
		}
		private void OnPacketReceived(object sender, SocketAsyncEventArgs e) {
			Socket socket = (Socket)sender;

			if (CheckAvailable(socket, e, socket.ReceiveAsync, OnPacketReceived)) {
				OnPacketReceived(e.Buffer);

				e.SetBuffer(header, 0, header.Length);

				e.Completed -= OnPacketReceived;
				e.Completed += OnHeaderReceived;
				try {
					if (!socket.ReceiveAsync(e)) {
						OnHeaderReceived(socket, e);
					}
				} catch (Exception ex) {
					DisconnectByError(ex);
				}
			}
		}
	}
}