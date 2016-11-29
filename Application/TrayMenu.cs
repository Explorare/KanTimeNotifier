using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using App = System.Windows.Forms.Application;
using File = System.IO.File;
using System.Diagnostics;
using RES = KanTimeNotifier.Properties.Resources;
using System.Threading;

namespace KanTimeNotifier
{
	class TrayMenu
	{
		public readonly ContextMenu Menu;
		private int debug_voice_id;

		Process configProcess;
		DateTime configLastWrite;
		string configFile = App.StartupPath + "\\" + Program._FriendlyName + ".ini";
		System.Threading.Timer timer;

		public TrayMenu()
		{
			this.Menu = new ContextMenu();
		}

		public void Initialize()
		{
			debug_voice_id = Settings.Current._VoiceId;
			if (debug_voice_id > 0)
			{
				Menu.MenuItems.Add(
					new MenuItem("开始测试", _ClickToDebug));
			}

			Menu.MenuItems.Add(
				new MenuItem("启动游戏", _ClickToStartGame)
					);

			Menu.MenuItems.Add(
				new MenuItem("编辑设置", _ClickToEditConfig)
					);

			Menu.MenuItems.Add(
				new MenuItem("阅读说明", _ClickToViewHelp)
					);

			Menu.MenuItems.Add(
				new MenuItem("重新加载", _ClickToReload)
					);

			Menu.MenuItems.Add(
				new MenuItem("退出程序",	_ClickToExit)
					);

			var item = new MenuItem();

		}

		public void Finish()
		{
			Menu.Dispose();
			_DisposeTimer();
		}

		void _ClickToDebug(object sender, EventArgs e)
		{
			Program._Core.VoicePlay(debug_voice_id);
			debug_voice_id = debug_voice_id % 53 + 1;
			(sender as MenuItem).Text = "测试：" + debug_voice_id.ToString();
		}

		void _ClickToStartGame(object sender, EventArgs e)
		{
			var si = new ProcessStartInfo();
			si.FileName = Settings.Current.StartGamePath;
			si.UseShellExecute = true;
			Process.Start(si);
		}

		void _ClickToReload(object sender, EventArgs e)
		{
			Program.Reload();
		}

		void _ClickToViewHelp(object sender, EventArgs e)
		{
			var filepath = App.StartupPath + "\\" + Program._FriendlyName + "的说明.txt";
			if (!File.Exists(filepath))
				File.WriteAllText(filepath, RES.Help, Encoding.Default);

			var si = new ProcessStartInfo();
			si.FileName = filepath;
			si.UseShellExecute = true;
			Process.Start(si);
		}

		void _ClickToEditConfig(object sender, EventArgs e)
		{	
			var filepath = configFile;
			if (File.Exists(filepath))
			{
				if (configProcess == null)
				{
					configProcess = Process.Start(
						new ProcessStartInfo()
						{
							FileName = filepath,
							UseShellExecute = true,
						});
				}
				else
				{
					configProcess.Start();
				}

				Program._Icon.ShowMessage("新的设置将在设置文件关闭后生效。若未生效，则请点击菜单项「重新加载」。");

				configLastWrite = File.GetLastWriteTime(filepath);
				_DisposeTimer();
				_StartTimer(200);
			}
			else
			{
				//即使文件不存在也不再生成了
				Program._Icon.ShowMessage("奇迹发生了，设置文档不见了！\r\n" + filepath);
			}
		}

		void _StartTimer(int period)
		{
			timer = new System.Threading.Timer(_TimerProc);
			timer.Change(0, period);
		}

		void _TimerProc(object state)
		{
			if (configProcess != null && !configProcess.HasExited)
			{
				return;
			}

			(state as System.Threading.Timer).Dispose();

			if (File.Exists(configFile) && configLastWrite != File.GetLastWriteTime(configFile))
			{
				Program.Reload();
			}

		}

		void _DisposeTimer()
		{
			if (timer != null) timer.Dispose();
		}

		void _ClickToExit (object sender, EventArgs e)
		{
			Application.ExitThread();
		}
		
		
	}
}
