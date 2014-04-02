using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;

namespace TiledLib
{
	/// <summary>
	/// A layer comprised of objects.
	/// </summary>
	public class MapObjectLayer : Layer
	{
		private readonly Dictionary<string, MapObject> namedObjects = new Dictionary<string, MapObject>();

		/// <summary>
		/// Gets the objects on the current layer.
		/// </summary>
		public ReadOnlyCollection<MapObject> Objects { get; private set; }

		internal MapObjectLayer(string name, int width, int height, bool visible, float opacity, PropertyCollection properties, IList<MapObject> objects)
			: base(name, width, height, visible, opacity, properties)
		{
			Objects = new ReadOnlyCollection<MapObject>(objects);
			foreach (var o in objects)
			{
                string oname = o.Name;
                if (namedObjects.ContainsKey(o.Name)) oname += "-" + Guid.NewGuid();
				namedObjects.Add(oname, o);
			}
		}

		/// <summary>
		/// Gets a MapObject by name.
		/// </summary>
		/// <param name="name">The name of the object to retrieve.</param>
		/// <returns>The MapObject with the given name.</returns>
		public MapObject GetObject(string objectName)
		{
			return namedObjects[objectName];
		}
	}
}
