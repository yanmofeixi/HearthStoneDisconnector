using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;

namespace HearthStoneDisconnector
{
    class Program
    {
        static void Main(string[] args)
        {
			var pid = getGamePid();
			Console.WriteLine("PID: " + pid);
			var serverIpAddr = getServerIpAddr(pid);
			Console.WriteLine("Server Ip: " + serverIpAddr);
			Console.WriteLine("Disconnecting..");
			Disconnecter.CloseRemoteIP(serverIpAddr);
		}

		static string getGamePid()
        {
			var a = Process.GetProcesses().Where(p => p.ProcessName == "Hearthstone").ToList();
			if(a.Count == 1)
            {
				return a.FirstOrDefault().Id.ToString();
			}
			Console.WriteLine("Cannot find process");
			return "x";
		}

        static string getServerIpAddr(string pid)
        {
			List<string> list = new List<string>
			{
				"127.0.0.1"
			};
			list.AddRange(GetHostAddresses("telemetry-in.battle.net"));
			list.AddRange(GetHostAddresses("cn.actual.battle.net"));
			list.AddRange(GetHostAddresses("gateway.battlenet.com.cn"));
			list.AddRange(GetHostAddresses("us.actual.battle.net"));
			list.AddRange(GetHostAddresses("eu.actual.battle.net"));
			list.AddRange(GetHostAddresses("gateway.battlenet.com"));
			try
			{
				using (Process process = new Process())
				{
					process.StartInfo.FileName = "cmd.exe";
					process.StartInfo.UseShellExecute = false;
					process.StartInfo.RedirectStandardInput = true;
					process.StartInfo.RedirectStandardOutput = true;
					process.StartInfo.CreateNoWindow = true;
					process.Start();
					process.StandardInput.WriteLine("netstat -ano|findstr " + pid);
					process.StandardInput.WriteLine("exit");
					string[] array = Regex.Split(process.StandardOutput.ReadToEnd(), "\r\n");
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = Regex.Split(array[i], "\\s+");
						if (array2.Length > 5 && array2[1] == "TCP" && array2[4] == "ESTABLISHED")
						{
							string[] array3 = array2[3].Split(new char[]
							{
								':'
							});
							string text = array3[0];
							if (array3[1] != "443" && !list.Contains(text))
							{
								return text;
							}
						}
					}
				}
			}
			catch (Exception)
			{
			}
			return null;
		}

		static List<string> GetHostAddresses(string host)
		{
			List<string> list = new List<string>();
			try
			{
				var hostAddrs = Dns.GetHostAddresses(host).Select(x => x.ToString());
/*				foreach(var a in hostAddrs)
                {
					Console.WriteLine(host + " --> " + a);
                }*/
				list.AddRange(hostAddrs);
			}
			catch (Exception)
			{
			}
			return list;
		}
	}
}
