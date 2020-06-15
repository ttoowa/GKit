///////////////////////////////////////////////////////////////////////////////
// MessageEvents.cs - native Windows message handling for WintabDN
//
// This code in this file is based on the example given at:
//  http://msdn.microsoft.com/en-us/magazine/cc163417.aspx
//  by Steven Toub.
//
// Copyright (c) 2010, Wacom Technology Corporation
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
///////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WintabDN {
	/// <summary>
	/// Support for registering a Native Windows message with MessageEvents class.
	/// </summary>
	public class MessageReceivedEventArgs : EventArgs {
		private readonly Message _message;

		/// <summary>
		/// MessageReceivedEventArgs constructor.
		/// </summary>
		/// <param name="message">Native windows message to be registered.</param>
		public MessageReceivedEventArgs(Message message) { _message = message; }

		/// <summary>
		/// Return native Windows message handled by this object.
		/// </summary>
		public Message Message { get { return _message; } }
	}

	/// <summary>
	/// Windows native message handler, to provide support for detecting and
	/// responding to Wintab messages. 
	/// </summary>
	public static class WMessageEvents {
		private static object _lock = new object();
		private static WMessageWindow _window;
		private static IntPtr _windowHandle;
		private static SynchronizationContext _context;
		private static ManualResetEvent mre;
		private static bool isRunning;

		/// <summary>
		/// MessageEvents delegate.
		/// </summary>
		public static event EventHandler<MessageReceivedEventArgs> MessageReceived;

		/// <summary>
		/// Registers to receive the specified native Windows message.
		/// </summary>
		/// <param name="message">Native Windows message to watch for.</param>
		public static void WatchMessage(int message) {
			EnsureInitialized();
			_window.RegisterEventForMessage(message);
		}

		/// <summary>
		/// Returns the MessageEvents native Windows handle.
		/// </summary>
		public static IntPtr WindowHandle => _windowHandle;
		private static void EnsureInitialized() {
			lock (_lock) {
				if (_window == null) {
					_context = AsyncOperationManager.SynchronizationContext;
					mre = new ManualResetEvent(false);
					Thread pumpThread = new Thread(WindowThread);
					pumpThread.Name = "MessageEvents message loop";
					pumpThread.IsBackground = true;
					pumpThread.Start();

					mre.WaitOne();
					mre.Dispose();
					mre = null;
				}
			}
		}
		public static void Start() {
			isRunning = true;
			EnsureInitialized();
		}
		public static void Close() {
			isRunning = false;
		}

		private static void WindowThread() {
			_window = new WMessageWindow();
			_windowHandle = _window.Handle;
			mre.Set();
			CheckAvailable();
			Application.Run();
		}
		private static async void CheckAvailable() {
			for (; ; ) {
				if (!isRunning) {
					Application.Exit();
					return;
				}
				await Task.Delay(1);
			}
		}

		private class WMessageWindow : Form {
			private const int WM_DESTROY = 0x0002;

			private ReaderWriterLock _lock = new ReaderWriterLock();
			private Dictionary<int, bool> _messageSet = new Dictionary<int, bool>();

			public void RegisterEventForMessage(int messageID) {
				_lock.AcquireWriterLock(Timeout.Infinite);
				_messageSet[messageID] = true;
				_lock.ReleaseWriterLock();
			}

			protected override void WndProc(ref Message m) {
				_lock.AcquireReaderLock(Timeout.Infinite);
				bool handleMessage = _messageSet.ContainsKey(m.Msg);
				_lock.ReleaseReaderLock();

				if (handleMessage) {
					WMessageEvents._context.Post(delegate (object state) {
						EventHandler<MessageReceivedEventArgs> handler = WMessageEvents.MessageReceived;
						if (handler != null) handler(null, new MessageReceivedEventArgs((Message)state));
					}, m);
				}
				switch (m.Msg) {
					case WM_DESTROY:
						Application.Exit();
						break;
				}

				base.WndProc(ref m);
			}
		}
	}
}
