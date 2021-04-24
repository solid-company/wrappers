using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SolidCompany.Wrappers.WkHtmlToImage
{
    /// <summary>
    /// Represents an HTML-to-image conversion service.
    /// </summary>
    public interface IHtmlToImage
    {
        /// <summary>
        /// Generates an image from HTML input.
        /// </summary>
        /// <param name="htmlImput">An HTML input to be converted to an image</param>
        /// <param name="widthPx">Image width.</param>
        /// <param name="imageFormat">Image format.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Returns a stream that contains the image in specified format.</returns>
        /// <remarks>Image height is automatically scaled to preserve image ratio.</remarks>
        Task<Stream> CreateImageAsync(string htmlImput, int widthPx, ImageFormat imageFormat, CancellationToken cancellationToken = default);
    }
}