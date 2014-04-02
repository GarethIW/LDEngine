using System;
using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace TiledContentPipeline
{
	public class Tile
	{
		public Rectangle Source;
		public List<Property> Properties = new List<Property>();
	}

	public class TileSetContent
	{
		public int FirstId;
		public string Name;
        public bool CollisionSet = false;
		public int TileWidth;
		public int TileHeight;
		public int Spacing;
		public int Margin;
		public string Image;
		public Color? ColorKey;
		public ExternalReference<TextureContent> Texture;
		public List<Tile> Tiles = new List<Tile>();
		public Dictionary<int, List<Property>> TileProperties = new Dictionary<int, List<Property>>();

		public TileSetContent(XmlNode node)
		{
			FirstId = int.Parse(node.Attributes["firstgid"].Value);
			Name = node.Attributes["name"].Value;
			TileWidth = int.Parse(node.Attributes["tilewidth"].Value);
			TileHeight = int.Parse(node.Attributes["tileheight"].Value);

			if (node.Attributes["spacing"] != null)
			{
				Spacing = int.Parse(node.Attributes["spacing"].Value);
			}

			if (node.Attributes["margin"] != null)
			{
				Margin = int.Parse(node.Attributes["margin"].Value);
			}

			XmlNode imageNode = node["image"];
			Image = imageNode.Attributes["source"].Value;

			if (imageNode.Attributes["trans"] != null)
			{
				string color = imageNode.Attributes["trans"].Value;
				string r = color.Substring(0, 2);
				string g = color.Substring(2, 2);
				string b = color.Substring(4, 2);
				ColorKey = new Color((byte)Convert.ToInt32(r, 16), (byte)Convert.ToInt32(g, 16), (byte)Convert.ToInt32(b, 16));
			}

			foreach (XmlNode tileProperty in node.SelectNodes("tile"))
			{
				int id = int.Parse(tileProperty.Attributes["id"].Value);
				List<Property> properties = new List<Property>();

				foreach (XmlNode propertyNode in tileProperty.SelectNodes("properties/property"))
				{
                    // If any tile has collisions set, mark the whole tileset as a collision set
                    if (propertyNode.Attributes["name"].Value.StartsWith("CollisionSet")) CollisionSet = true;

					properties.Add(new Property
					{
						Name = propertyNode.Attributes["name"].Value,
						Value = propertyNode.Attributes["value"].Value,

					});
				}

				TileProperties.Add(id, properties);
			}
		}
	}
}
