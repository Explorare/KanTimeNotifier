using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;

namespace KanTimeNotifier
{
	public class ProgramCore
	{
		//static public ProgramCore Current;
		Timer _Timer;
		Settings _Sets;
		Wscript _Vbs;
		WebClient _Client;

		string requiredUrl = "http://125.6.189.167/kcs/sound/kc{0}/{1}.mp3";
		readonly string defaultPath = System.IO.Path.GetTempPath() + "ktnVoice.mp3";
		string lastDownPath;
		string lastRequest;
		
		
		public void Initialize()
		{
			_Sets = Settings.Current;

			//隐藏的设置，URL地址
			if (!String.IsNullOrEmpty(_Sets.URL))
				requiredUrl = _Sets.URL;
			
			//VBS初始化
			_Vbs = new Wscript(System.IO.Path.GetTempPath() + "ktnPlay.vbs")
			{
				WmpVolume = _Sets.Volume,
			};
			
			//Web客户端初始化
			_Client = new WebClient();
			//_Client.DownloadFileCompleted += OnDownloadCompletedAction;

			//计时器初始化
			_Timer = new Timer()
			{
				Interval = 10000,
				Enabled = true,
			};

			_Timer.Tick += OnTimedAction;
			_Timer.Start();
		}

		public void Finish()
		{
			_Timer.Dispose();
			
			if (_Client.IsBusy)
			{
				_Client.CancelAsync();
				if (System.IO.File.Exists(lastDownPath))
					System.IO.File.Delete(lastDownPath);
			}
			_Client.Dispose();
			
			_Vbs.Finish();

			if (_Sets.Download == 1)
			{
				RemoveTempFile();
			}

		}

		public void RemoveTempFile()
		{
				try { System.IO.File.Delete(defaultPath); }
				catch { }
				//for (int i = 2; i <= 4; i++)
				//	try { System.IO.File.Delete(defaultPath + i.ToString() + ".mp3"); }
				//	catch { }
		}

		public async void VoicePlayAsync(int voice_id)
		{
			var fileurl = getDownloadUrl(voice_id);
			var filepath = getDownloadPath(voice_id);
			DirectoryEx.CreateFolderTree(filepath, true);

			//下载文件
			try
			{
				if (_Client.IsBusy)
				{
					_Client.CancelAsync();
					if (System.IO.File.Exists(lastDownPath))
						System.IO.File.Delete(lastDownPath);
				}

				//开始下载
				var uri = new Uri(fileurl);
				var task = _Client.DownloadFileTaskAsync(uri, filepath);

				//消息内容
				string message = getMessageText(voice_id);

				//等待完成
				await task;

				//播放声音
				_Vbs.WmpUrl = filepath;
				_Vbs.PlaySound();

				//显示消息
				if (_Sets.ToolTipDisabled < 1)
					Program._Icon.ShowMessage(message);

				lastDownPath = filepath;
				lastRequest = fileurl;
			}
			catch (WebException ex)
			{
				var respose = ex.Response as HttpWebResponse;
				if (respose == null)
					Program._Icon.ShowError("网络异常", "当前计算机未连接到互联网，请检查互联网的连接状况。");
				else
					Program._Icon.ShowError("网络异常", String.Format("{0} {1}: {2}",
							(int)respose.StatusCode, respose.StatusCode, respose.StatusDescription));

				if (System.IO.File.Exists(filepath))
					System.IO.File.Delete(filepath);
				return;
			}
			catch (Exception ex)
			{
				HandleException(this, ex.GetType().ToString() + " " + ex.Message, ex);
				return;
			}

		}

