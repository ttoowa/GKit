using System;
using System.Collections.Generic;
using System.Windows;
using WintabDN;
using System.Windows.Forms;
#if OnUnity
using UnityEngine;
using GKitForUnity.MultiThread;
#elif OnWPF
using GKitForWPF.MultiThread;
using System.Windows.Media;
#else
using GKit.MultiThread;
using System.Windows.Media;
#endif

#if OnUnity
namespace GKitForUnity
#elif OnWPF
namespace GKitForWPF
#else
namespace GKit
#endif
{
	/// <summary>
	/// 32Bit 프로세스에서만 사용하세요.
	/// </summary>
	public class WintabInput {
		private const int ActiveCheckFrequency = 100;
		private const float OutputExtFactor = 100f;

		public bool IsRunning {
			get; private set;
		}
		public bool IsAvailable => WInfo.IsWintabAvailable();
		public bool IsStylusActive => WInfo.IsStylusActive();
		public bool IsPenActive {
			get; private set;
		}
		public float Distance {
			get {
				return distance;
			}
		}
		public float Pressure {
			get {
				return pressure;
			}
		}
		public Vector2 Position {
			get {
				return position;
			}
		}
		private float pressure;
		private float distance;
		private Vector2 position;
		public WintabPacket Packet => packet;

		private GLoopEngine core;
		private WContext context;
		private WData data;
		private int maxNormalPressure;
		private int maxTangentPressure;
		private WintabPacket packet;
		public Vector2 displaySize;
		public BRect NativeRect;
		public BRect inputRect;
		public BRect outputRect;
		public BRect systemRect;
		private List<GLoopAction> taskList;
		private float activeCheckDelaySec;
		private int activeStack;
		private int deviceID;
		private object dataWriteLock;

		public WintabInput(GLoopEngine core) {
			this.core = core;
			dataWriteLock = new object();
			taskList = new List<GLoopAction>();

			pressure = 0f;
			distance = 1f;
		}
		private void UpdateFrame() {
			if (!IsRunning)
				return;

			UpdateState();
		}
		private void UpdateSometime() {
			if (!IsRunning)
				return;

			maxNormalPressure = WInfo.GetMaxPressure(true);
			maxTangentPressure = WInfo.GetMaxPressure(false);
			displaySize = new Vector2(
				SystemInformation.VirtualScreen.Width,
				SystemInformation.VirtualScreen.Height);
			if (context != null) {
				inputRect = new BRect(context.InOrgX, context.InOrgY, context.InExtX, context.InExtY);
				outputRect = new BRect(context.OutOrgX, context.OutOrgY, context.OutExtX, context.OutExtY);
				systemRect = new BRect(context.SysOrgX, context.SysOrgY, context.SysExtX, context.SysExtY);
			}
		}

		public void CaptureStart(WContextMode mode) {
			if (IsRunning)
				return;

			try {
				bool result;
				switch (mode) {
					default:
					case WContextMode.Digital:
						result = InitDigitalContextCapture();
						break;
					case WContextMode.System:
						result = InitSystemContextCapture();
						break;
				}
				if (result) {
					IsRunning = true;
					taskList.Add(core.AddLoopAction(UpdateFrame));
					taskList.Add(core.AddLoopAction(UpdateSometime, GLoopCycle.VeryLow));
					UpdateSometime();
				}
			} catch {
				CaptureStop();
			}
		}
		public void CaptureStop() {
			if (!IsRunning)
				return;
			try {
				if (context != null) {
					context.Close();
				}
				IsRunning = false;
				for (int i = 0; i < taskList.Count; ++i) {
					taskList[i].Stop();
				}
				taskList.Clear();
			} catch (Exception ex) {

			}
			context = null;
			data = null;
		}

		private bool InitDigitalContextCapture(bool ctrlSysCursor = true) {
			context = OpenDigitalContext(ctrlSysCursor);

			if (context == null) {
				return false;
			}

			data = new WData(context);
			data.SetWTPacketEventHandler(OnReceivePacket);
			return true;
		}
		private bool InitSystemContextCapture(bool ctrlSysCursor = true) {
			context = OpenSystemContext(ctrlSysCursor);

			if (context == null)
				return false;

			data = new WData(context);
			data.SetWTPacketEventHandler(OnReceivePacket);
			return true;
		}

		private WContext OpenDigitalContext(bool ctrlSysCursor = true) {
			WContext context = WInfo.GetDefaultDigitizingContext();

			if (context == null) {
				return null;
			}

			context.Options |= (uint)ECTXOptionValues.CXO_MESSAGES;
			if (ctrlSysCursor) {
				context.Options |= (uint)ECTXOptionValues.CXO_SYSTEM;
			}
			context.Name = "BgoonLibrary Tablet Context";

			deviceID = WInfo.GetDefaultDeviceIndex();
			WintabAxis tabletX = WInfo.GetDeviceAxis(deviceID, EAxisDimension.AXIS_X);
			WintabAxis tabletY = WInfo.GetDeviceAxis(deviceID, EAxisDimension.AXIS_Y);
			NativeRect = new BRect(tabletX.axMin, tabletX.axMax, tabletY.axMin, tabletY.axMax);

			context.OutOrgX = context.OutOrgY = 0;
			context.OutExtX = (int)(context.OutExtX * OutputExtFactor);
			context.OutExtY = (int)(context.OutExtY * OutputExtFactor);

			//context.OutExtY *= -1;

			return context.Open() ? context : null;
		}
		private WContext OpenSystemContext(bool ctrlSysCursor = true) {
			WContext context = WInfo.GetDefaultSystemContext();

			if (context == null) {
				return null;
			}
			// Set system cursor if caller wants it.
			if (ctrlSysCursor) {
				context.Options |= (uint)ECTXOptionValues.CXO_SYSTEM;
			} else {
				context.Options &= ~(uint)ECTXOptionValues.CXO_SYSTEM;
			}
			context.Name = "BgoonLibrary Tablet Context";

			deviceID = WInfo.GetDefaultDeviceIndex();
			WintabAxis tabletX = WInfo.GetDeviceAxis(deviceID, EAxisDimension.AXIS_X);
			WintabAxis tabletY = WInfo.GetDeviceAxis(deviceID, EAxisDimension.AXIS_Y);
			NativeRect = new BRect(tabletX.axMin, tabletY.axMin, tabletX.axMax, tabletY.axMax);

			context.OutOrgX = context.OutOrgY = 0;
			context.OutExtX = (int)(context.OutExtX * OutputExtFactor);
			context.OutExtY = (int)(context.OutExtY * OutputExtFactor);
			//context.OutOrgX = context.OutOrgY = 0;
			//context.OutExtX = MaxPos.x;
			//context.OutExtY = MaxPos.y;

			//context.OutExtY *= -1;
			return context.Open() ? context : null;
		}

		private void OnReceivePacket(object sender, MessageReceivedEventArgs e) {
			//6~7ms rate
			if (data == null || !IsRunning) {
				return;
			}
			try {
				uint pktID = (uint)e.Message.WParam;
				packet = data.GetDataPacket(pktID);
				if (packet.pkContext.IsValid) {
					lock (dataWriteLock) {
						MarkActive();

						pressure = (float)packet.pkNormalPressure.pkAbsoluteNormalPressure / maxNormalPressure;
						distance = (float)packet.pkZ / maxTangentPressure;
						position = new Vector2(packet.pkX, packet.pkY) / OutputExtFactor;

						position.y = systemRect.yMax - position.y + systemRect.yMin;
					}
				}
			} catch (Exception ex) {
				GDebug.Log(ex.ToString(), GLogLevel.Warnning);
			}
		}

		private void UpdateState() {
			activeCheckDelaySec -= core.LoopElapsedMilliseconds;
			if (activeCheckDelaySec <= 0f) {
				activeCheckDelaySec = ActiveCheckFrequency;

				IsPenActive = ReduceActive();
			}
		}
		private void MarkActive() {
			activeStack = 2;
		}
		private bool ReduceActive() {
			lock (dataWriteLock) {
				if (--activeStack <= 0) {
					activeStack = 0;
					return false;
				} else {
					return true;
				}
			}
		}
#if OnWPF
		public Vector2 GetRelativePos(Visual visual) {
			Vector2 visualPos = (Vector2)visual.PointToScreen(new Point()) - (Vector2)ScreenInfo.VirtualPosition;

			return Position - visualPos;
		}
#endif
	}
}