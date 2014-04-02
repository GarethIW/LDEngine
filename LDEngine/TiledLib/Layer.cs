namespace TiledLib
{
	/// <summary>
	/// An abstract base for a layer in a map.
	/// </summary>
	public abstract class Layer
	{
		/// <summary>
		/// Gets the name of the layer.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the width (in tiles) of the layer.
		/// </summary>
		public int Width { get; private set; }

		/// <summary>
		/// Gets the height (in tiles) of the layer.
		/// </summary>
		public int Height { get; private set; }

		/// <summary>
		/// Gets or sets the whether the layer is visible.
		/// </summary>
		public bool Visible { get; set; }

		/// <summary>
		/// Gets or sets the opacity of the layer.
		/// </summary>
		public float Opacity { get; set; }

		/// <summary>
		/// Gets the list of properties for the layer.
		/// </summary>
		public PropertyCollection Properties { get; private set; }

		internal Layer(string name, int width, int height, bool visible, float opacity, PropertyCollection properties)
		{
			Name = name;
			Width = width;
			Height = height;
			Visible = visible;
			Opacity = opacity;
			Properties = properties;
		}
	}
}
