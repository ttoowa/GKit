///////////////////////////////////////////////////////////////////////////////
// CWintabData.cs - Wintab data management for WintabDN
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
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WintabDN {
	/// <summary>
	/// Wintab Packet bits.
	/// </summary>
	public enum EWintabPacketBit {
		PK_CONTEXT = 0x0001,    /* reporting context */
		PK_STATUS = 0x0002, /* status bits */
		PK_TIME = 0x0004,   /* time stamp */
		PK_CHANGED = 0x0008,    /* change bit vector */
		PK_SERIAL_NUMBER = 0x0010,  /* packet serial number */
		PK_CURSOR = 0x0020, /* reporting cursor */
		PK_BUTTONS = 0x0040,    /* button information */
		PK_X = 0x0080,  /* x axis */
		PK_Y = 0x0100,  /* y axis */
		PK_Z = 0x0200,  /* z axis */
		PK_NORMAL_PRESSURE = 0x0400,    /* normal or tip pressure */
		PK_TANGENT_PRESSURE = 0x0800,   /* tangential or barrel pressure */
		PK_ORIENTATION = 0x1000,    /* orientation info: tilts */
		PK_ROTATION = 0x2000,   /* rotation info */
		PK_PKTBITS_ALL = 0x3FFF    // The Full Monty - all the bits
	}

	/// <summary>
	/// Wintab event messsages sent to an application.
	/// See Wintab Spec 1.4 for a description of these messages.
	/// </summary>
	public enum EWintabEventMessage {
		WT_PACKET = 0x7FF0,
		WT_CTXOPEN = 0x7FF1,
		WT_CTXCLOSE = 0x7FF2,
		WT_CTXUPDATE = 0x7FF3,
		WT_CTXOVERLAP = 0x7FF4,
		WT_PROXIMITY = 0x7FF5,
		WT_INFOCHANGE = 0x7FF6,
		WT_CSRCHANGE = 0x7FF7,
		WT_PACKETEXT = 0x7FF8
	}

	/// <summary>
	/// Wintab packet status values.
	/// </summary>
	public enum EWintabPacketStatusValue {
		/// <summary>
		/// Specifies that the cursor is out of the context.
		/// </summary>
		TPS_PROXIMITY = 0x0001,

		/// <summary>
		/// Specifies that the event queue for the context has overflowed.
		/// </summary>
		TPS_QUEUE_ERR = 0x0002,

		/// <summary>
		/// Specifies that the cursor is in the margin of the context.
		/// </summary>
		TPS_MARGIN = 0x0004,

		/// <summary>
		/// Specifies that the cursor is out of the context, but that the 
		/// context has grabbed input while waiting for a button release event.
		/// </summary>
		TPS_GRAB = 0x0008,

		/// <summary>
		/// Specifies that the cursor is in its inverted state.
		/// </summary>
		TPS_INVERT = 0x0010
	}

	/// <summary>
	/// WintabPacket.pkButton codes.
	/// </summary>
	public enum EWintabPacketButtonCode {
		/// <summary>
		/// No change in button state.
		/// </summary>
		TBN_NONE = 0,

		/// <summary>
		/// Button was released.
		/// </summary>
		TBN_UP = 1,

		/// <summary>
		/// Button was pressed.
		/// </summary>
		TBN_DOWN = 2
	}

	/// <summary>
	/// Pen normal presure (PK_NORMAL_PRESSURE)
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct WTNormalPressure {
		/// <summary>
		/// Tip-button or normal-to-surface relative pressure data.
		/// </summary>
		[FieldOffset(0)]
		public Int32 pkRelativeNormalPressure;        // relative PK_NORMAL_PRESSURE

		/// <summary>
		/// Tip-button or normal-to-surface absolute pressure data.
		/// </summary>
		[FieldOffset(0)]
		public UInt32 pkAbsoluteNormalPressure;       // absolute !PK_NORMAL_PRESSURE
	}

	/// <summary>
	/// Pen tangential pressure (PK_TANGENT_PRESSURE)
	/// </summary>
	[StructLayout(LayoutKind.Explicit)]
	public struct WTTangentPressure {
		/// <summary>
		/// Barrel-button or tangent-to-surface relative pressure data.
		/// </summary>
		[FieldOffset(0)]
		public Int32 pkRelativeTangentPressure;        // relative PK_TANGENT_PRESSURE

		/// <summary>
		/// Barrel-button or tangent-to-surface absolute pressure data.
		/// </summary>
		[FieldOffset(0)]
		public UInt32 pkAbsoluteTangentPressure;       // absolute !PK_TANGENT_PRESSURE
	}


	/// <summary>
	/// Pen Orientation
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct WTOrientation {
		/// <summary>
		/// Specifies the clockwise rotation of the cursor about the 
		/// z axis through a full circular range.
		/// </summary>
		public Int32 orAzimuth;

		/// <summary>
		/// Specifies the angle with the x-y plane through a signed, semicircular range.  
		/// Positive values specify an angle upward toward the positive z axis; negative 
		/// values specify an angle downward toward the negative z axis. 
		/// </summary>
		public Int32 orAltitude;

		/// <summary>
		/// Specifies the clockwise rotation of the cursor about its own major axis.
		/// </summary>
		public Int32 orTwist;
	}

	/// <summary>
	/// Pen Rotation
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct WTRotation {
		/// <summary>
		/// Specifies the pitch of the cursor.
		/// </summary>
		public Int32 rotPitch;

		/// <summary>
		/// Specifies the roll of the cursor. 
		/// </summary>
		public Int32 rotRoll;

		/// <summary>
		/// Specifies the yaw of the cursor.
		/// </summary>
		public Int32 rotYaw;
	}
	/// <summary>
	/// Wintab data packet.  Contains the "Full Monty" for all possible data values.
	/// </summary>
	[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
	public struct WintabPacket {
		/// <summary>
		/// Specifies the context that generated the event.
		/// </summary>
		public HCTX pkContext;                      // PK_CONTEXT

		/// <summary>
		/// Specifies various status and error conditions. These conditions can be 
		/// combined by using the bitwise OR opera-tor. The pkStatus field can be any
		/// any combination of the values defined in EWintabPacketStatusValue.
		/// </summary>
		public UInt32 pkStatus;                     // PK_STATUS

		/// <summary>
		/// In absolute mode, specifies the system time at which the event was posted. In
		/// relative mode, specifies the elapsed time in milliseconds since the last packet.
		/// </summary>
		public UInt32 pkTime;                       // PK_TIME

		/// <summary>
		/// Specifies which of the included packet data items have changed since the 
		/// previously posted event.
		/// </summary>
		public WTPKT pkChanged;                     // PK_CHANGED

		/// <summary>
		/// This is an identifier assigned to the packet by the context. Consecutive 
		/// packets will have consecutive serial numbers.
		/// </summary>
		public UInt32 pkSerialNumber;               // PK_SERIAL_NUMBER

		/// <summary>
		/// Specifies which cursor type generated the packet.
		/// </summary>
		public UInt32 pkCursor;                     // PK_CURSOR

		/// <summary>
		/// In absolute mode, is a UInt32 containing the current button state. 
		/// In relative mode, is a UInt32 whose low word contains a button number, 
		/// and whose high word contains one of the codes in EWintabPacketButtonCode.
		/// </summary>
		public UInt32 pkButtons;                    // PK_BUTTONS

		/// <summary>
		/// In absolute mode, each is a UInt32 containing the scaled cursor location 
		/// along the X axis.  In relative mode, this is an Int32 containing 
		/// scaled change in cursor position.
		/// </summary>
		public Int32 pkX;                           // PK_X

		/// <summary>
		/// In absolute mode, each is a UInt32 containing the scaled cursor location 
		/// along the Y axis.  In relative mode, this is an Int32 containing 
		/// scaled change in cursor position.
		/// </summary>
		public Int32 pkY;                           // PK_Y

		/// <summary>
		/// In absolute mode, each is a UInt32 containing the scaled cursor location 
		/// along the Z axis.  In relative mode, this is an Int32 containing 
		/// scaled change in cursor position.
		/// </summary>
		public Int32 pkZ;                           // PK_Z    

		/// <summary>
		/// In absolute mode, this is a UINT containing the adjusted state  
		/// of the normal pressure, respectively. In relative mode, this is
		/// an int containing the change in adjusted pressure state.
		/// </summary>
		public WTNormalPressure pkNormalPressure;   // PK_NORMAL_PRESSURE

		/// <summary>
		/// In absolute mode, this is a UINT containing the adjusted state  
		/// of the tangential pressure, respectively. In relative mode, this is
		/// an int containing the change in adjusted pressure state.
		/// </summary>
		public WTTangentPressure pkTangentPressure; // PK_TANGENT_PRESSURE

		/// <summary>
		/// Contains updated cursor orientation information. See the 
		/// WTOrientation structure for details.
		/// </summary>
		public WTOrientation pkOrientation;         // ORIENTATION

		/// <summary>
		/// Contains updated cursor rotation information.  See the
		/// WTRotation structure for details.
		/// </summary>
		public WTRotation pkRotation;               // ROTATION
	}

	/// <summary>
	/// Class to support capture and management of Wintab daa.
	/// </summary>
	public class WData {
		private WContext m_context;

		/// <summary>
		/// CWintabData constructor
		/// </summary>
		/// <param name="context_I">logical context for this data object</param>
		public WData(WContext context_I) {
			Init(context_I);
		}

		/// <summary>
		/// Initialize this data object.
		/// </summary>
		/// <param name="context_I">logical context for this data object</param>
		private void Init(WContext context_I) {
			try {
				if (context_I == null) {
					throw new Exception("Trying to init CWintabData with null context.");
				}
				m_context = context_I;

				// Watch for the Wintab WT_PACKET event.
				WMessageEvents.WatchMessage((int)EWintabEventMessage.WT_PACKET);
			} catch (Exception ex) {
				throw new Exception("FAILED CWintabData.Init: " + ex.ToString());
			}
		}

		/// <summary>
		/// Set the handler to be called when WT_PACKET events are received.
		/// </summary>
		/// <param name="handler_I">WT_PACKET event handler supplied by the client.</param>
		public void SetWTPacketEventHandler(EventHandler<MessageReceivedEventArgs> handler_I) {
			WMessageEvents.MessageReceived += handler_I;
		}

		/// <summary>
		/// Set packet queue size for this data object's context.
		/// </summary>
		/// <param name="numPkts_I">desired #packets in queue</param>
		/// <returns>Returns true if operation successful</returns>
		public bool SetPacketQueueSize(UInt32 numPkts_I) {
			bool status = false;

			try {
				CheckForValidHCTX("SetPacketQueueSize");
				status = WNativeMethods.WTQueueSizeSet(m_context.HCtx, numPkts_I);
			} catch (Exception ex) {
				throw new Exception("FAILED SetPacketQueueSize: " + ex.ToString());
			}

			return status;
		}

		/// <summary>
		/// Get packet queue size for this data object's context.
		/// </summary>
		/// <returns>Returns a packet queue size in #packets or 0 if fails</returns>
		public UInt32 GetPacketQueueSize() {
			UInt32 numPkts = 0;

			try {
				CheckForValidHCTX("GetPacketQueueSize");
				numPkts = WNativeMethods.WTQueueSizeGet(m_context.HCtx);
			} catch (Exception ex) {
				throw new Exception("FAILED GetPacketQueueSize: " + ex.ToString());
			}

			return numPkts;
		}


		/// <summary>
		/// Returns one packet of Wintab data from the packet queue.
		/// </summary>
		/// <param name="pktID_I">Identifier for the tablet event packet to return.</param>
		/// <returns>Returns a data packet with non-null context if successful.</returns>
		public WintabPacket GetDataPacket(UInt32 pktID_I) {
			WintabPacket packet = new WintabPacket();

			try {
				bool status = false;

				if (pktID_I == 0) {
					throw new Exception("GetDataPacket - invalid pktID");
				}

				CheckForValidHCTX("GetDataPacket");
				status = WNativeMethods.WTPacket(m_context.HCtx, pktID_I, ref packet);

				// If fails, make sure context is zero.
				if (!status) {
					packet.pkContext = HCTX.Zero;
				}
			} catch (Exception ex) {
				throw new Exception("FAILED GetDataPacket: " + ex.ToString());
			}

			return packet;
		}

		/// <summary>
		/// Returns an array of Wintab data packets from the packet queue.
		/// </summary>
		/// <param name="maxPkts_I">Specifies the maximum number of packets to return.</param>
		/// <param name="remove_I">If true, returns data packets and removes them from the queue.</param>
		/// <param name="numPkts_O">Number of packets actually returned.</param>
		/// <returns>Returns the next maxPkts_I from the list.  Note that if remove_I is false, then 
		/// repeated calls will return the same packets.  If remove_I is true, then packets will be 
		/// removed and subsequent calls will get different packets (if any).</returns>
		public WintabPacket[] GetDataPackets(UInt32 maxPkts_I, bool remove_I, ref UInt32 numPkts_O) {
			WintabPacket[] packets = null;

			try {
				CheckForValidHCTX("GetDataPackets");

				if (maxPkts_I == 0) {
					throw new Exception("GetDataPackets - maxPkts_I is zero.");
				}

				// Packet array is used whether we're just looking or buying.
				int size = (int)(maxPkts_I * Marshal.SizeOf(new WintabPacket()));
				IntPtr buf = WMemUtils.AllocUnmanagedBuf(size);

				if (remove_I) {
					// Return data packets and remove packets from queue.
					numPkts_O = WNativeMethods.WTPacketsGet(m_context.HCtx, maxPkts_I, buf);

					packets = WMemUtils.MarshalDataPackets(numPkts_O, buf);

					//System.Diagnostics.Debug.WriteLine("GetDataPackets: numPkts_O: " + numPkts_O);
				} else {
					// Return data packets, but leave on queue.  (Peek mode)
					UInt32 pktIDOldest = 0;
					UInt32 pktIDNewest = 0;

					// Get oldest and newest packet identifiers in the queue.  These will bound the
					// packets that are actually returned.
					if (WNativeMethods.WTQueuePacketsEx(m_context.HCtx, ref pktIDOldest, ref pktIDNewest)) {
						UInt32 pktIDStart = pktIDOldest;
						UInt32 pktIDEnd = pktIDNewest;

						if (pktIDStart == 0) { throw new Exception("WTQueuePacketsEx reports zero start packet identifier"); }

						if (pktIDEnd == 0) { throw new Exception("WTQueuePacketsEx reports zero end packet identifier"); }

						// Peek up to the max number of packets specified.
						UInt32 numFoundPkts = WNativeMethods.WTDataPeek(m_context.HCtx, pktIDStart, pktIDEnd, maxPkts_I, buf, ref numPkts_O);

						System.Diagnostics.Debug.WriteLine("GetDataPackets: WTDataPeek - numFoundPkts: " + numFoundPkts + ", numPkts_O: " + numPkts_O);

						if (numFoundPkts > 0 && numFoundPkts < numPkts_O) {
							throw new Exception("WTDataPeek reports more packets returned than actually exist in queue.");
						}

						packets = WMemUtils.MarshalDataPackets(numPkts_O, buf);
					}
				}

			} catch (Exception ex) {
				throw new Exception("FAILED GetPacketDataRange: " + ex.ToString());
			}

			return packets;
		}


		/// <summary>
		/// Throws exception if logical context for this data object is zero.
		/// </summary>
		private void CheckForValidHCTX(string msg) {
			if (!m_context.HCtx.IsValid) {
				throw new Exception(msg + " - Bad Context");
			}
		}
	}
}
