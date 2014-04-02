using System;
using System.IO;
using System.IO.Compression;
using System.Xml;

namespace TiledContentPipeline
{
	public class TileLayerContent : LayerContent
	{
		public int[] Data;

		public TileLayerContent(XmlNode node)
			: base(node)
		{
			XmlNode dataNode = node["data"];
			string encoding = dataNode.Attributes["encoding"].Value;
			string compression = dataNode.Attributes["compression"].Value;
			Data = new int[Width * Height];

			if (encoding != "base64")
			{
				throw new Exception("Unknown encoding: " + encoding);
			}

			Stream data = new MemoryStream(Convert.FromBase64String(node.InnerText), false);
			if (compression == "gzip")
			{
				data = new GZipStream(data, CompressionMode.Decompress, false);
			}

			using (data)
			{
				using (BinaryReader reader = new BinaryReader(data))
				{
					for (int i = 0; i < Data.Length; i++)
					{
						Data[i] = reader.ReadInt32();
					}
				}
			}
		}
	}
}
