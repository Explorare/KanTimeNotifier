using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Icon = System.Drawing.Icon;
using App = System.Windows.Forms.Application;

namespace KanTimeNotifier
{
	public class TrayIcon
	{
		readonly public NotifyIcon _NotifyIcon;
		int TimeOut;
		string ToolTitle;
		bool ToolTipDiabled;
		
		public TrayIcon ()
		{
			_NotifyIcon = new NotifyIcon();
			_NotifyIcon.MouseClick +=OnMouseClickAction;
		}



		#region 显示消息、警告、错误
		public void Show()
		{
			if (ToolTipDiabled) return;
			_NotifyIcon.ShowBalloonTip(TimeOut);
		}

		public void ShowBalloonTip(int timeout,string tipTitle, string tipText, ToolTipIcon tipIcon)
		{
			if (ToolTipDiabled) return;
			_NotifyIcon.ShowBalloonTip(timeout, tipTitle, tipText, tipIcon);
		}

		public void ShowMessage(string title, string text, bool showIcon = true)
		{
			this.ShowBalloonTip(TimeOut, title, text, showIcon ? ToolTipIcon.Info : ToolTipIcon.None);
		}

		public void ShowMessage(string text, bool showIcon = true)
		{
			this.ShowBalloonTip(TimeOut, ToolTitle, text, showIcon ? ToolTipIcon.Info : ToolTipIcon.None);
		}

		public void ShowWarning(string title, string text)
		{
			this.ShowBalloonTip(TimeOut, title, text, ToolTipIcon.Warning);
		}

		public void ShowWarning(string text)
		{
			this.ShowBalloonTip(TimeOut, ToolTitle, text, ToolTipIcon.Warning);
		}

		public void ShowError(string title, string text)
		{
			this.ShowBalloonTip(TimeOut, title, text, ToolTipIcon.Error);
		}

		public void ShowError(string text)
		{
			this.ShowBalloonTip(TimeOut, ToolTitle, text, ToolTipIcon.Error);
		}
		#endregion

		public void Initialize()
		{
			_NotifyIcon.Icon = KanTimeNotifier.Properties.Resources._16;
			TimeOut = 10000;

			Settings _sets = Settings.Current;
			//Settings必须已经初始化

			ToolTipDiabled = _sets.ToolTipDisabled >= 2;

			_NotifyIcon.Text = _sets.IconText;
			
			_NotifyIcon.BalloonTipTitle = ToolTitle = _sets.ToolTipTitle;

			_NotifyIcon.Visible = true;
		}

		void OnMouseClickAction(object sender, MouseEventArgs e)
		{
			if (e.Button != MouseButtons.Left) return;

			if (DateTime.Now.Minute == 0)
			{
				Program._Core.VoicePlay(DateTime.Now.Hour + 30);
				//if (Settings.Current.ToolTipDisabled < 1)
				//	ShowMessage(String.Format(Settings.Current.OnTimeText, DateTime.Now));
			}
			else
				Program._Core.VoicePlay(DateTime.Now.Second % 3 + 2);
		}

		public void Finish()
		{
			_NotifyIcon.Dispose();
		}

		#region 显示/隐藏图标
		public void IconShow()
		{
			_NotifyIcon.Visible = true;
		}

		public void IconHide()
		{
			_NotifyIcon.Visible = false;
		}
		#endregion

	}
}
