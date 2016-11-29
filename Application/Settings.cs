using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using d_f_32.Configration;
using System.IO;
using System.Diagnostics;
using KanTimeNotifier.Properties;

namespace KanTimeNotifier
{
	public class Settings
	{
		static public Settings Current;
		public Ship ShipInfo { private set; get; }				//【ShipInfo是否还需要public？因为已经有了Variables了
		public List<NotifyTask> TaskList  { private set; get; }
		
		//[Setting]
		public string ShipGirl { set; get; }
		public string ShipFileName { set; get; }
		public int Download { set; get; }
		public string Download2Folder { set; get; }
		public int Volume { set; get; }
		public string StartGamePath {set; get; }
		//[Advanced]
		public string URL { private set; get; }
		//[Debug]
		public int _VoiceId { private set; get; }
		//[ToolTip]
		public string IconText { private set; get; }
		public string ToolTipTitle { private set; get; }
		public string OnLoadText { private set; get; }
		public string OnTimeText {private set; get; }
		public int ToolTipDisabled { private set; get; }


		static public void Initialize(string filePath = "Setting.ini")
		{
			Current = new Settings();
			Current.TaskList = new List<NotifyTask>();
			Load(filePath);
		}
		
		static public void Load(string filePath = "Setting.ini")
		{
			//【Parser是临时的，因为程序运行过程中不需要它
			ConfigParser _Parser = new ConfigParser();

			//若设置文件不存在，则创建一个新的设置文件
			if (!File.Exists(filePath))
			{
				//【就不检查读写异常了
				File.AppendAllText(filePath, Resources.Setting,Encoding.Default);
			}

			//读取设置文件，并读取设置
			if (_Parser.TryReadIniFile(filePath))
			{
				//[Setting]
				Current.ShipGirl = _Parser.ReadString("Setting", "ShipGirl", "");
				Current.ShipFileName = _Parser.ReadString("Setting", "ShipFileName", "");
				Current.Download = _Parser.ReadInt("Setting", "Download", 1);
				Current.Download2Folder = _Parser.ReadString("Setting", "Download2Folder", "");
				Current.Volume = _Parser.ReadInt("Setting", "Volume", 80);
				Current.StartGamePath = _Parser.ReadString("Setting", "StartGamePath", "");
				//[Debug]
				Current._VoiceId = _Parser.ReadInt("Debug", "VoiceId", 0);
				//[Advance]
				Current.URL = _Parser.ReadString("Advance", "URL", "");
				//[ToolTip]
				Current.ToolTipDisabled = _Parser.ReadInt("ToolTip", "Disabled", 0);
				
				//检查设置并匹配ShipInfo
				Current.Check(_Parser);

				//从ShipInfo设置_Parser的变量
				_Parser.Variables["Name"] = Current.ShipInfo.Name;
				_Parser.Variables["JpName"] = Current.ShipInfo.JpName;
				_Parser.Variables["Postfix"] = Current.ShipInfo.Postfix;
				_Parser.Variables["ShipType"] = Current.ShipInfo.ShipType;
				_Parser.Variables["CV"] = Current.ShipInfo.CV;

				//[ToolTip] 支持变量
				Current.ToolTipTitle = _Parser.ReadString("ToolTip", "Title", @"#Name#",true);
				Current.IconText = _Parser.ReadString("ToolTip", "IconText", @"#Name##Postfix#（CV：#CV#）", true);
				Current.OnLoadText = _Parser.ReadString("ToolTip", "OnLoadText", @"司令官，现在由#ShipType#「#Name#」为您报时。", true);
				Current.OnTimeText = _Parser.ReadString("ToolTip", "OnTimeText", @"现在时间是 {0:tth点mm分}。司令官，今天也要一起加油哦。", true);

				//[Notification]
				Current.LoadNotifyTaskList(_Parser);

				//保存设置
				Current.Save(_Parser, filePath);
			}
			else
			{
				//不会运行到这里
			}
		}

