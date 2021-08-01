using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using SolidCompany.Wrappers.WkHtmlToImage.Registration;

namespace SolidCompany.Wrappers.WkHtmlToImage.Tests
{
    public class Tests
    {
        [Test]
        public async Task Can_generate_image()
        {
            var options = new HtmlToImageOptions
            {
                ExectuionDirectory = new CustomDirectory(TestContext.CurrentContext.WorkDirectory)
            };

            using var htmlToImage = new HtmlToImage(options, NullLoggerFactory.Instance);

            var stream = await htmlToImage.CreateImageAsync("<html><body style=\"background-color: red\"></body></html>", 100, ImageFormat.Png);

            Assert.That(stream.Length, Is.GreaterThan(0));
        }

        [Test]
        public async Task Can_generate_image_from_host()
        {
            var host = new HostBuilder()
                .ConfigureServices(services => services.AddHtmlToImageConversion((sp, options) => { }))
                .Build();

            var htmlToImage = host.Services.GetRequiredService<IHtmlToImage>();

            var stream = await htmlToImage.CreateImageAsync("<html><body style=\"background-color: red\"></body></html>", 100, ImageFormat.Png);

            Assert.That(stream.Length, Is.GreaterThan(0));
        }
    }
}