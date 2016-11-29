using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace d_f_32.Configration
{
	/// <summary>
	/// 表示一个INI文档节点
	/// </summary>
	public class Section
	{
		readonly Dictionary<string, string> Options = new Dictionary<string,string>();
		readonly Dictionary<string, string> Comments = new Dictionary<string,string>();

		/// <summary>
		/// 获取或设置指定指定的选项的选项值。选项名称不区分大小写。
		/// </summary>
		/// <param name="section">选项的名称</param>
		/// <returns>指定的选项。如果找不到指定选项，get 将返回null，set 将创建指定值的选项</returns>
		public string this[string option]
		{
			get
			{
				foreach (var key in Options.Keys)
				{
					if (String.Compare(key, option, true) == 0)
						return Options[key];
				}
				return null;
			}
			set
			{
				Options[option] = value;
			}
		}

		/// <summary>
		/// 添加指定选项后的注释。空值选项表示节点注释。注释将自动换行。
		/// </summary>
		/// <param name="option">选项名</param>
		/// <param name="comment">注释</param>
		public void AddComment(string option, string comment)
		{
			foreach (var key in Comments.Keys)
			{
				if (String.Compare(key, option, true) == 0)
				{
					Comments[key] += "\r\n" + comment;
					return;
				}
			}
			Comments.Add(option.ToLower(), comment);
		}

		/// <summary>
		/// 将节点内容序列化为文本
		/// </summary>
		/// <returns>表示节点内容的文本</returns>
		public string Serialize()
		{
			StringBuilder contents = new StringBuilder();
			
			//节点注释
			if (Comments.ContainsKey(""))
			{
				contents.AppendLine(Comments[""]);
			}

			foreach (var pairs in Options)
			{
				contents.AppendFormat(@"{0}={1}", pairs.Key, pairs.Value);
				contents.AppendLine();

				var key = pairs.Key.ToLower();
				if (Comments.ContainsKey(key))
				{
					contents.AppendLine(Comments[key]);
				}
			}
			return contents.ToString(0, contents.Length -2);
		}
	}

	/// <summary>
	/// 表示一个INI配置文件
	/// </summary>
	public class ConfigParser
	{
		/// <summary>
		/// 此INI文件的文档注释。
		/// 文档注释位于所有节点的上面。
		/// </summary>
		public string Comment { set; get; }

		Dictionary<string, Section> m_Sections;

		/// <summary>
		/// 获取或设置指定名称的节点。 节点名称不区分大小写。
		/// </summary>
		/// <param name="section">节点的名称</param>
		/// <returns>指定的节点。如果找不到指定的键，get 将返回null，set 将创建指定的节点</returns>
		public Section this[string section]
		{
			set
			{
				m_Sections[section] = value;
			}
			get
			{
				foreach (var key in m_Sections.Keys)
				{
					if (String.Compare(key, section, true) == 0)
						return m_Sections[key];
				}
				return null;
			}
		}
		
		
		/// <summary>
		/// Variables 节点
		/// 声明了在INI文档中可以通过“#变量名#”的形式引用的名为“变量”的特使选项。
		/// 此成员仅用于替换变量的引用，不会被打印。
		/// </summary>
		public Section Variables;

		
		/// <summary>
		/// 使用指定的INI文件初始化<see cref="ConfigParser"/>类的新实例
		/// </summary>
		/// <param name="iniFile">要加载的INI文件地址</param>
		public ConfigParser(string iniFile = "")
		{
			m_Sections = new Dictionary<string,Section>();
			
			if (!String.IsNullOrEmpty(iniFile))
				ReadIniFile(iniFile);

			if (Variables == null)
				Variables = new Section();
		}

		/// <summary>
		/// 读取并加载INI文件。
		/// 若文件不存在或无法访问将引发异常。
		/// </summary>
		/// <param name="iniFile">要加载的INI文件地址</param>
		/// <param name="encoding">应用到文件内容的字符编码</param>
		/// <exception cref="FileNotFoundException">应当是存在的文件</exception>
		public void ReadIniFile(string iniFile, Encoding encoding = null)
		{
			//检查文件是否存在
			if (!File.Exists(iniFile))
			{
				throw new FileNotFoundException("指定的INI文件不存在。", iniFile);
			}

			m_Sections.Clear();
			Comment = "";

			//开始读取
			//Debug.WriteLine("开始读取文档：" + iniFile);

			//记录当前节点与选项
			Section curSection = null;
			string curOption = "";

			//正则表达式
			Regex regex = new Regex(@"^([^=]+?)=(.*)");

			//读取所有行
			var lines = File.ReadLines(iniFile, encoding?? Encoding.Default);

			//行遍历
			foreach (var line in lines)
			{
				line.Trim();

				//当前行位于 任何节点以外
				if (curSection == null)
				{
					//当前行是 [节点]
					if (line.StartsWith("[") && line.EndsWith("]"))
					{
						var section = line.Substring(1, line.Count() - 2);

						curSection = this[section] = this[section] ?? new Section();
						curOption = "";
					}
					//当前行是 文档注释
					else
					{
						if (String.IsNullOrEmpty(Comment))
							Comment = line;
						else
							Comment += "\r\n" + line;
					}
				}
				//当前行位于 某个节点下
				else
				{
					//当前行是 [节点]
					if (line.StartsWith("[") && line.EndsWith("]"))
					{
						var section = line.Substring(1, line.Count() - 2);

						curSection = this[section] = this[section] ?? new Section();
						curOption = "";
					}
					//当前行是 注释
					else if (line.StartsWith(";"))
					{
						curSection.AddComment(curOption, line);
					}
					else
					{
						Match match = regex.Match(line);
						//当前行是 Option=Value
						if (match.Success)
						{
							var option = match.Groups[1].Value.Trim();
							var value = match.Groups[2].Value.Trim();
							curSection[option] = value;
							curOption = option.ToLower();
						}
						//当前行是 无效内容
						else
						{
							curSection.AddComment(curOption, line);
						}
					}
				}
			}

			//【读取文档前设置变量是无效的
			var _vars = this["Variables"];
			if (_vars != null)
				Variables = _vars;
		}

		/// <summary>
		/// 读取并加载INI文件。
		/// </summary>
		/// <param name="iniFile">INI文件地址</param>
		/// <param name="encoding">应用到文件的字符编码</param>
		/// <returns>是否成功</returns>
		public bool TryReadIniFile(string iniFile, Encoding encoding = null)
		{
			try { ReadIniFile(iniFile,encoding);}
			catch { return false; }
			return true;
		}

		/// <summary>
		/// 将INI文档保存并输出。
		/// 若文件无法访问将引发异常。
		/// </summary>
		/// <param name="iniFile">INI文件地址</param>
		/// <exception cref="DirectoryNoFoundException">必须是已经存在的文件夹</exception>
		public void SaveIniFile(string iniFile, Encoding encoding = null)
		{
			File.WriteAllText(iniFile, Serialize(), Encoding.Default);
		}

		/// <summary>
		/// 尝试将INI文档保存并输出。返回是否成功。
		/// </summary>
		/// <param name="iniFile">INI文件地址</param>
		/// <param name="encoding">应用到文件的字符编码</param>
		/// <returns>是否成功</returns>
		public bool TrySaveIniFile(string iniFile, Encoding encoding = null)
		{
			try { SaveIniFile(iniFile, encoding); }
			catch { return false; }
			return true;
		}

		/// <summary>
		/// 将INI文档内容序列化为文本。
		/// </summary>
		/// <returns>表示文档内容的文本</returns>
		public string Serialize()
		{
			StringBuilder contents = new StringBuilder();

			//节点注释
			if (!String.IsNullOrEmpty(Comment))
			{
				contents.AppendLine(Comment);
			}

			foreach (var pairs in m_Sections)
			{
				contents.AppendFormat(
@"[{0}]
{1}
", pairs.Key, pairs.Value.Serialize());
			}
			
			return contents.ToString();
		}

		/// <summary>
		/// 设置指定节点下指定选项的值。操作将自动创建不存在的节点和选项。
		/// </summary>
		/// <param name="section">节点名</param>
		/// <param name="option">选项名</param>
		/// <param name="value">选项值</param>
		public void SetValue(string section, string option, string value)
		{
			var sec = this[section];

			if (sec == null)
				this[section] = sec = new Section();

			sec[option] = value;
		}

		/// <summary>
		/// 读取值类型为<see cref="String"/>的选项。当节点或选项不存在时，返回指定的默认值。
		/// </summary>
		/// <param name="section">节点名</param>
		/// <param name="option">选项名</param>
		/// <param name="defValue">默认值</param>
		/// <returns>选项值</returns>
		public string ReadString(string section, string option, string defValue = "", bool replaceVar = false)
		{
			var sec = this[section];
			if (sec != null)
			{
				var val = sec[option];

				defValue = val ?? defValue;
			}
			if (replaceVar) return ReplaceVariables(defValue);

			return defValue;
		}

		public string GetVariable(string name,string defValue)
		{
			var value = Variables[name];
			//return String.IsNullOrEmpty(value) ? defValue : value;
			//变量应当可以是空白的！在使用#var#引用时替换为""
			//这点与Rainmeter不同
			return value == null ? defValue : value;
		}

		/// <summary>
		/// 返回替换变量后的字符串
		/// </summary>
		/// <param name="str">包含变量的字符串</param>
		/// <returns>替换后的字符串</returns>
		public string ReplaceVariables(string str)
		{
			if (String.IsNullOrEmpty(str)) return "";
			var spls = str.Split('#');
			var lst = spls.Count() - 1;
			bool isvar = false;			//当前字符串是否为变量
			for (int i = 0; i < lst; i++)
			{
				if (isvar)
				{
					var _key = spls[i].Trim();
					var _val = "";
					isvar = !String.IsNullOrEmpty(_key);
					if (isvar)
					{
						_val = Variables[_key];
						isvar = _val != null;	//变量可以是空的
					}
					if (isvar)
						spls[i] = _val;
					else
						spls[i] = "#" + spls[i];
				}
				isvar = !isvar;
			}
			if (isvar) spls[lst] = "#" + spls[lst];
			return String.Concat(spls);
		}

		/// <summary>
		/// 读取值类型为<see cref="Int32"/>的选项。当节点或选项不存在时，返回指定的默认值。
		/// </summary>
		/// <param name="section">节点名</param>
		/// <param name="option">选项名</param>
		/// <param name="defValue">默认值</param>
		/// <returns>选项值</returns>
		public int ReadInt(string section, string option, int defValue = 0)
		{
			var ret = ReadString(section, option, "");

			int value;
			if (ret != "" && Int32.TryParse(ret, out value))
				return value;

			return defValue;
		}

		/// <summary>
		/// 读取值类型为<see cref="bool"/>的选项。当节点或选项不存在时，返回指定的默认值。
		/// </summary>
		/// <param name="section">节点名</param>
		/// <param name="option">选项名</param>
		/// <param name="defValue">默认值</param>
		/// <returns>选项值</returns>
		public bool ReadBool(string section,string option, bool defValue = false)
		{
			return ReadInt(section, option, defValue ? 1 : 0) > 0 ;
		}

		/// <summary>
		/// 添加指定节点下指定选项后的注释。
		/// </summary>
		/// <param name="section">节点名。将忽略不存在的节点。</param>
		/// <param name="option">选项名。空值表示节点的注释。</param>
		/// <param name="comment">注释。将自动换行。</param>
		public void AddComment(string section, string option, string comment)
		{
			var sec = this[section];
			if ( sec != null)
				sec.AddComment(option, comment);
		}
	}
}
