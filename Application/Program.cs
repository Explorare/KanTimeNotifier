using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using App = System.Windows.Forms.Application;
using Path = System.IO.Path;
using System.Diagnostics;

namespace KanTimeNotifier
{
	static class Program
	{
		static public ProgramCore _Core;
		static public TrayIcon _Icon;
		static public TrayMenu _Menu;
		static public Settings _Sets;
		static public string _FriendlyName;
		/// <summary>
		/// 应用程序的主入口点。
		/// </summary>
		[STAThread]
		static void Main()
		{
			AppDomain.CurrentDomain.UnhandledException += HandleException;
			
			_FriendlyName= Path.GetFileNameWithoutExtension(App.ExecutablePath);

			//检查重复进程
			Process[] ktrPrcs = Process.GetProcessesByName(_FriendlyName);
			if (ktrPrcs.Count() > 1)
			{
				MessageBox.Show(_FriendlyName + ".exe 已经启动，请不要重复运行。", App.ProductName, 
					MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
				return;
			}
			
			App.SetCompatibleTextRenderingDefault(false);
			App.EnableVisualStyles();
			
			Initialize();

			App.Run();
		}

		static void Initialize()
		{
			Settings.Initialize(_FriendlyName + ".ini");
			_Sets = Settings.Current;

			_Menu = new TrayMenu();
			_Menu.Initialize();

			_Icon = new TrayIcon();
			_Icon.Initialize();
			_Icon._NotifyIcon.ContextMenu = _Menu.Menu;

			_Core = new ProgramCore();
			_Core.Initialize();


			_Core.VoicePlay(1);
			//_Icon.ShowMessage(_Sets.OnLoadText);

			App.ApplicationExit += OnApplicationExit;
		}

		static public void Reload()
		{
			Settings.Load(_FriendlyName + ".ini");
			_Sets = Settings.Current;
			
			//Menu需要清除之前添加的菜单项
			_Menu.Menu.MenuItems.Clear();
			_Menu.Initialize();

			//Icon需要没有什么要清除的
			_Icon.Initialize();

			//Core需要删除之前的临时文件
			//Core的成员对象全部new了，但是安全的
			_Core.Finish();
			_Core.Initialize();

			_Core.VoicePlay(1);
			//_Icon.ShowMessage(_Sets.OnLoadText);
		}

		static void OnApplicationExit(object sender, EventArgs e)
		{
			_Core.Finish();
			_Icon.Finish();
			_Menu.Finish();
		}

		static void HandleException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = e.ExceptionObject as Exception;

			System.IO.File.AppendAllText("error.log", String.Format(
@"
EXCEPTION: {0}
========================================================
> 异常来源 = {1}
> 终止程序 = {2}
> 描述信息 = {3} 
{4}
--------------------------------------------------------

", DateTime.Now, ex.Source, e.IsTerminating, ex.Message, ex.InnerException));

			if (_Icon != null&& _Icon._NotifyIcon != null)
			{
				_Icon._NotifyIcon.ShowBalloonTip(2000, App.ProductName + "发生异常",
							String.Format("描述：{0}\r\n详细见{1}\\error.log", ex.Message, App.StartupPath),
							ToolTipIcon.Error);
			}
			else
			{
				MessageBox.Show(String.Format("程序发生未知异常：\r\n{0}\r\n另见{1}\\error.log", ex, App.StartupPath), 
					App.ProductName);
			}
		}

		
	}
}
