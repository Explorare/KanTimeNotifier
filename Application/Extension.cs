using System;
using System.IO;
using Dir = System.IO.Directory;

namespace KanTimeNotifier
{
	#region 保留的代码
	//static public class NotifyIconExtension
	//{
	//	static int TimeOut = 5000;
	//	static string TipTitle;

	//	static public void SetTimeOut(this NotifyIcon notifyIcon, int timeout)
	//	{
	//		TimeOut = timeout;
	//	}

	//	#region 显示消息、警告、错误
	//	static public void Show(this NotifyIcon notifyIcon)
	//	{
	//		notifyIcon.ShowBalloonTip(TimeOut);
	//	}

	//	static public void ShowMessage(this NotifyIcon notifyIcon, string title, string text, bool showIcon = true)
	//	{
	//		notifyIcon.ShowBalloonTip(TimeOut, title, text, showIcon ? ToolTipIcon.Info : ToolTipIcon.None);
	//	}

	//	static public void ShowMessage(this NotifyIcon notifyIcon, string text, bool showIcon = true)
	//	{
	//		notifyIcon.ShowBalloonTip(TimeOut, TipTitle, text, showIcon ? ToolTipIcon.Info : ToolTipIcon.None);
	//	}

	//	static public void ShowWarning(this NotifyIcon notifyIcon, string title, string text)
	//	{
	//		notifyIcon.ShowBalloonTip(TimeOut, title, text, ToolTipIcon.Warning);
	//	}

	//	static public void ShowWarning(this NotifyIcon notifyIcon, string text)
	//	{
	//		notifyIcon.ShowBalloonTip(TimeOut, TipTitle, text, ToolTipIcon.Warning);
	//	}

	//	static public void ShowError(this NotifyIcon notifyIcon, string title, string text)
	//	{
	//		notifyIcon.ShowBalloonTip(TimeOut, title, text, ToolTipIcon.Error);
	//	}

	//	static public void ShowError(this NotifyIcon notifyIcon, string text)
	//	{
	//		notifyIcon.ShowBalloonTip(TimeOut, TipTitle, text, ToolTipIcon.Error);
	//	}
	//	#endregion

	//	static public void DefaultInit(this NotifyIcon notifyIcon, Ship shipInfo)
	//	{
	//		notifyIcon.Icon = KanTimeNotifier.Properties.Resources._16;

	//		bool _loc1 = !String.IsNullOrEmpty(shipInfo.CV);
	//		bool _loc2 = !String.IsNullOrEmpty(shipInfo.ShipType);

	//		if (_loc1)
	//		{
	//			notifyIcon.Text = String.Format("{0}（CV：{1}）", shipInfo.Name, shipInfo.CV);
	//		}
	//		else if (_loc2)
	//		{
	//			notifyIcon.Text = String.Format("{1}「{0}」", shipInfo.Name, shipInfo.ShipType);
	//		}
	//		else
	//		{
	//			notifyIcon.Text = shipInfo.Name;
	//		}

	//		notifyIcon.BalloonTipTitle = TipTitle = shipInfo.Name;
			
	//		var menu = new ContextMenu();
			

	//		_voice_id = Settings.Current._voice_id;
	//		if (_voice_id>0)
	//		{
	//			menu.MenuItems.Add(new MenuItem("测试",
	//				new EventHandler(
	//						(s, e) =>
	//						{
	//							Program.ProgramCore.DownloadAndPlay(_voice_id);
	//							_voice_id = _voice_id % 53 + 1;
	//						}
	//					)));
	//		}

	//		menu.MenuItems.Add(new MenuItem("退出",
	//				new EventHandler((s, e) => Application.ExitThread())
	//					));

	//		notifyIcon.ContextMenu = menu;

	//		notifyIcon.ShowIcon();
	//		//notifyIcon.ShowBalloonTip(TimeOut);
	//	}

	//	static int _voice_id = 29;

	//	#region 显示/隐藏图标
	//	static public void ShowIcon(this NotifyIcon notifyIcon)
	//	{
	//		notifyIcon.Visible = true;
	//	}

	//	static public void HideIcon(this NotifyIcon notifyIcon)
	//	{
	//		notifyIcon.Visible = false;
	//	}
	//	#endregion

	//}
	#endregion

	static public class DirectoryEx
	{
		static public bool CreateFolderTree(string path, bool isfile = false)
		{
			string[] sps = null;
			int len = 0;


			if (isfile)
			{
				path = Path.GetDirectoryName(path);
			}
			
			if (Dir.Exists(path)) return true;
			sps = path.Replace('/', '\\').Split('\\');
			len = sps.Length;

			bool ok = false;
			int ret = 0;

			while (!ok&& len>=1)
			{
				try
				{
					Dir.CreateDirectory(path);
					ok = true;
				}
				catch (DirectoryNotFoundException)
				{
					len--;
					ret++;
					path = String.Join("\\", sps, 0, len);
				}
				catch (Exception)
				{
					System.Diagnostics.Debug.WriteLine("CDir: len,ret={0},{1} {2}", len, ret, path);
					return false;
				}
			}

			if (!ok) return false; //不会出现

			while (ret > 0)
			{
				ret--;
				len++;
				path = String.Join("\\", sps, 0, len);
				try { Dir.CreateDirectory(path); }
				catch {
					System.Diagnostics.Debug.WriteLine("CDir: len,ret={0},{1} {2}", len, ret, path);
					return false;
				}
			}
			return true;
		}
		

	}
}
