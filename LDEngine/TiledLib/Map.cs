using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TiledLib
{
	/// <summary>
	/// A full map from Tiled.
	/// </summary>
	public class Map
	{
		private readonly Dictionary<string, Layer> namedLayers = new Dictionary<string, Layer>();
		
		/// <summary>
		/// Gets the version of Tiled used to create the Map.
		/// </summary>
		public Version Version { get; private set; }

		/// <summary>
		/// Gets the orientation of the map.
		/// </summary>
		public Orientation Orientation { get; private set; }

		/// <summary>
		/// Gets the width (in tiles) of the map.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Gets the height (in tiles) of the map.
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// Gets the width of a tile in the map.
		/// </summary>
		public int TileWidth { get; private set; }

		/// <summary>
		/// Gets the height of a tile in the map.
		/// </summary>
		public int TileHeight { get; private set; }

		/// <summary>
		/// Gets a list of the map's properties.
		/// </summary>
		public PropertyCollection Properties { get; private set; }

		/// <summary>
		/// Gets a collection of all of the tiles in the map.
		/// </summary>
		public Collection<Tile> Tiles { get; private set; }

		/// <summary>
		/// Gets a collection of all of the layers in the map.
		/// </summary>
		public ReadOnlyCollection<Layer> Layers { get; private set; }

        //private Layer collisionLayer;
	
		internal Map(ContentReader reader) 
		{
			// read in the basic map information
			Version = new Version(reader.ReadString());
			Orientation = (Orientation)reader.ReadByte();
			Width = reader.ReadInt32();
			Height = reader.ReadInt32();
			TileWidth = reader.ReadInt32();
			TileHeight = reader.ReadInt32();
			Properties = new PropertyCollection();
			Properties.Read(reader);

			// create a list for our tiles
			List<Tile> tiles = new List<Tile>();
			Tiles = new Collection<Tile>(tiles);

			// read in each tile set
			int numTileSets = reader.ReadInt32();
			for (int i = 0; i < numTileSets; i++)
			{
				// get the id and texture
				int firstId = reader.ReadInt32();
                string tilesetName = reader.ReadString();
                bool collisionSet = reader.ReadBoolean();

				Texture2D texture = reader.ReadExternalReference<Texture2D>();

                // Read in color data for collision purposes
                // You'll probably want to limit this to just the tilesets that are used for collision
                // I'm checking for the name of my tileset that contains wall tiles
                // Color data takes up a fair bit of RAM
                Color[] collisionData = null;
                bool[] collisionBitData = null;
                if (collisionSet)
                {
                    collisionData = new Color[texture.Width * texture.Height];
                    collisionBitData = new bool[texture.Width * texture.Height];
                    texture.GetData<Color>(collisionData);
                    for (int col = 0; col < collisionData.Length; col++) if (collisionData[col].A > 0) collisionBitData[col] = true;
                    collisionData = null;
                }

				// read in each individual tile
				int numTiles = reader.ReadInt32();
				for (int j = 0; j < numTiles; j++)
				{
					int id = firstId + j;
					Rectangle source = reader.ReadObject<Rectangle>();
					PropertyCollection props = new PropertyCollection();
					props.Read(reader);

					Tile t = new Tile(texture, source, props, collisionBitData);
					while (id >= tiles.Count)
					{
						tiles.Add(null);
					}
					tiles.Insert(id, t);
				}
			}

			// read in all the layers
			List<Layer> layers = new List<Layer>();
			Layers = new ReadOnlyCollection<Layer>(layers);
			int numLayers = reader.ReadInt32();
			for (int i = 0; i < numLayers; i++)
			{
				Layer layer = null;

				// read generic layer data
				string type = reader.ReadString();
				string name = reader.ReadString();
				int width = reader.ReadInt32();
				int height = reader.ReadInt32();
				bool visible = reader.ReadBoolean();
				float opacity = reader.ReadSingle();
				PropertyCollection props = new PropertyCollection();
				props.Read(reader);

				// using the type, figure out which object to create
				if (type == "layer")
				{
					int[] data = reader.ReadObject<int[]>();
					layer = new TileLayer(name, width, height, visible, opacity, props, this, data);
				}
				else if (type == "objectgroup")
				{
					List<MapObject> objects = new List<MapObject>();

					// read in all of our objects
					int numObjects = reader.ReadInt32();
					for (int j = 0; j < numObjects; j++)
					{
						string objName = reader.ReadString();
						string objType = reader.ReadString();
						Rectangle objLoc = reader.ReadObject<Rectangle>();
                        List<Point> objPoints = reader.ReadObject<List<Point>>();
						PropertyCollection objProps = new PropertyCollection();
						objProps.Read(reader);

						objects.Add(new MapObject(objName, objType, objLoc, objPoints, objProps));
					}

					layer = new MapObjectLayer(name, width, height, visible, opacity, props, objects);
				}
				else
				{
					throw new Exception("Invalid type: " + type);
				}

				layers.Add(layer);
				namedLayers.Add(name, layer);
			}
		}

		/// <summary>
		/// Gets a layer by name.
		/// </summary>
		/// <param name="name">The name of the layer to retrieve.</param>
		/// <returns>The layer with the given name.</returns>
		public Layer GetLayer(string name)
		{
            if (namedLayers.ContainsKey(name))
                return namedLayers[name];
            else
                return null;
		}

        /// <summary>
        /// Draws all layers of the map
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the map.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        public void Draw(SpriteBatch spriteBatch, Camera gameCamera)
        {
            foreach (var l in Layers)
            {
                if (!l.Visible)
                    continue;

                DrawLayer(spriteBatch, l, gameCamera, Vector2.Zero, 1.0f, Color.White * l.Opacity);
            }
        }

        /// <summary>
        /// Draws all layers of the map
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the map.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        public void DrawAsMap(SpriteBatch spriteBatch, int scale)
        {
            foreach (var l in Layers)
            {
                if (!l.Visible)
                    continue;

                TileLayer tileLayer = l as TileLayer;
                if (tileLayer != null)
                {

                    for (int y = 0; y < Height; y++)
                    {
                        for (int x = 0;x<Width ; x++)
                        {
                            if (x >= 0 && x < tileLayer.Width && y >= 0 && y < tileLayer.Height)
                            {
                                Tile tile = tileLayer.Tiles[x, y];

                                if (tile != null && tile.Texture != null)
                                {
                                    spriteBatch.Draw(tile.Texture, new Rectangle(x * (int)(TileWidth / scale), y * (int)(TileHeight / scale), (int)(TileWidth / scale), (int)(TileHeight / scale)), tile.Source, Color.White);
                                }
                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Draws a single layer by layer name
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the layer.</param>
        /// <param name="layerName">The name of the layer to draw.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        public void DrawLayer(SpriteBatch spriteBatch, string layerName, Camera gameCamera)
        {
            var l = GetLayer(layerName);

            if (l == null)
                return;

            if (!l.Visible)
                return;

            TileLayer tileLayer = l as TileLayer;
			if (tileLayer != null)
			{
                if (tileLayer.Properties.Contains("Shadows"))
                    DrawLayer(spriteBatch, tileLayer.Name, gameCamera, new Vector2(-10f, -10f), 0.2f);

                DrawLayer(spriteBatch, l, gameCamera, new Vector2(0,0), tileLayer.Opacity, Color.White);
            }
        }



        /// <summary>
        /// Draws a single layer by layer name, with a specified color
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the layer.</param>
        /// <param name="layerName">The name of the layer to draw.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        public void DrawLayer(SpriteBatch spriteBatch, string layerName, Camera gameCamera, Color color)
        {
            var l = GetLayer(layerName);

            if (l == null)
                return;

            if (!l.Visible)
                return;

            TileLayer tileLayer = l as TileLayer;
            if (tileLayer != null)
            {
                if(tileLayer.Properties.Contains("Shadows"))
                    DrawLayer(spriteBatch, tileLayer.Name, gameCamera, new Vector2(-10f, -10f), 0.2f);

                DrawLayer(spriteBatch, l, gameCamera, new Vector2(0, 0), tileLayer.Opacity, color);
            }
        }

        /// <summary>
        /// Draws a single layer as shadows, by layer name
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the layer.</param>
        /// <param name="layerName">The name of the layer to draw.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        /// <param name="shadowOffset">Pixel amount to offset the shadowing by.</param>
        /// <param name="alpha">Shadow opacity</param>
        public void DrawLayer(SpriteBatch spriteBatch, string layerName, Camera gameCamera, Vector2 shadowOffset, float alpha)
        {
            var l = GetLayer(layerName);

            if (l == null)
                return;

            if (!l.Visible)
                return;

            TileLayer tileLayer = l as TileLayer;
			if (tileLayer != null)
			{
                //for (float mult = 0f; mult < 1f; mult += 0.1f)
                //{
                    DrawLayer(spriteBatch, l, gameCamera, shadowOffset, alpha, Color.Black);
                //DrawLayer(spriteBatch, l, gameCamera, shadowOffset, alpha, Color.Black);
                //}
                //DrawLayer(spriteBatch, l, gameCamera, shadowOffset * 0.5f, alpha * 0.75f, Color.Black);
                //DrawLayer(spriteBatch, l, gameCamera, shadowOffset * 0.75f, alpha * 0.5f, Color.Black);
                //DrawLayer(spriteBatch, l, gameCamera, shadowOffset, alpha *0.25f, Color.Black);
            }
        }


        /// <summary>
        /// Draws a single layer of the map
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch to use to render the layer.</param>
        /// <param name="tileLayer">The layer to draw.</param>
        /// <param name="gameCamera">The camera to use for positioning.</param>
        /// <param name="offset">A pixel amount to offset the tile positioning by</param>
        /// <param name="alpha">Layer opacity.</param>
        /// <param name="color">The color to use when drawing.</param>
        public void DrawLayer(SpriteBatch spriteBatch, Layer layer, Camera gameCamera, Vector2 offset, float alpha, Color color)
        {
            if (!layer.Visible)
                return;

            TileLayer tileLayer = layer as TileLayer;
            if (tileLayer != null)
            {
                Rectangle worldArea = new Rectangle((int)gameCamera.Position.X - (gameCamera.Width+200), (int)gameCamera.Position.Y - (int)(gameCamera.Height*1.5), (gameCamera.Width *2)+400, gameCamera.Height*3);

                // figure out the min and max tile indices to draw
                int minX = Math.Max((int)Math.Floor((float)worldArea.Left / TileWidth), 0);
                int maxX = Math.Min((int)Math.Ceiling((float)worldArea.Right / TileWidth), Width);

                int minY = Math.Max((int)Math.Floor((float)worldArea.Top / TileHeight), 0);
                int maxY = Math.Min((int)Math.Ceiling((float)worldArea.Bottom / TileHeight), Height);

                
                for (int x = minX; x < maxX; x++)
                {
                    for (int y = minY; y < maxY; y++)
                    {
                        //if ((new Vector2((x * TileWidth) + (TileWidth/2), (y * TileHeight) + (TileHeight/2)) - new Vector2(worldArea.Center.X, worldArea.Center.Y)).Length() < gameCamera.Width * 3f)
                        //{
                            Tile tile = tileLayer.Tiles[x, y];

                            if (tile == null)
                                continue;
                            // - tile.Source.Height + TileHeight;
                            Rectangle r = new Rectangle(x * TileWidth, y * TileHeight, tile.Source.Width, tile.Source.Height);


                            spriteBatch.Draw(tile.Texture, r, tile.Source, color);
                        //}

                    }
                }
                    

            }

        }

        

        public bool? CheckCollision(Vector2 position)
        {
            for(int i=0;i<Layers.Count;i++)
            {
                if (!Layers[i].Properties.Contains("Collision"))
                    continue;

                TileLayer tileLayer = Layers[i] as TileLayer;

                position.X = (int)position.X;
                position.Y = (int)position.Y;

                Vector2 tilePosition = new Vector2((int)(position.X / TileWidth), (int)(position.Y / TileHeight));

                if (tilePosition.X < 0 || tilePosition.Y < 0 || tilePosition.X > Width - 1 || tilePosition.Y > Height - 1)
                    continue;

                Tile collisionTile = tileLayer.Tiles[(int)tilePosition.X, (int)tilePosition.Y];

                if (collisionTile == null)
                    continue;

                if (collisionTile.CollisionData != null)
                {
                    int positionOnTileX = ((int)position.X - (((int)position.X / TileWidth) * TileWidth));
                    int positionOnTileY = ((int)position.Y - (((int)position.Y / TileHeight) * TileHeight));
                    positionOnTileX = (int)MathHelper.Clamp(positionOnTileX, 0, TileWidth);
                    positionOnTileY = (int)MathHelper.Clamp(positionOnTileY, 0, TileHeight);

                    int pixelCheckX = (collisionTile.Source.X) + positionOnTileX;
                    int pixelCheckY = (collisionTile.Source.Y) + positionOnTileY;

                    return collisionTile.CollisionData[(pixelCheckY * collisionTile.Texture.Width) + pixelCheckX];
                }
                else
                {
                    continue;
                }
            }

            return null;
        }

        public bool CheckTileCollision(Vector2 position)
        {
            for(int i=0;i<Layers.Count;i++)
            {
                if (!Layers[i].Properties.Contains("Collision"))
                    continue;

                TileLayer tileLayer = Layers[i] as TileLayer;

                position.X = (int)position.X;
                position.Y = (int)position.Y;

                Vector2 tilePosition = new Vector2((int)(position.X / TileWidth), (int)(position.Y / TileHeight));

                if (tilePosition.X < 0 || tilePosition.Y < 0 || tilePosition.X > Width - 1 || tilePosition.Y > Height - 1)
                    continue;

                Tile collisionTile = tileLayer.Tiles[(int)tilePosition.X, (int)tilePosition.Y];

                if (collisionTile == null)
                    return false;
                else return true;
            }

            return false;
        }

        public Rectangle? CheckTileCollisionIntersect(Vector2 position, Rectangle rect)
        {
            for (int i = 0; i < Layers.Count; i++)
            {
                if (!Layers[i].Properties.Contains("Collision"))
                    continue;

                TileLayer tileLayer = Layers[i] as TileLayer;

                position.X = (int)position.X;
                position.Y = (int)position.Y;

                Vector2 tilePosition = new Vector2((int)(position.X / TileWidth), (int)(position.Y / TileHeight));

                if (tilePosition.X < 0 || tilePosition.Y < 0 || tilePosition.X > Width - 1 || tilePosition.Y > Height - 1)
                    continue;

                Tile collisionTile = tileLayer.Tiles[(int)tilePosition.X, (int)tilePosition.Y];

                if (collisionTile == null)
                    return null;
                else return Rectangle.Intersect(rect, new Rectangle((int)tilePosition.X * TileWidth, (int)tilePosition.Y * TileHeight, TileWidth, TileHeight));
            }

            return null;
        }

        public void UnloadContent()
        {
            for(int i=0;i<Tiles.Count;i++)
            {
                if (Tiles[i] != null)
                {
                    Tiles[i].UnloadContent();
                }

                Tiles[i] = null;
            }
        }
    }
}
