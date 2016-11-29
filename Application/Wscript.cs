using System;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace KanTimeNotifier
{
	public class Wscript
	{
		const string tmplate = 
@"Set Wmp=CreateObject(""WMPlayer.OCX"")
Wmp.Settings.Volume=""{1}""
Wmp.Url=""{0}""
While Wmp.PlayState=6 or Wmp.PlayState=9 or Wmp.PlayState=3
  Wscript.Sleep 5
Wend
";
		public string WmpUrl { set; get; }
		public int WmpVolume { set; get; }
		public string Content
		{
			get
			{ 
				return String.Format(tmplate, WmpUrl, WmpVolume);
			}
		}

		string filepath;
		Process process;

		public Wscript(string path)
		{
			path = String.IsNullOrEmpty(path) ?
						Path.GetTempPath() + "tmpPlay.vbs" : path;

			this.filepath = path;
			//this.process.StartInfo.FileName = path;
			//this.process.StartInfo.UseShellExecute = true;
		}

		public void PlaySound()
		{
			if (String.IsNullOrEmpty(WmpUrl))
				//throw new ArgumentNullException("WmpUrl");
				return;

			this.Save();
			this.Execute();
		}

		void Save()
		{
			try
			{
				File.WriteAllText(filepath, Content, Encoding.Unicode);
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show("文件读写异常：" + filepath + "\r\n详细：" + ex.Message);
				return;
			}
		}

		void Execute()
		{
			if (!File.Exists(filepath))
				//throw new FileNotFoundException("在执行Execute()前应当执行Save()保存文件", filepath);
				return;

			try
			{
				if (process == null)
				{
					var si = new ProcessStartInfo()
					{
						FileName = filepath,
						UseShellExecute = true,
					};
					process = Process.Start(si);
				}
				else
				{
					if (!process.HasExited)
					{
						process.Kill();
					}
					process.Start();
				}
			}
			catch (Exception ex)
			{
				System.Windows.Forms.MessageBox.Show("创建进程异常：" + ex.Message);
				process = null;
				return;
			}
			
		}

		public void Finish()
		{
			try { File.Delete(filepath); }
			catch { }
		}
	}
}
