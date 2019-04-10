
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace GKit.Network {
	public abstract class ServerBase {
		public ServerState ServerState {
			get {
				lock (stateLock) {
					return state;
				}
			}
		}
		public bool IsAcceptConnecting {
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
				lock (clientSet) {
					return clientSet.Count;
				}
			}
		}

		#region Locks

		private object stateLock = new object();
		private object globalLock = new object();

		#endregion

		private ServerState state = ServerState.Stopped;
		private NetProtocol protocol;
		private bool acceptConnection;
		private int port;
		private int backLogNum;
		private bool noDelay;
		private bool usekeepAlive;
		private bool useLinger;
		private int keepAliveTime;
		private int keepAliveInternal;
		private int lingerTime;
		private Socket socket;
		private Thread acceptThread;
		private HashSet<Socket> clientSet;
		private Dictionary<Socket, ClientData> clientDataDict;
		
		protected abstract void OnStarted();
		protected abstract void OnStartFailed(Exception ex);
		protected abstract void OnClosed();
		protected abstract void OnClosedByError(Exception ex);
		protected abstract void OnClientConnected(Socket client);
		protected abstract void OnClientDisconnected(Socket client);
		protected abstract void OnClientDisconnectedByError(Socket client, Exception ex);
		protected abstract void OnHeaderReceived(Socket client, byte[] header);
		protected abstract void OnPacketReceived(Socket client, byte[] packet);

		/// <param name="protocol">통신할 때 사용되는 규칙 (null : 기본값)</param>
		/// <param name="noDelay">패킷을 여러 개 모아 전송할 지 여부</param>
		/// <param name="useKeepAlive">연결 유지 사용</param>
		/// <param name="keepAliveTime">연결 유지 시간제한 (밀리초)</param>
		/// <param name="keepAliveInternal">연결 유지 간격 (밀리초)</param>
		/// <param name="useLinger">연결 끊김 시 남은 버퍼 전송 여부</param>
		/// <param name="lingerTime">남은 버퍼 전송 대기시간 (초)</param>
		public ServerBase(NetProtocol protocol = null, bool noDelay = false, bool useKeepAlive = true, int keepAliveTime = 3000, int keepAliveInternal = 1000, bool useLinger = false, int lingerTime = 3) {
			if (protocol == null) {
				protocol = new NetProtocol();
			}

			this.protocol = protocol;
			this.noDelay = noDelay;
			this.usekeepAlive = useKeepAlive;
			this.useLinger = useLinger;
			this.keepAliveTime = keepAliveTime;
			this.keepAliveInternal = keepAliveInternal;
			this.lingerTime = lingerTime;

			Init();
		}
		private void Init() {
			clientSet = new HashSet<Socket>();
			clientDataDict = new Dictionary<Socket, ClientData>();

			acceptConnection = true;
		}

		public void Start(int port, int backLogNum) {
			lock (stateLock) {
				if (state != ServerState.Stopped)
					return;

				state = ServerState.Starting;
			}
			

			lock (globalLock) {
				try {
					//소켓 설정
					this.port = port;
					this.backLogNum = backLogNum;
					socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					socket.Bind(new IPEndPoint(IPAddress.Any, port));
					socket.Listen(backLogNum);
					socket.NoDelay = !noDelay;

					byte[] optionBuffer = new byte[12];
					Array.Copy(BitConverter.GetBytes(usekeepAlive ? 1 : 0), 0, optionBuffer, 0, sizeof(int));
					Array.Copy(BitConverter.GetBytes(keepAliveTime), 0, optionBuffer, sizeof(int), sizeof(int));
					Array.Copy(BitConverter.GetBytes(keepAliveInternal), 0, optionBuffer, sizeof(int) * 2, sizeof(int));

					try {
						socket.IOControl(IOControlCode.KeepAliveValues, optionBuffer, null);
					} catch {
						usekeepAlive = false;
					}

					acceptThread = new Thread(AcceptClient);
					acceptThread.Start();

					lock (stateLock)
						state = ServerState.Running;

					OnStarted();
				} catch (Exception ex) {
					port = -1;
					backLogNum = -1;

					if (socket != null) {
						socket.Close();
						socket = null;
					}

					lock (stateLock)
						state = ServerState.Stopped;

					OnStartFailed(ex);
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
			foreach (var client in clientSet) {
				SendTo(client, packet);
			}
		}
		public void SendTo(Socket client, byte[] data) {
			ClientData clientData;
			lock (clientSet) {
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
					OnSendPacket(client, clientData.sendEventArgs);
			} catch (Exception ex) {
				DisconnectClientByError(clientData.socket, ex);
			}
		}


		private void CloseByError(Exception ex) {
			CloseWork();

			OnClosedByError(ex);
		}
		private void CloseWork() {
			lock (stateLock) {
				if (state != ServerState.Running)
					return;

				state = ServerState.Stopping;
			}

			int nClientNum;

			lock (clientSet)
				nClientNum = clientSet.Count;

			lock (globalLock) {
				lock (clientSet) {
					foreach (var sClient in clientSet) {
						try {
							sClient.Shutdown(SocketShutdown.Both);
						} finally {
							sClient.Close();
						}
					}

					clientSet.Clear();
					clientDataDict.Clear();
				}

				port = -1;
				backLogNum = -1;

				if (socket != null) {
					socket.Close();
					socket = null;
				}

				lock (stateLock)
					state = ServerState.Stopped;
			}
		}
		private void DisconnectClientByError(Socket client, Exception ex) {
			if(ex is ObjectDisposedException) {
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
				if (state != ServerState.Running) {
					return false;
				}

			lock (clientSet) {
				clientSet.Remove(client);
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
					if (state != ServerState.Starting && state != ServerState.Running)
						break;

					Socket client = null;
					try {
						client = socket.Accept();
					} catch(SocketException ex) {
					}

					if (client == null)
						continue;
					if(!acceptConnection) {
						try {
							client.Shutdown(SocketShutdown.Both);
						} catch {
							client.Close();
						}
						client = null;
						continue;
					}

					client.NoDelay = !noDelay;
					client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Linger, new LingerOption(useLinger, lingerTime));

					//클라이언트 데이터 생성
					ClientData data;
					lock (clientSet) {
						data = new ClientData(client, protocol.HeaderSize);
						clientSet.Add(client);
						clientDataDict.Add(client, data);
					}

					//이벤트 설정
					data.sendEventArgs.Completed += OnSendPacket;

					OnClientConnected(client);

					StartReceiveClient(data);
				}
			} catch (Exception sServerException) {
				CloseByError(sServerException);
			}
		}
		private void StartReceiveClient(ClientData clientData) {
			try {
				clientData.receiveEventArgs.SetBuffer(clientData.receivedHeaderBuffer, 0, clientData.receivedHeaderBuffer.Length);
				clientData.receiveEventArgs.Completed += OnHeaderReceived;

				if (!clientData.socket.ReceiveAsync(clientData.receiveEventArgs))
					OnHeaderReceived(clientData.socket, clientData.receiveEventArgs);
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
		private void OnSendPacket(object sender, SocketAsyncEventArgs e) {
			Socket client = (Socket)sender;
			
			if(CheckAvailable(client, e, client.SendAsync, OnSendPacket)) {
				ClientData data;

				lock (clientSet) {
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
						OnSendPacket(client, e);
				} catch (Exception ex) {
					DisconnectClientByError(client, ex);
				}
			}
		}
		private void OnHeaderReceived(object sender, SocketAsyncEventArgs e) {
			Socket client = (Socket)sender;

			if(CheckAvailable(client, e, client.ReceiveAsync, OnHeaderReceived)) {
				int packetLength = protocol.Bytes2Header(e.Buffer);

				OnHeaderReceived(client, e.Buffer);

				if (packetLength <= 0)
					DisconnectClientByError(client, new ArgumentOutOfRangeException("올바르지 않은 패킷 길이입니다."));
				else {
					e.SetBuffer(new byte[packetLength], 0, packetLength);

					e.Completed -= OnHeaderReceived;
					e.Completed += OnPacketReceived;

					try {
						if (!client.ReceiveAsync(e))
							OnPacketReceived(client, e);
					} catch (Exception ex) {
						DisconnectClientByError(client, ex);
					}
				}
			}
		}
		private void OnPacketReceived(object sender, SocketAsyncEventArgs e) {
			Socket client = (Socket)sender;
			
			if(CheckAvailable(client, e, client.ReceiveAsync, OnPacketReceived)) {
				ClientData data;

				lock (clientSet)
					if (!clientDataDict.TryGetValue(client, out data))
						return;

				OnPacketReceived(client, e.Buffer);

				e.SetBuffer(data.receivedHeaderBuffer, 0, data.receivedHeaderBuffer.Length);
				e.Completed -= OnPacketReceived;
				e.Completed += OnHeaderReceived;

				try {
					if (!client.ReceiveAsync(e))
						OnHeaderReceived(client, e);
				} catch (Exception sException) {
					DisconnectClientByError(client, sException);
				}
			}
		}
	}
}