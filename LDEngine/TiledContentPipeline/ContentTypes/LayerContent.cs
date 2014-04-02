using System.Collections.Generic;
using System.Xml;

namespace TiledContentPipeline
{
	public abstract class LayerContent
	{
		public string Name;
		public string Type;
		public int Width;
		public int Height;
		public float Opacity = 1f;
		public bool Visible = true;
		public List<Property> Properties = new List<Property>();

		public LayerContent(XmlNode node)
		{
			Type = node.Name;
			Name = node.Attributes["name"].Value;
			Width = int.Parse(node.Attributes["width"].Value);
			Height = int.Parse(node.Attributes["height"].Value);

			if (node.Attributes["opacity"] != null)
			{
				Opacity = float.Parse(node.Attributes["opacity"].Value);
			}

			if (node.Attributes["visible"] != null)
			{
				Visible = int.Parse(node.Attributes["visible"].Value) == 1;
			}

			XmlNode propertiesNode = node["properties"];
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
		}
	}
}
