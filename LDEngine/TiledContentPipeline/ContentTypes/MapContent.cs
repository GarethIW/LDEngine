using System;
using System.Collections.Generic;
using TiledLib;
using System.Xml;
using System.Diagnostics;

namespace TiledContentPipeline
{
	public class MapContent
	{
		public string Version = string.Empty;
		public Orientation Orientation;
		public int Width;
		public int Height;
		public int TileWidth;
		public int TileHeight;
		public List<Property> Properties = new List<Property>();

		public List<TileSetContent> TileSets = new List<TileSetContent>();
		public List<LayerContent> Layers = new List<LayerContent>();

		public MapContent(XmlDocument document)
		{
			XmlNode mapNode = document["map"];

			Version = mapNode.Attributes["version"].Value;
			Orientation = (Orientation)Enum.Parse(typeof(Orientation), mapNode.Attributes["orientation"].Value, true);
			Width = int.Parse(mapNode.Attributes["width"].Value);
			Height = int.Parse(mapNode.Attributes["height"].Value);
			TileWidth = int.Parse(mapNode.Attributes["tilewidth"].Value);
			TileHeight = int.Parse(mapNode.Attributes["tileheight"].Value);

			XmlNode propertiesNode = document.SelectSingleNode("map/properties");
			if (propertiesNode != null)
			{
				foreach (XmlNode property in propertiesNode.ChildNodes)
				{
					Properties.Add(new Property
					{
						Name = property.Attributes["name"].Value,
						Value = property.Attributes["value"].Value,
					});
				}
			}

			foreach (XmlNode tileSet in document.SelectNodes("map/tileset"))
			{
				TileSets.Add(new TileSetContent(tileSet));
			}

			foreach (XmlNode layerNode in document.SelectNodes("map/layer|map/objectgroup"))
			{
				if (layerNode.Name == "layer")
				{
					Layers.Add(new TileLayerContent(layerNode));
				}
				else if (layerNode.Name == "objectgroup")
				{
					Layers.Add(new MapObjectLayerContent(layerNode));
				}
				else
				{
					throw new Exception("Unknown layer name: " + layerNode.Name);
				}
			}
		}
	}
}
