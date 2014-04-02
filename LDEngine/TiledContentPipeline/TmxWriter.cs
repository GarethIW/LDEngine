using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;
using Microsoft.Xna.Framework.Content.Pipeline;
using System.Collections.Generic;

namespace TiledContentPipeline
{
	[ContentTypeWriter]
	public class TmxWriter : ContentTypeWriter<MapContent>
	{
		protected override void Write(ContentWriter output, MapContent value)
		{
			// write the map information
			output.Write(value.Version);
			output.Write((byte)value.Orientation);
			output.Write(value.Width);
			output.Write(value.Height);
			output.Write(value.TileWidth);
			output.Write(value.TileHeight);
			WritePropertyList(output, value.Properties);

			// write out our tile sets. we don't use a runtime TileSet, we read in as
			// a list of tile objects. so we don't write out a lot of the TileSet
			// information as most of it isn't useful.
			output.Write(value.TileSets.Count);
			foreach (var tileSet in value.TileSets)
			{
				// write out the first global ID of these tiles
				output.Write(tileSet.FirstId);

                // write out the name of the tileset
                output.Write(tileSet.Name);

                output.Write(tileSet.CollisionSet);

				// write out the texture used by the tiles
				output.WriteExternalReference(tileSet.Texture);

				// write out all the tiles in the tile set
				output.Write(tileSet.Tiles.Count);
				foreach (var tile in tileSet.Tiles)
				{
					output.WriteObject(tile.Source);
					WritePropertyList(output, tile.Properties);
				}
			}

			// write each layer
			output.Write(value.Layers.Count);
			foreach (var layer in value.Layers)
			{
				// basic information
				output.Write(layer.Type);
				output.Write(layer.Name);
				output.Write(layer.Width);
				output.Write(layer.Height);
				output.Write(layer.Visible);
				output.Write(layer.Opacity);
				WritePropertyList(output, layer.Properties);

				// figure out specific type of layer
				TileLayerContent tileLayer = layer as TileLayerContent;
				MapObjectLayerContent objLayer = layer as MapObjectLayerContent;

				// tile layers just write out index data
				if (tileLayer != null)
				{
					output.WriteObject(tileLayer.Data);
				}

				// object layers write out all the objects
				else if (objLayer != null)
				{
					output.Write(objLayer.Objects.Count);
					foreach (var mapObj in objLayer.Objects)
					{
						output.Write(mapObj.Name);
						output.Write(mapObj.Type);
						output.WriteObject(mapObj.Location);
                        output.WriteObject(mapObj.LinePoints);
						WritePropertyList(output, mapObj.Properties);
					}
				}
			}
		}

		// helper for writing out property lists
		private void WritePropertyList(ContentWriter writer, List<Property> properties)
		{
			writer.Write(properties.Count);
			foreach (var p in properties)
			{
				writer.Write(p.Name);
				writer.Write(p.Value);
			}
		}

		public override string GetRuntimeReader(TargetPlatform targetPlatform)
		{
			return "TiledLib.MapReader, TiledLib";
		}

		public override string GetRuntimeType(TargetPlatform targetPlatform)
		{
			return "TiledLib.Map, TiledLib";
		}
	}
}
