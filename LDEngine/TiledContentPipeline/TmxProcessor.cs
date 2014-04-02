using System.ComponentModel;
using System.IO;
using System.Xml;
using System.Drawing;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Graphics;

namespace TiledContentPipeline
{
	[ContentProcessor(DisplayName = "TMX Processor")]
	public class TmxProcessor : ContentProcessor<XmlDocument, MapContent>
	{
		[DisplayName("TileSet Directory")]
		[Description("The directory (relative to the content root) in which the processor will find the tile sheet images.")]
		public string TileSetDirectory { get; set; }

		public override MapContent Process(XmlDocument input, ContentProcessorContext context)
		{
			// get our MapContent which does all the parsing of the XmlDocument we need
			MapContent content = new MapContent(input);

			// now we do some processing on tile sets to load external textures and figure out tile regions
			foreach (var tileSet in content.TileSets)
			{
				// get the full path to the file
				string path = string.IsNullOrEmpty(TileSetDirectory) ? tileSet.Image : Path.Combine(TileSetDirectory, tileSet.Image);
				string asset = path.Remove(path.LastIndexOf('.'));
				path = Path.Combine(Directory.GetCurrentDirectory(), path);

                //if (path.StartsWith("\\")) path = path.Substring(1);
                //if (asset.StartsWith("\\")) asset = asset.Substring(1);


				// build the asset as an external reference
				OpaqueDataDictionary data = new OpaqueDataDictionary();
				data.Add("GenerateMipmaps", false);
				data.Add("ResizeToPowerOfTwo", false);
				data.Add("TextureFormat", TextureProcessorOutputFormat.Color);
				data.Add("ColorKeyEnabled", tileSet.ColorKey.HasValue);
				data.Add("ColorKeyColor", tileSet.ColorKey.HasValue ? tileSet.ColorKey.Value : Microsoft.Xna.Framework.Color.Magenta);
				tileSet.Texture = context.BuildAsset<TextureContent, TextureContent>(
					new ExternalReference<TextureContent>(path),
					"TextureProcessor",
					data,
					"TextureImporter",
					asset);

				// load the image so we can compute the individual tile source rectangles
				int imageWidth = 0;
				int imageHeight = 0;
				using (Image image = Image.FromFile(path))
				{
					imageWidth = image.Width;
					imageHeight = image.Height;
				}

				// remove the margins from our calculations
				imageWidth -= tileSet.Margin * 2;
				imageHeight -= tileSet.Margin * 2;

				// figure out how many frames fit on the X axis
				int frameCountX = 1;
				while (frameCountX * tileSet.TileWidth < imageWidth)
				{
					frameCountX++;
					imageWidth -= tileSet.Spacing;
				}

				// figure out how many frames fit on the Y axis
				int frameCountY = 1;
				while (frameCountY * tileSet.TileHeight < imageHeight)
				{
					frameCountY++;
					imageHeight -= tileSet.Spacing;
				}

				// make our tiles. tiles are numbered by row, left to right.
				for (int y = 0; y < frameCountY; y++)
				{
					for (int x = 0; x < frameCountX; x++)
					{
						Tile tile = new Tile();

						// calculate the source rectangle
						int rx = tileSet.Margin + x * (tileSet.TileWidth + tileSet.Spacing);
						int ry = tileSet.Margin + y * (tileSet.TileHeight + tileSet.Spacing);
						tile.Source = new Microsoft.Xna.Framework.Rectangle(rx, ry, tileSet.TileWidth, tileSet.TileHeight);

						// get any properties from the tile set
						if (tileSet.TileProperties.ContainsKey(y + x))
						{
							tile.Properties = tileSet.TileProperties[y + x];
						}

						// save the tile
						tileSet.Tiles.Add(tile);
					}
				}
			}

			return content;
		}
	}
}
