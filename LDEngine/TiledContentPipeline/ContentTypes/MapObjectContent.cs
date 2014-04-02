using System.Collections.Generic;
using System.Xml;
using Microsoft.Xna.Framework;
using System;

namespace TiledContentPipeline
{
    public enum ObjectType
    {
        Rectangle,
        PolyLine
    }

	public class MapObjectContent
	{
		public string Name = string.Empty;
		public string Type = string.Empty;
        public ObjectType ObjectType;
		public Rectangle Location;
        public List<Point> LinePoints = new List<Point>();
		public List<Property> Properties = new List<Property>();

		public MapObjectContent(XmlNode node)
		{
            if (node.Attributes["name"] != null)
            {
                Name = node.Attributes["name"].Value;
            }

            if (node.Attributes["type"] != null)
            {
                Type = node.Attributes["type"].Value;
            }

            Location = new Rectangle(
                    int.Parse(node.Attributes["x"].Value),
                    int.Parse(node.Attributes["y"].Value),
                    0,
                    0);

            XmlNode polyNode = node["polyline"];
            if (polyNode != null)
            {
                // Polyline node
                ObjectType = ObjectType.PolyLine;

                

                string pointsString = polyNode.Attributes["points"].Value;
                foreach (string point in pointsString.Split(' '))
                {
                    LinePoints.Add(new Point(Location.X + Convert.ToInt32(point.Split(',')[0]), Location.Y + Convert.ToInt32(point.Split(',')[1])));
                }
            }
            else
            {
                ObjectType = ObjectType.Rectangle;
                
                Location = new Rectangle(
                    int.Parse(node.Attributes["x"].Value),
                    int.Parse(node.Attributes["y"].Value),
                    int.Parse(node.Attributes["width"].Value),
                    int.Parse(node.Attributes["height"].Value));
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