		public void VoicePlay(int voice_id, bool onlyLocal = false)
		{
			var fileurl = getDownloadUrl(voice_id);
			var filepath = getDownloadPath(voice_id);

			//检查不用下载直接播放的两种情况
			if (_Sets.Download == 0)
			{
				_Vbs.WmpUrl = fileurl;
				_Vbs.PlaySound();
				//return;
			}
			else if (System.IO.File.Exists(filepath))
			{
				if (_Sets.Download == 2 || 
					(lastRequest == fileurl && lastDownPath == filepath))
				{
					_Vbs.WmpUrl = filepath;
					_Vbs.PlaySound();
					//return;
				}
			}
			else
			{
				VoicePlayAsync(voice_id);
				return;
			}


			if (_Sets.ToolTipDisabled < 1)
				Program._Icon.ShowMessage(getMessageText(voice_id));
		}

		public bool CheckNotifyTask(int hour,int minute)
		{
			var found = _Sets.TaskList.Find(x => x.Minute == minute && x.Hour == hour);
			if (found == null) return false;

			if (String.IsNullOrEmpty(found.Title))
			{
				Program._Icon.ShowMessage(found.Text);
			}
			else
			{
				Program._Icon.ShowMessage(found.Title, found.Text);
			}

			if (!String.IsNullOrEmpty(found.Sound))
			{
				_Vbs.WmpUrl = found.Sound;
				_Vbs.PlaySound();
			}
			return true;
		}

		//计时动作
		void OnTimedAction(object sender, EventArgs e)
		{
			var now = DateTime.Now;
			if (now.Second >= (sender as Timer).Interval / 1000) return;

			//自定义提醒
			if (CheckNotifyTask(now.Hour, now.Minute)) return;

			//整点报时 与 放置语音
			if (now.Minute == 0)
			{
				int voice_id = now.Hour + 30;

				VoicePlay(voice_id);

			}
			else if (now.Minute % 20 == 0 && now.Millisecond % 3 == 0)
			{
				VoicePlay(29);
			}
		}
		
		//下载完成动作【废弃】
		void OnDownloadCompletedAction(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
		{
			if (e.Cancelled)
			{
				if (System.IO.File.Exists(lastDownPath))
					System.IO.File.Delete(lastDownPath);
				return;
			}

			if (e.Error != null)
			{
				var respose = (e.Error as WebException).Response as HttpWebResponse;
				if (respose == null)
					Program._Icon.ShowError("网络异常", "当前计算机未连接到互联网，请检查互联网的连接状况。");
				else
					Program._Icon.ShowError("网络异常", String.Format("{0} {1}: {2}",
							(int)respose.StatusCode, respose.StatusCode, respose.StatusDescription));

				if (System.IO.File.Exists(lastDownPath))
					System.IO.File.Delete(lastDownPath);
				return;
			}

			_Vbs.WmpUrl = lastDownPath;
			_Vbs.PlaySound();
		}
		

		//返回下载URL
		string getDownloadUrl(int voice_id)
		{
			return String.Format(requiredUrl, _Sets.ShipFileName, voice_id);
		}

		//返回保存地址
		string getDownloadPath(int voice_id)
		{
			if (_Sets.Download == 0)
				return getDownloadUrl(voice_id);

			else if (_Sets.Download == 1)
			{
				//if (voice_id == 2 || voice_id == 3 || voice_id == 4)
				//	return defaultPath + voice_id.ToString() + ".mp3";
				return defaultPath;
			}


			return _Sets.Download2Folder + "kc" + _Sets.ShipFileName + "\\" + voice_id + ".mp3";
		}

		//返回voice_id对应的信息文本
		string getMessageText(int voice_id)
		{
			int textSource = 2;
			if (textSource > 0)
			{
				var list = new VoiceList();
				//list.Initialize();
				if (list.SearchShip(_Sets.ShipInfo.Name, voice_id))
				{
					string ret = textSource == 1 ? list.Result.Text : list.Result.Text2;
					if (!String.IsNullOrEmpty(ret)) return ret;
				}
			}
			if (voice_id == 1)
				return _Sets.OnLoadText;

			return String.Format(_Sets.OnTimeText, DateTime.Now);
		}

		//异常处理
		static void HandleException(object sender, string message, Exception ex)
		{
			if (Program._Icon != null)
			{
				Program._Icon.ShowError("程序异常", message);
			}
		}
	}
}
