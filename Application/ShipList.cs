using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using KanTimeNotifier.Properties;

namespace KanTimeNotifier
{
	public class Ship
	{
		public string Name { get; set; }
		public string JpName { get; set; }
		public string Postfix { get; set; }
		public string FileName { get; set; }
		public string ShipType { get; set; }
		public string CV { get; set; }
	}


	public class ShipList: IEnumerable<Ship>
	{
		static readonly List<Ship> shipList;
		public Ship Result { private set; get; }

		static ShipList()
		{
			using (TextReader reader = new StringReader(Resources.ShipList))
			{
				var serializer = new XmlSerializer(typeof(List<Ship>));
				shipList = serializer.Deserialize(reader) as List<Ship>;
			}
		}

		public bool Search(string shipName)
		{
			var found = shipList.Find(x =>
				{
					return shipName.Contains(x.Name) || shipName.Contains(x.JpName);
				});

			if (found != null)
			{
				this.Result = found;
				return true;
			}
			return false;
		}

		public bool SearchInfo(string filename)
		{
			var found = shipList.Find(x =>
			{
				return x.FileName == filename;
			});

			if (found != null)
			{
				this.Result = found;
				return true;
			}
			return false;
		}

		public IEnumerator<Ship> GetEnumerator()
		{
			return shipList.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return shipList.GetEnumerator();
		}
	}
}