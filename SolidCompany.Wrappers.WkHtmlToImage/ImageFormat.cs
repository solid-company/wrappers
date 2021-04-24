namespace SolidCompany.Wrappers.WkHtmlToImage
{
    /// <summary>
    /// Represents a set of supported image formats.
    /// </summary>
    public sealed class ImageFormat
    {
        /// <summary>
        /// PNG image format.
        /// </summary>
        public static ImageFormat Png => new ImageFormat("png");

        /// <summary>
        /// JPG image format.
        /// </summary>
        public static ImageFormat Jpg => new ImageFormat("jpg");

        /// <summary>
        /// BMP image format.
        /// </summary>
        public static ImageFormat Bmp => new ImageFormat("bmp");

        /// <summary>
        /// Image format name.
        /// </summary>
        public string Name { get; }

        private ImageFormat(string name)
        {
            Name = name;
        }
    }
}