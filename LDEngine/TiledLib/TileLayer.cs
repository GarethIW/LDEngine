using System.Collections.Generic;
using Microsoft.Xna.Framework;

using TiledSharp;

namespace TiledLib
{
	/// <summary>
	/// A map layer containing tiles.
	/// </summary>
	public class TileLayer : Layer
	{
		/// <summary>
		/// Gets the layout of tiles on the layer.
		/// </summary>
		public Tile[,] Tiles { get; private set; }

		internal TileLayer(string name, int width, int height, bool visible, float opacity, PropertyCollection properties, Map map, int[] data)
			: base(name, width, height, visible, opacity, properties)
		{
			Tiles = new Tile[width, height];

			// data is left-to-right, top-to-bottom
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					int index = data[y * width + x];
					Tiles[x, y] = map.Tiles[index];
				}
			}
		}

        internal TileLayer(
            string name,
            int width,
            int height,
            bool visible,
            float opacity,
            PropertyCollection properties,
            Map map,
            List<TmxLayerTile> data
        ) : base(name, width, height, visible, opacity, properties)
        {
            Tiles = new Tile[width, height];

            // data is left-to-right, top-to-bottom
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int index = data[y * width + x].Gid;
                    Tiles[x, y] = map.Tiles[index];
                }
            }
        }

	}
}
