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

		// Token: 0x0600012B RID: 299 RVA: 0x00008304 File Offset: 0x00006504
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

		// Token: 0x0600012C RID: 300 RVA: 0x00008420 File Offset: 0x00006620
		private static IntPtr GetPtrToNewObject(object obj)
		{
			IntPtr intPtr = Marshal.AllocCoTaskMem(Marshal.SizeOf(obj));
			Marshal.StructureToPtr(obj, intPtr, false);
			return intPtr;
		}

		// Token: 0x0600012D RID: 301 RVA: 0x00008444 File Offset: 0x00006644
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
			// Token: 0x04000EEC RID: 3820
			All,
			// Token: 0x04000EED RID: 3821
			Closed,
			// Token: 0x04000EEE RID: 3822
			Listen,
			// Token: 0x04000EEF RID: 3823
			Syn_Sent,
			// Token: 0x04000EF0 RID: 3824
			Syn_Rcvd,
			// Token: 0x04000EF1 RID: 3825
			Established,
			// Token: 0x04000EF2 RID: 3826
			Fin_Wait1,
			// Token: 0x04000EF3 RID: 3827
			Fin_Wait2,
			// Token: 0x04000EF4 RID: 3828
			Close_Wait,
			// Token: 0x04000EF5 RID: 3829
			Closing,
			// Token: 0x04000EF6 RID: 3830
			Last_Ack,
			// Token: 0x04000EF7 RID: 3831
			Time_Wait,
			// Token: 0x04000EF8 RID: 3832
			Delete_TCB
		}

		private struct MIB_TCPROW
		{
			// Token: 0x04000EF9 RID: 3833
			public int dwState;

			// Token: 0x04000EFA RID: 3834
			public int dwLocalAddr;

			// Token: 0x04000EFB RID: 3835
			public int dwLocalPort;

			// Token: 0x04000EFC RID: 3836
			public int dwRemoteAddr;

			// Token: 0x04000EFD RID: 3837
			public int dwRemotePort;
		}
	}
}
