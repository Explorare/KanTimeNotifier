using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using KanTimeNotifier.Properties;

namespace KanTimeNotifier
{
	public class Voice
	{
		public int Id { set; get; }
		public string Args { set; get; }
		public string Text { set; get; }
		public string Text2 { set; get; }
	}

	public class VoiceList
	{
		static readonly Dictionary<string, List<Voice>> dict = new Dictionary<string, List<Voice>>();
		public Voice Result { private set; get; }

		static VoiceList()
		{
			TextReader reader = new StringReader(Resources.VoiceList);

			int count = 0;
			bool space = false;
			string line;
			string key = "";
			Voice ln = null;
			List<Voice> lst = null;

			while((line = reader.ReadLine())!= null)
			{
				line = line.Trim();

				if (String.IsNullOrEmpty(line))
				{
					if (!space) count++;
					space = true;
				}
				else if (line.StartsWith("#ship"))
				{
					if (lst != null)
					{
						if (ln != null) lst.Add(ln);
						dict[key] = lst;
					}
					key = line.Substring(6).Trim();
					lst = new List<Voice>();
					ln = null;

					count = 0;
					space = false;
				}
				else if (lst != null && line.StartsWith("#"))
				{
					if (ln != null) lst.Add(ln);
					ln = new Voice();

					var spl = line.Split(' ');
					var id = 0;
					if (!Int32.TryParse(spl[0].Substring(1), out id))
					{
						ln.Id = 0;
						ln.Args = line.Substring(1);
					}
					else
					{
						ln.Id = id;
						ln.Args = line.Substring(spl[0].Length).Trim();
					}

					count = 0;
					space = false;
				}
				else if (ln != null)
				{
					if (count == 0) count = 1;

					if (count == 1)
					{
						if (space)
							ln.Text = line;
						else
							ln.Text += "\r\n" + line;
					}
					else if (count == 2)
					{
						if (space)
							ln.Text2 = line;
						else
							ln.Text2 += "\r\n" + line;
					}

					space = false;
				}

			}

			if (lst != null)
			{
				if (ln != null) lst.Add(ln);
				dict[key] = lst;
			}

		}

		public bool SearchShip(string key, int id)
		{
			if (!dict.ContainsKey(key)) return false;

			var res = dict[key].Where(x => x.Id == id);

			if (res.Count() <= 0) return false;

			Result = res.First();

			return true;
		}

	}

}
