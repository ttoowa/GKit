using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace WintabDN {
	/// <summary>
	/// Class to access Wintab interface data.
	/// </summary>
	public class WInfo {
		private const int MAX_STRING_SIZE = 256;

		/// <summary>
		/// Returns TRUE if Wintab service is running and responsive.
		/// </summary>
		/// <returns></returns>
		public static bool IsWintabAvailable() {
			IntPtr buf = IntPtr.Zero;
			bool status = false;

			status = (WFuncs.WTInfoA(0, 0, buf) > 0);

			return status;
		}

		/// <summary>
		/// Returns a string containing device name.
		/// </summary>
		/// <returns></returns>
		public static String GetDeviceInfo() {
			string devInfo = null;
			IntPtr buf = WMemoryUtility.AllocUnmanagedBuf(MAX_STRING_SIZE);

			int size = (int)WFuncs.WTInfoA(
				(uint)EWTICategoryIndex.WTI_DEVICES,
				(uint)EWTIDevicesIndex.DVC_NAME, buf);

			if (size < 1) {
				throw new Exception("GetDeviceInfo returned empty string.");
			}

			// Strip off final null character before marshalling.
			devInfo = WMemoryUtility.MarshalUnmanagedString(buf, size - 1);

			WMemoryUtility.FreeUnmanagedBuf(buf);
			return devInfo;
		}

		/// <summary>
		/// Returns the default digitizing context, with useful context overrides. 
		/// </summary>
		/// <param name="options_I">caller's options; OR'd into context options</param>
		/// <returns>A valid context object or null on error.</returns>
		public static WContext GetDefaultDigitizingContext(ECTXOptionValues options_I = 0) {
			// Send all possible data bits (not including extended data).
			// This is redundant with CWintabContext initialization, which
			// also inits with PK_PKTBITS_ALL.
			uint PACKETDATA = (uint)EWintabPacketBit.PK_PKTBITS_ALL;  // The Full Monty
			uint PACKETMODE = (uint)EWintabPacketBit.PK_BUTTONS;

			WContext context = GetDefaultContext(EWTICategoryIndex.WTI_DEFCONTEXT);

			if (context != null) {
				// Add digitizer-specific context tweaks.
				context.PktMode = 0;        // all data in absolute mode (set EWintabPacketBit bit(s) for relative mode)
				context.SysMode = false;    // system cursor tracks in absolute mode (zero)

				// Add caller's options.
				context.Options |= (uint)options_I;

				// Set the context data bits.
				context.PktData = PACKETDATA;
				context.PktMode = PACKETMODE;
				context.MoveMask = PACKETDATA;
				context.BtnUpMask = context.BtnDnMask;
			}

			return context;
		}



		/// <summary>
		/// Returns the default system context, with useful context overrides.
		/// </summary>
		/// <param name="options_I">caller's options; OR'd into context options</param>
		/// <returns>A valid context object or null on error.</returns>
		public static WContext GetDefaultSystemContext(ECTXOptionValues options_I = 0) {
			// Send all possible data bits (not including extended data).
			// This is redundant with CWintabContext initialization, which
			// also inits with PK_PKTBITS_ALL.
			uint PACKETDATA = (uint)EWintabPacketBit.PK_PKTBITS_ALL;  // The Full Monty
			uint PACKETMODE = (uint)EWintabPacketBit.PK_BUTTONS;

			WContext context = GetDefaultContext(EWTICategoryIndex.WTI_DEFSYSCTX);

			if (context != null) {
				// TODO: Add system-specific context tweaks.

				// Add caller's options.
				context.Options |= (uint)options_I;

				// Make sure we get data packet messages.
				context.Options |= (uint)ECTXOptionValues.CXO_MESSAGES;

				// Set the context data bits.
				context.PktData = PACKETDATA;
				context.PktMode = PACKETMODE;
				context.MoveMask = PACKETDATA;
				context.BtnUpMask = context.BtnDnMask;
			}

			return context;
		}

		/// <summary>
		/// Helper function to get digitizing or system default context.
		/// </summary>
		/// <param name="contextType_I">Use WTI_DEFCONTEXT for digital context or WTI_DEFSYSCTX for system context</param>
		/// <returns>Returns the default context or null on error.</returns>
		private static WContext GetDefaultContext(EWTICategoryIndex contextIndex_I) {
			WContext context = new WContext();
			IntPtr buf = WMemoryUtility.AllocUnmanagedBuf(context.LogContext);

			int size = (int)WFuncs.WTInfoA((uint)contextIndex_I, 0, buf);

			context.LogContext = WMemoryUtility.MarshalUnmanagedBuf<WintabLogContext>(buf, size);

			WMemoryUtility.FreeUnmanagedBuf(buf);

			return context;
		}

		/// <summary>
		/// Returns the default device.  If this value is -1, then it also known as a "virtual device".
		/// </summary>
		/// <returns></returns>
		public static int GetDefaultDeviceIndex() {
			int devIndex = 0;
			IntPtr buf = WMemoryUtility.AllocUnmanagedBuf(devIndex);

			int size = (int)WFuncs.WTInfoA(
				(uint)EWTICategoryIndex.WTI_DEFCONTEXT,
				(uint)EWTIContextIndex.CTX_DEVICE, buf);

			devIndex = WMemoryUtility.MarshalUnmanagedBuf<int>(buf, size);

			WMemoryUtility.FreeUnmanagedBuf(buf);

			return devIndex;
		}

		/// <summary>
		/// Returns the WintabAxis object for specified device and dimension.
		/// </summary>
		/// <param name="devIndex_I">Device index (-1 = virtual device)</param>
		/// <param name="dim_I">Dimension: AXIS_X, AXIS_Y or AXIS_Z</param>
		/// <returns></returns>
		public static WintabAxis GetDeviceAxis(int devIndex_I, EAxisDimension dim_I) {
			WintabAxis axis = new WintabAxis();
			IntPtr buf = WMemoryUtility.AllocUnmanagedBuf(axis);

			int size = (int)WFuncs.WTInfoA(
				(uint)(EWTICategoryIndex.WTI_DEVICES + devIndex_I),
				(uint)dim_I, buf);

			// If size == 0, then returns a zeroed struct.
			axis = WMemoryUtility.MarshalUnmanagedBuf<WintabAxis>(buf, size);

			WMemoryUtility.FreeUnmanagedBuf(buf);

			return axis;
		}

		/// <summary>
		/// Returns a 3-element array describing the tablet's orientation range and resolution capabilities.
		/// </summary>
		/// <returns></returns>
		public static WintabAxisArray GetDeviceOrientation(out bool tiltSupported_O) {
			WintabAxisArray axisArray = new WintabAxisArray();
			tiltSupported_O = false;
			IntPtr buf = WMemoryUtility.AllocUnmanagedBuf(axisArray);

			int size = (int)WFuncs.WTInfoA(
				(uint)EWTICategoryIndex.WTI_DEVICES,
				(uint)EWTIDevicesIndex.DVC_ORIENTATION, buf);

			// If size == 0, then returns a zeroed struct.
			axisArray = WMemoryUtility.MarshalUnmanagedBuf<WintabAxisArray>(buf, size);
			tiltSupported_O = (axisArray.array[0].axResolution != 0 && axisArray.array[1].axResolution != 0);

			WMemoryUtility.FreeUnmanagedBuf(buf);

			return axisArray;
		}


		/// <summary>
		/// Returns a 3-element array describing the tablet's rotation range and resolution capabilities
		/// </summary>
		/// <returns></returns>
		public static WintabAxisArray GetDeviceRotation(out bool rotationSupported_O) {
			WintabAxisArray axisArray = new WintabAxisArray();
			rotationSupported_O = false;
			IntPtr buf = WMemoryUtility.AllocUnmanagedBuf(axisArray);

			int size = (int)WFuncs.WTInfoA(
				(uint)EWTICategoryIndex.WTI_DEVICES,
				(uint)EWTIDevicesIndex.DVC_ROTATION, buf);

			// If size == 0, then returns a zeroed struct.
			axisArray = WMemoryUtility.MarshalUnmanagedBuf<WintabAxisArray>(buf, size);
			rotationSupported_O = (axisArray.array[0].axResolution != 0 && axisArray.array[1].axResolution != 0);

			WMemoryUtility.FreeUnmanagedBuf(buf);

			return axisArray;
		}

		/// <summary>
		/// Returns the number of devices connected.
		/// </summary>
		/// <returns></returns>
		public static uint GetNumberOfDevices() {
			uint numDevices = 0;
			IntPtr buf = WMemoryUtility.AllocUnmanagedBuf(numDevices);
			int size = (int)WFuncs.WTInfoA(
				(uint)EWTICategoryIndex.WTI_INTERFACE,
				(uint)EWTIInterfaceIndex.IFC_NDEVICES, buf);

			numDevices = WMemoryUtility.MarshalUnmanagedBuf<uint>(buf, size);

			WMemoryUtility.FreeUnmanagedBuf(buf);

			return numDevices;
		}

		/// <summary>
		/// Returns whether a stylus is currently connected to the active cursor.
		/// </summary>
		/// <returns></returns>
		public static bool IsStylusActive() {
			bool isStylusActive = false;
			IntPtr buf = WMemoryUtility.AllocUnmanagedBuf(isStylusActive);

			int size = (int)WFuncs.WTInfoA(
				(uint)EWTICategoryIndex.WTI_INTERFACE,
				(uint)EWTIInterfaceIndex.IFC_NDEVICES, buf);

			isStylusActive = WMemoryUtility.MarshalUnmanagedBuf<bool>(buf, size);

			WMemoryUtility.FreeUnmanagedBuf(buf);

			return isStylusActive;
		}


		/// <summary>
		/// Returns a string containing the name of the selected stylus. 
		/// </summary>
		/// <param name="index_I">indicates stylus type</param>
		/// <returns></returns>
		public static string GetStylusName(EWTICursorNameIndex index_I) {
			string stylusName = null;
			IntPtr buf = WMemoryUtility.AllocUnmanagedBuf(MAX_STRING_SIZE);

			int size = (int)WFuncs.WTInfoA(
				(uint)index_I,
				(uint)EWTICursorsIndex.CSR_NAME, buf);

			if (size < 1) {
				return "Empty";
			}

			// Strip off final null character before marshalling.
			stylusName = WMemoryUtility.MarshalUnmanagedString(buf, size - 1);

			WMemoryUtility.FreeUnmanagedBuf(buf);

			return stylusName;
		}



		/// <summary>
		/// Return max normal pressure supported by tablet.
		/// </summary>
		/// <param name="getNormalPressure_I">TRUE=> normal pressure; 
		/// FALSE=> tangential pressure (not supported on all tablets)</param>
		/// <returns>maximum pressure value or zero on error</returns>
		public static int GetMaxPressure(bool getNormalPressure_I = true) {
			WintabAxis pressureAxis = new WintabAxis();
			IntPtr buffer = WMemoryUtility.AllocUnmanagedBuf(pressureAxis);

			EWTIDevicesIndex devIdx = (getNormalPressure_I ?
				EWTIDevicesIndex.DVC_NPRESSURE : EWTIDevicesIndex.DVC_TPRESSURE);

			int size = (int)WFuncs.WTInfoA(
				(uint)EWTICategoryIndex.WTI_DEVICES,
				(uint)devIdx, buffer);

			pressureAxis = WMemoryUtility.MarshalUnmanagedBuf<WintabAxis>(buffer, size);

			WMemoryUtility.FreeUnmanagedBuf(buffer);

			return pressureAxis.axMax;
		}



		/// <summary>
		/// Return the WintabAxis object for the specified dimension.
		/// </summary>
		/// <param name="dimension_I">Dimension to fetch (eg: x, y)</param>
		/// <returns></returns>
		public static WintabAxis GetTabletAxis(EAxisDimension dimension_I) {
			WintabAxis axis = new WintabAxis();
			IntPtr buf = WMemoryUtility.AllocUnmanagedBuf(axis);

			int size = (int)WFuncs.WTInfoA(
				(uint)EWTICategoryIndex.WTI_DEVICES,
				(uint)dimension_I, buf);

			axis = WMemoryUtility.MarshalUnmanagedBuf<WintabAxis>(buf, size);

			WMemoryUtility.FreeUnmanagedBuf(buf);

			return axis;
		}
	}
}