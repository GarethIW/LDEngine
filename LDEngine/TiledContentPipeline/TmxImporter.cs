using System.Xml;
using Microsoft.Xna.Framework.Content.Pipeline;

namespace TiledContentPipeline
{
	[ContentImporter(".tmx", DisplayName = "TMX Importer", DefaultProcessor = "TmxProcessor")]
	public class TmxImporter : ContentImporter<XmlDocument>
	{
		public override XmlDocument Import(string filename, ContentImporterContext context)
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			return doc;
		}
	}
}
