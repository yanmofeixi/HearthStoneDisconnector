using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace HearthStoneDisconnector
{
	public class Disconnecter
	{
		[DllImport("iphlpapi.dll")]
		private static extern int GetTcpTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder);

		[DllImport("iphlpapi.dll")]
		private static extern int SetTcpEntry(IntPtr pTcprow);

		public static void CloseRemoteIP(string IP)
		{
			Disconnecter.MIB_TCPROW[] tcpTable = Disconnecter.getTcpTable();
			for (int i = 0; i < tcpTable.Length; i++)
			{
				if (tcpTable[i].dwRemoteAddr == Disconnecter.IPStringToInt(IP))
				{
					tcpTable[i].dwState = 12;
					Disconnecter.SetTcpEntry(Disconnecter.GetPtrToNewObject(tcpTable[i]));
				}
			}
		}

		private static Disconnecter.MIB_TCPROW[] getTcpTable()
		{
			IntPtr intPtr = IntPtr.Zero;
			bool flag = false;
			Disconnecter.MIB_TCPROW[] result;
			try
			{
				int cb = 0;
				Disconnecter.GetTcpTable(IntPtr.Zero, ref cb, false);
				intPtr = Marshal.AllocCoTaskMem(cb);
				flag = true;
				Disconnecter.GetTcpTable(intPtr, ref cb, false);
				int num = Marshal.ReadInt32(intPtr);
				IntPtr intPtr2 = (IntPtr)((int)intPtr + 4);
				Disconnecter.MIB_TCPROW[] array = new Disconnecter.MIB_TCPROW[num];
				int num2 = Marshal.SizeOf<Disconnecter.MIB_TCPROW>(default(Disconnecter.MIB_TCPROW));
				for (int i = 0; i < num; i++)
				{
					array[i] = (Disconnecter.MIB_TCPROW)Marshal.PtrToStructure(intPtr2, typeof(Disconnecter.MIB_TCPROW));
					intPtr2 = (IntPtr)((int)intPtr2 + num2);
				}
				result = array;
			}
			catch (Exception ex)
			{
				throw new Exception(string.Concat(new string[]
				{
					"getTcpTable failed! [",
					ex.GetType().ToString(),
					",",
					ex.Message,
					"]"
				}));
			}
			finally
			{
				if (flag)
				{
					Marshal.FreeCoTaskMem(intPtr);
				}
			}
			return result;
		}

		private static IntPtr GetPtrToNewObject(object obj)
		{
			IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(obj));
			Marshal.StructureToPtr(obj, intPtr, false);
			return intPtr;
		}

		private static int IPStringToInt(string IP)
		{
			if (IP.IndexOf(".") < 0)
			{
				throw new Exception("Invalid IP address");
			}
			string[] array = IP.Split(new char[]
			{
				'.'
			});
			if (array.Length != 4)
			{
				throw new Exception("Invalid IP address");
			}
			return BitConverter.ToInt32(new byte[]
			{
				byte.Parse(array[0]),
				byte.Parse(array[1]),
				byte.Parse(array[2]),
				byte.Parse(array[3])
			}, 0);
		}

		public enum State
		{
			All,
			Closed,
			Listen,
			Syn_Sent,
			Syn_Rcvd,
			Established,
			Fin_Wait1,
			Fin_Wait2,
			Close_Wait,
			Closing,
			Last_Ack,
			Time_Wait,
			Delete_TCB
		}

		private struct MIB_TCPROW
		{
			public int dwState;
			public int dwLocalAddr;
			public int dwLocalPort;
			public int dwRemoteAddr;
			public int dwRemotePort;
		}
	}
}
