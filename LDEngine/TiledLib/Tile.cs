using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;

namespace TiledLib
{
	public class Tile
	{
		public Texture2D Texture { get; private set; }
		public Rectangle Source { get; private set; }
		public PropertyCollection Properties { get; private set; }
        public bool[] CollisionData { get; private set; }

		internal Tile(Texture2D texture, Rectangle source, PropertyCollection properties, bool[] collision)
		{
			Texture = texture;
			Source = source;
			Properties = properties;
            CollisionData = collision;
		}

        internal void UnloadContent()
        {
            CollisionData = null;
            //Texture = null;
            //Properties = null;
        }
    }
}