		void Check(ConfigParser _Parser)
		{
			//加载内置信息
			ShipList _List = new ShipList();
			//_List.Initialize();

			//检查文件夹地址
			if (Download >= 2)
			{
				if (String.IsNullOrEmpty(Download2Folder))
				{
					Download2Folder = Directory.GetCurrentDirectory();
				}
				else if (!Directory.Exists(Download2Folder))
				{
					if (!DirectoryEx.CreateFolderTree(Download2Folder))
						Download2Folder = Directory.GetCurrentDirectory();
				}

				if (!Download2Folder.EndsWith("\\") && !Download2Folder.EndsWith("/"))
				{
					Download2Folder += "\\";
				}
			}

			//检查舰娘名与文件名，并匹配信息
			if (!String.IsNullOrEmpty(ShipGirl) && _List.Search(ShipGirl))
			{
				//ShipGirl=有效

				goto LoadResult;
			}
			else if (String.IsNullOrEmpty(ShipFileName))
			{
				//ShipGirl=无效 ShipFileName=空	
				//【默认设置】

				ShipGirl = "大淀";
				_List.Search(ShipGirl);

				goto LoadResult;
			}
			else if (_List.SearchInfo(ShipFileName))
			{
				//ShipGirl=无效 ShipFileName=有效

				goto LoadResult;
			}
			else
			{
				//ShipGirl=无效 ShipFileName=无效
				//【用户自定义】

				goto LoadCustom;
			}

		LoadCustom:

			ShipInfo = new Ship
			{
				FileName = ShipFileName,
				Name = ShipGirl,
				//仅当设置了FileName，且FileName没有匹配ShipInfo的时候，才信任[Variables]节点中的信息
				JpName = _Parser.GetVariable("JpName", ShipGirl),
				Postfix = _Parser.GetVariable("Postfix", ""),
				ShipType = _Parser.GetVariable("ShipType", ""),
				CV = _Parser.GetVariable("CV", ""),
			};

		goto ExitCheck;

		LoadResult:

			ShipInfo = _List.Result;
			ShipGirl = _List.Result.Name;
			ShipFileName = _List.Result.FileName;

			if (!String.IsNullOrEmpty(ShipInfo.Postfix))
				ShipGirl += " " + ShipInfo.Postfix;

			goto ExitCheck;

		ExitCheck:

			Volume = Volume > 100 ? 100 : (Volume < 0 ? 0 : Volume);

			if (String.IsNullOrEmpty(StartGamePath))
				TryGetGamePath();
		}

		void Save(ConfigParser _Parser, string filePath)
		{
			//[Setting] 只有程序会修改的设置才需要保存
			_Parser.SetValue("Setting", "ShipGirl", ShipGirl);
			_Parser.SetValue("Setting", "ShipFileName", ShipFileName);
			_Parser.SetValue("Setting", "Download", Download.ToString());
			if (Download > 1)
				_Parser.SetValue("Setting", "Download2Folder", Download2Folder);
			_Parser.SetValue("Setting", "Volume", Volume.ToString());
			_Parser.SetValue("Setting", "StartGamePath", StartGamePath);

			
			//【异常处理需要修改
			if(!_Parser.TrySaveIniFile(filePath))
			{
				Debug.WriteLine("设置文件未能保存:" + filePath);
			}
		}

		//【public保存，是否有必要？若是，需要重新设计异常处理
		public void Save(string filePath = "Setting.ini")
		{
			try { Save(new ConfigParser(filePath), filePath); }
			catch { Debug.WriteLine("设置文件未能保存:" + filePath); }
		}



		void LoadNotifyTaskList(ConfigParser _Parser)
		{
			TaskList.Clear();
			var section = _Parser["Notification"];
			if (section == null) return;

			if (!AddNotifyTask(
					_Parser.ReplaceVariables(section["Time"]), 
					_Parser.ReplaceVariables(section["Title"]), 
					_Parser.ReplaceVariables(section["Text"]), 
					_Parser.ReplaceVariables(section["Sound"])))
				return;

			int i = 2;
			while (AddNotifyTask(
				_Parser.ReplaceVariables(section["Time" + i]),
				_Parser.ReplaceVariables(section["Title" + i]),
				_Parser.ReplaceVariables(section["Text" + i]), 
				_Parser.ReplaceVariables(section["Sound" + i])))
			{
				i++;
			}
		}

		bool AddNotifyTask(string time, string title, string text, string sound)
		{
			if (String.IsNullOrEmpty(time)) return false;

			var spl = time.Split(':');

			if (spl.Count() < 2) return false;

			int hour, minute;

			if (Int32.TryParse(spl[0], out hour) && Int32.TryParse(spl[1], out minute))
			{
				TaskList.Add(new NotifyTask
				{
					Hour = hour,
					Minute = minute,
					Title = title,
					Text = text ,
					Sound = sound,
				});
				return true;
			}
			return false;
		}

		void TryGetGamePath()
		{
			var prcs = Process.GetProcessesByName("KanColleViewer");
			if (!prcs.Any())
			{
				prcs = Process.GetProcessesByName("ShimakazeGo");
			}
			if (prcs.Any())
			{
				StartGamePath = prcs[0].MainModule.FileName;
				return;
			}

			var shortcut = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\Programs\\KanColleViewer!.lnk";
			if (!File.Exists(shortcut))
			{
				shortcut = Environment.GetFolderPath(Environment.SpecialFolder.StartMenu) + "\\Programs\\提督業も忙しい！.lnk";
			}
			if (File.Exists(shortcut))
			{
				StartGamePath = shortcut;
				return;
			}

			StartGamePath = "http://www.kancolle.tv";
		}
	
	}

	public class NotifyTask
	{
		public int Hour { set; get; }
		public int Minute { set; get; }
		public string Title { set; get; }
		public string Text { set; get; }
		public string Sound { set; get; }
	}
}
