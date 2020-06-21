using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
.Network {
	public abstract class GServerBase {
		public class DisconnectedEventArgs {
			public bool OnException => exception != null;
			public Exception exception;
		}

		public GServerState ServerState {
			get {
				lock (stateLock) {
					return state;
				}
			}
		}
		public bool AcceptConnection {
			get {
				return acceptConnection;
			}
		}
		public int Port {
			get {
				return port;
			}
		}
		public int BackLogNum {
			get {
				return backLogNum;
			}
		}
		public int ClientCount {
			get {
				lock (ClientSet) {
					return ClientSet.Count;
				}
			}
		}

		//Locks
		private object stateLock = new object();
		private object globalLock = new object();

		private GServerState state = GServerState.Stopped;
		private bool acceptConnection;

		private int port;
		private int backLogNum;
		private GSocketArgs socketArgs;
		private NetProtocol protocol;

		private Socket socket;
		private Thread acceptThread;

		public HashSet<Socket> ClientSet {
			get; private set;
		}
		private Dictionary<Socket, GClientData> clientDataDict;

		protected abstract void OnFatalError(Exception ex);
		protected abstract void OnStarted();
		protected abstract void OnClosed();
		protected abstract void OnClientConnected(Socket client);
		protected abstract void OnClientDisconnected(Socket client);
		protected abstract void OnClientDisconnectedByError(Socket client, Exception ex);
		protected abstract void OnHeaderReceived(Socket client, byte[] header);
		protected abstract void OnPacketReceived(Socket client, byte[] packet);

		public GServerBase(NetProtocol protocol, GSocketArgs args) {
			if (protocol == null) {
				protocol = new NetProtocol();
			}

			this.protocol = protocol;
			this.socketArgs = args;

			Init();
		}
		private void Init() {
			ClientSet = new HashSet<Socket>();
			clientDataDict = new Dictionary<Socket, GClientData>();

			acceptConnection = true;
		}
		public void Clear() {
			port = -1;
			backLogNum = -1;

			if (socket != null) {
				socket.Close();
				socket = null;
			}

			lock (stateLock)
				state = GServerState.Stopped;
		}

		public void Start(int port, int backLogNum) {
			lock (stateLock) {
				if (state != GServerState.Stopped)
					return;

				state = GServerState.Starting;
			}


			lock (globalLock) {
				try {
					//소켓 설정
					this.port = port;
					this.backLogNum = backLogNum;
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Bind(new IPEndPoint(IPAddress.Any, port));
					socket.Listen(backLogNum);
					socket.NoDelay = !socketArgs.noDelay;

					byte[] optionBuffer = new byte[12];
					Array.Copy(BitConverter.GetBytes(socketArgs.useKeepAlive ? 1 : 0), 0, optionBuffer, 0, sizeof(int));
					Array.Copy(BitConverter.GetBytes(socketArgs.keepAliveTime), 0, optionBuffer, sizeof(int), sizeof(int));
					Array.Copy(BitConverter.GetBytes(socketArgs.keepAliveInterval), 0, optionBuffer, sizeof(int) * 2, sizeof(int));

					try {
						socket.IOControl(IOControlCode.KeepAliveValues, optionBuffer, null);
					} catch {
						socketArgs.useKeepAlive = false;
					}

					acceptThread = new Thread(AcceptClient);
					acceptThread.Start();

					lock (stateLock)
						state = GServerState.Running;

					OnStarted();
				} catch (Exception ex) {
					Clear();

					OnFatalError(ex);
				}
			}
		}
		public void Close() {
			CloseWork();

			OnClosed();
		}
		public void SetAcceptConnection(bool accept) {
			acceptConnection = accept;
		}
		public void DisconnectClient(Socket client) {
			if (DisconnectClientWork(client)) {

				OnClientDisconnected(client);
			}
		}

		public void SendAll(byte[] packet) {
			Socket[] sockets = ClientSet.ToArray();
			foreach (var client in sockets) {
				SendTo(client, packet);
			}
		}
		public void SendTo(Socket client, byte[] data) {
			if (data == null)
				return;
			if(!client.Connected) {
				OnFatalError(new Exception("Packet can't be sent because the socket isn't connected."));
				return;
			}
			
			GClientData clientData;
			lock (ClientSet) {
				clientData = clientDataDict[client];
			}
			byte[] packet = protocol.Header2Bytes(data.Length).Concat(
				data).ToArray();

			lock (clientData.sendEventArgs) {
				if (clientData.isSending) {
					clientData.sendQueue.Enqueue(packet);
					return;
				} else {
					clientData.isSending = true;
				}
			}

			try {
				clientData.sendEventArgs.SetBuffer(packet, 0, packet.Length);
				if (!clientData.SendAsync())
					Base_OnSendPacket(client, clientData.sendEventArgs);
			} catch (Exception ex) {
				DisconnectClientByError(clientData.socket, ex);
			}
		}


		private void CloseByError(Exception ex) {
			CloseWork();

			OnFatalError(ex);
		}
		private void CloseWork() {
			lock (stateLock) {
				if (state != GServerState.Running)
					return;

				state = GServerState.Stopping;
			}

			int nClientNum;

			lock (ClientSet)
				nClientNum = ClientSet.Count;

			lock (globalLock) {
				lock (ClientSet) {
					foreach (var sClient in ClientSet) {
						try {
							sClient.Shutdown(SocketShutdown.Both);
						} finally {
							sClient.Close();
						}
					}

					ClientSet.Clear();
					clientDataDict.Clear();
				}

				port = -1;
				backLogNum = -1;

				if (socket != null) {
					socket.Close();
					socket = null;
				}

				lock (stateLock)
					state = GServerState.Stopped;
			}
		}
		private void DisconnectClientByError(Socket client, Exception ex) {
			if (ex is ObjectDisposedException) {
				//접속 종료
				DisconnectClient(client);
			} else {
				//처리되지 않은 예외
				if (DisconnectClientWork(client)) {
					OnClientDisconnectedByError(client, ex);
				}
			}
		}
		private bool DisconnectClientWork(Socket client) {
			lock (stateLock)
				if (state != GServerState.Running) {
					return false;
				}

			lock (ClientSet) {
				ClientSet.Remove(client);
				clientDataDict.Remove(client);
			}

			try {
				client.Shutdown(SocketShutdown.Both);
			} catch {
			}
			client.Close();
			client = null;
			return true;
		}
		private void AcceptClient() {
			try {
				for (; ; ) {
					if (state != GServerState.Starting && state != GServerState.Running)
						break;

					Socket client = null;
					try {
						client = socket.Accept();
					} catch (SocketException ex) {
					}

					if (client == null)
						continue;
					if (!acceptConnection) {
						try {
							client.Shutdown(SocketShutdown.Both);
						} catch {
							client.Close();
						}
						client = null;
						continue;
					}

					client.NoDelay = socketArgs.noDelay;
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(socketArgs.useLinger, socketArgs.lingerTime));

					//클라이언트 데이터 생성
					GClientData data;
					lock (ClientSet) {
						data = new GClientData(client, protocol.HeaderSize);
						ClientSet.Add(client);
						clientDataDict.Add(client, data);
					}

					//이벤트 설정
					data.sendEventArgs.Completed += Base_OnSendPacket;

					OnClientConnected(client);

					StartReceiveClient(data);
				}
			} catch (Exception sServerException) {
				CloseByError(sServerException);
			}
		}
		private void StartReceiveClient(GClientData clientData) {
			try {
				clientData.receiveEventArgs.SetBuffer(clientData.receivedHeaderBuffer, 0, clientData.receivedHeaderBuffer.Length);
				clientData.receiveEventArgs.Completed += Base_OnHeaderReceived;

				if (!clientData.socket.ReceiveAsync(clientData.receiveEventArgs))
					Base_OnHeaderReceived(clientData.socket, clientData.receiveEventArgs);
			} catch (Exception sException) {
				DisconnectClientByError(clientData.socket, sException);
			}
		}
		private bool CheckAvailable(Socket client, SocketAsyncEventArgs e, NetProtocol.SocketAsyncWorkDelegate tryMethod, NetProtocol.SocketAsyncEventDelegate repeatMethod) {
			if (e.SocketError != SocketError.TimedOut &&
			   e.SocketError != SocketError.ConnectionReset &&
			   e.SocketError != SocketError.Success) {
				DisconnectClientByError(client, new SocketException((int)e.SocketError));
			} else if (e.SocketError == SocketError.TimedOut) {
				DisconnectClient(client);
			} else if (e.SocketError == SocketError.ConnectionReset || e.BytesTransferred == 0) {
				DisconnectClient(client);
			} else if (e.Count - e.BytesTransferred != 0) {
				try {
					e.SetBuffer(e.Buffer, e.Offset + e.BytesTransferred, e.Count - e.BytesTransferred);
					if (!tryMethod(e))
						repeatMethod(client, e);
				} catch (Exception ex) {
					DisconnectClientByError(client, ex);
				}
			} else {
				return true;
			}
			return false;
		}
		
		//Event
		private void Base_OnSendPacket(object sender, SocketAsyncEventArgs e) {
			Socket client = (Socket)sender;

			if (CheckAvailable(client, e, client.SendAsync, Base_OnSendPacket)) {
				GClientData data;

				lock (ClientSet) {
					if (!clientDataDict.TryGetValue(client, out data)) {
						return;
					}
				}

				byte[] packet;

				lock (e) {
					if (data.sendQueue.Count == 0) {
						data.isSending = false;
						return;
					} else {
						for (; ; ) {
							packet = data.sendQueue.Dequeue();
							if (packet != null)
								break;
							if (data.sendQueue.Count == 0) {
								data.isSending = false;
								return;
							}
						}
					}
				}

				try {
					e.SetBuffer(packet, 0, packet.Length);
					if (!client.SendAsync(e))
						Base_OnSendPacket(client, e);
				} catch (Exception ex) {
					DisconnectClientByError(client, ex);
				}
			}
		}
		private void Base_OnHeaderReceived(object sender, SocketAsyncEventArgs e) {
			Socket client = (Socket)sender;

			if (CheckAvailable(client, e, client.ReceiveAsync, Base_OnHeaderReceived)) {
				int packetLength = protocol.Bytes2Header(e.Buffer);

				OnHeaderReceived(client, e.Buffer);

				if (packetLength <= 0)
					DisconnectClientByError(client, new ArgumentOutOfRangeException("올바르지 않은 패킷 길이입니다."));
				else {
					e.SetBuffer(new byte[packetLength], 0, packetLength);

					e.Completed -= Base_OnHeaderReceived;
					e.Completed += Base_OnPacketReceived;

					try {
						if (!client.ReceiveAsync(e))
							Base_OnPacketReceived(client, e);
					} catch (Exception ex) {
						DisconnectClientByError(client, ex);
					}
				}
			}
		}
		private void Base_OnPacketReceived(object sender, SocketAsyncEventArgs e) {
			Socket client = (Socket)sender;

			if (CheckAvailable(client, e, client.ReceiveAsync, Base_OnPacketReceived)) {
				GClientData data;

				lock (ClientSet)
					if (!clientDataDict.TryGetValue(client, out data))
						return;

				OnPacketReceived(client, e.Buffer);

				e.SetBuffer(data.receivedHeaderBuffer, 0, data.receivedHeaderBuffer.Length);
				e.Completed -= Base_OnPacketReceived;
				e.Completed += Base_OnHeaderReceived;

				try {
					if (!client.ReceiveAsync(e))
						Base_OnHeaderReceived(client, e);
				} catch (Exception sException) {
					DisconnectClientByError(client, sException);
				}
			}
		}
	}
}