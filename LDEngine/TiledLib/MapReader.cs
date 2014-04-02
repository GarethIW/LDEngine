using Microsoft.Xna.Framework.Content;

namespace TiledLib
{
	/// <summary>
	/// Reads in a Map from an XNB.
	/// </summary>
	public sealed class MapReader : ContentTypeReader<Map>
	{
		protected override Map Read(ContentReader input, Map existingInstance)
		{
			return new Map(input);
		}
	}
}
