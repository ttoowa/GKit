using GKit.Base.Network.Socket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Network {

	//TODO : 에러가 발생하는 상황을 캐치해서 Disconnect하는 로직 작성하기
	public abstract class GClientBase {
		public class DisconnectedEventArgs {
			public bool OnException => exception != null;
			public Exception exception;
		}

		public GClientState State {
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

		//Locks
		private object stateLock = new object();
		private object globalLock = new object();

		private IPEndPoint serverEndPoint;
		private GSocketArgs socketArgs;
		private NetProtocol protocol;

		private Socket socket;

		private GClientState state = GClientState.Disconnected;
		private bool isSending;
		private byte[] header;
		private SocketAsyncEventArgs sendEventArgs;
		private SocketAsyncEventArgs receiveEventArgs;
		private Queue<byte[]> sendQueue = new Queue<byte[]>();

		//Event

		protected abstract void OnFatalError(Exception ex);
		protected abstract void OnConnected();
		protected abstract void OnDisconnected(DisconnectedEventArgs e);
		protected abstract void OnHeaderReceived(byte[] header);
		protected abstract void OnPacketReceived(byte[] packet);

		public GClientBase(NetProtocol protocol, GSocketArgs args) {
			if (protocol == null) {
				protocol = new NetProtocol();
			}

			this.protocol = protocol;
			this.socketArgs = args;
			
			header = new byte[protocol.HeaderSize];
		}
		private void Reset() {
			sendEventArgs = new SocketAsyncEventArgs();
			receiveEventArgs = new SocketAsyncEventArgs();

			sendEventArgs.Completed += Base_OnPacketSended;
		}

		public void Connect(IPEndPoint serverEndPoint, bool useAsync = false) {
			//Change state
			lock (stateLock) {
				if (state != GClientState.Disconnected)
					throw new Exception("Socket already connected.");

				state = GClientState.Connecting;
				this.serverEndPoint = serverEndPoint;
			}

			Reset();

			//Socket setting
			Exception socketSettingException = null;
			lock (globalLock) {
				try {
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.NoDelay = socketArgs.noDelay;
					socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(socketArgs.useLinger, socketArgs.lingerTime));

					byte[] optionBuffer = new byte[12];
					Array.Copy(BitConverter.GetBytes(socketArgs.useKeepAlive ? 1 : 0), 0, optionBuffer, 0, sizeof(int));
					Array.Copy(BitConverter.GetBytes(socketArgs.keepAliveTime), 0, optionBuffer, sizeof(int), sizeof(int));
					Array.Copy(BitConverter.GetBytes(socketArgs.keepAliveInterval), 0, optionBuffer, sizeof(int) * 2, sizeof(int));

					try {
						socket.IOControl(IOControlCode.KeepAliveValues, optionBuffer, null);
					} catch {
						socketArgs.useKeepAlive = false;
					}
				} catch (Exception ex) {
					if (socket != null) {
						socket.Close();
						socket = null;
					}

					lock (stateLock)
						state = GClientState.Disconnected;

					socketSettingException = ex;
				}
			}

			if (socketSettingException != null) {
				OnFatalError(socketSettingException);
				return;
			}

			//Connect
			SocketAsyncEventArgs connectEventArgs = null;
			try {
				if (useAsync) {
					//Async
					connectEventArgs = new SocketAsyncEventArgs();
					connectEventArgs.Completed += Base_OnConnected;
					connectEventArgs.RemoteEndPoint = serverEndPoint;

					if (!socket.ConnectAsync(connectEventArgs)) {
						//비동기 완료
						Base_OnConnected(socket, connectEventArgs);
					} else {
						return;
					}
				} else {
					//Blocking
					socket.Connect(serverEndPoint);
					Base_OnConnected(socket, null);
				}
			} catch (Exception ex) {
				OnFatalError(ex);
			}
		}
		public void Disconnect() {
			if (DisconnectedJob()) {
				var args = new DisconnectedEventArgs() {
				};

				OnDisconnected(args);
				serverEndPoint = null;
			}
		}

		public void Send(byte[] data) {
			if (state != GClientState.Connected || data == null)
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
					Base_OnPacketSended(socket, sendEventArgs);
			} catch (Exception ex) {
				DisconnectByError(ex);
			}
		}
		

		private void DisconnectByError(Exception ex) {
			if (DisconnectedJob()) {

				DisconnectByError(ex);
				serverEndPoint = null;
			}
		}
		private bool DisconnectedJob() {
			lock (stateLock) {
				if (state != GClientState.Connected) {
					return false;
				}

				state = GClientState.Disconnecting;
			}

			lock (globalLock) {
				try {
					socket.Shutdown(SocketShutdown.Both);
				} catch {
				}
				socket.Close();
				socket = null;

				lock (stateLock)
					state = GClientState.Disconnected;
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
		private void Base_OnConnected(object sender, SocketAsyncEventArgs e) {
			if(e != null) {
				if (e.SocketError != SocketError.Success) {
					if (socket != null) {
						socket.Close();
						socket = null;
					}

					lock (stateLock)
						state = GClientState.Disconnected;

					OnFatalError(new SocketException((int)e.SocketError));

					return;
				}
				e.Dispose();
			}

			lock (stateLock)
				state = GClientState.Connected;

			receiveEventArgs.SetBuffer(header, 0, header.Length);
			receiveEventArgs.Completed += Base_OnHeaderReceived;

			OnConnected();

			try {
				if (!socket.ReceiveAsync(receiveEventArgs))
					Base_OnHeaderReceived(socket, receiveEventArgs);
			} catch (Exception ex) {
				DisconnectByError(ex);
			}
		}
		private void Base_OnPacketSended(object sender, SocketAsyncEventArgs e) {
			Socket socket = (Socket)sender;

			if (CheckAvailable(socket, e, socket.SendAsync, Base_OnPacketSended)) {
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
						Base_OnPacketSended(socket, e);
				} catch (Exception ex) {
					DisconnectByError(ex);
				}
			}
		}
		private void Base_OnHeaderReceived(object sender, SocketAsyncEventArgs e) {
			Socket socket = (Socket)sender;
			if (CheckAvailable(this.socket, e, socket.ReceiveAsync, this.Base_OnHeaderReceived)) {
				int packetLength = protocol.Bytes2Header(e.Buffer);

				OnHeaderReceived(e.Buffer);

				if (packetLength <= 0) {
					DisconnectByError(new ArgumentOutOfRangeException("올바르지 않은 패킷 길이입니다."));
				} else {
					e.SetBuffer(new byte[packetLength], 0, packetLength);

					e.Completed -= Base_OnHeaderReceived;
					e.Completed += Base_OnPacketReceived;
					try {
						if (!socket.ReceiveAsync(e)) {
							Base_OnPacketReceived(socket, e);
						}
					} catch (Exception ex) {
						DisconnectByError(ex);
					}
				}
			}
		}
		private void Base_OnPacketReceived(object sender, SocketAsyncEventArgs e) {
			Socket socket = (Socket)sender;

			if (CheckAvailable(socket, e, socket.ReceiveAsync, Base_OnPacketReceived)) {
				OnPacketReceived(e.Buffer);

				e.SetBuffer(header, 0, header.Length);

				e.Completed -= Base_OnPacketReceived;
				e.Completed += Base_OnHeaderReceived;
				try {
					if (!socket.ReceiveAsync(e)) {
						Base_OnHeaderReceived(socket, e);
					}
				} catch (Exception ex) {
					DisconnectByError(ex);
				}
			}
		}
	}
}