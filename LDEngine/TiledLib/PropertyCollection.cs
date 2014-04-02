using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System;

namespace TiledLib
{
	public class PropertyCollection : IEnumerable<KeyValuePair<string, string>>
	{
		private readonly Dictionary<string, string> values = new Dictionary<string, string>();

		public string this[string key]
		{
			get { return values[key]; }
			set { values[key] = value; }
		}

		internal PropertyCollection() { }

		internal void Add(string key, string value)
		{
            if (this.Contains(key)) key = key + Guid.NewGuid().ToString();
			values.Add(key, value);
		}

		internal void Read(ContentReader reader)
		{
			int count = reader.ReadInt32();
			for (int i = 0; i < count; i++)
			{
				Add(reader.ReadString(), reader.ReadString());
			}
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
		{
			return values.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return values.GetEnumerator();
		}

        public bool Contains(string key)
        {
            bool contains = false;

            if (values.ContainsKey(key)) contains = true;

            return contains;
        }
	}
}
