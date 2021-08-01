using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SolidCompany.Wrappers.Logging.Abstractions;

namespace SolidCompany.Wrappers.WkHtmlToImage.Registration
{
    /// <summary>
    /// Extension methods for registering HTML-to-image conversion in an <see cref="IServiceCollection" />.
    /// </summary>
    public static class RegistrationExtensions
    {
        /// <summary>
        /// Adds image conversion services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> instance.</param>
        /// The <see cref="IServiceCollection"/>.
        public static IServiceCollection AddHtmlToImageConversion(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddHtmlToImageConversion(options => { });

            return serviceCollection;
        }

        /// <summary>
        /// Adds image conversion services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="configure">An <see cref="Action{HtmlToImageOptions}"/> to configure the provided <see cref="HtmlToImageOptions"/>.</param>
        /// The <see cref="IServiceCollection"/>.
        public static IServiceCollection AddHtmlToImageConversion(this IServiceCollection serviceCollection, Action<HtmlToImageOptions> configure)
        {
            serviceCollection.AddSingleton<IHtmlToImage, HtmlToImage>(sp =>
            {
                var options = new HtmlToImageOptions();
                configure(options);

                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                return new HtmlToImage(options, loggerFactory);
            });

            return serviceCollection;
        }


        /// <summary>
        /// Adds image conversion services to the specified <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="serviceCollection">The <see cref="IServiceCollection"/> instance.</param>
        /// <param name="configure">An <see cref="Action{HtmlToImageOptions}"/> to configure the provided <see cref="HtmlToImageOptions"/>.</param>
        /// The <see cref="IServiceCollection"/>.
        public static IServiceCollection AddHtmlToImageConversion(this IServiceCollection serviceCollection, Action<IServiceProvider, HtmlToImageOptions> configure)
        {
            serviceCollection.AddSingleton<IHtmlToImage, HtmlToImage>(sp =>
            {
                var options = new HtmlToImageOptions();
                configure(sp, options);

                var loggerFactory = sp.GetRequiredService<ILoggerFactory>();

                return new HtmlToImage(options, loggerFactory);
            });

            return serviceCollection;
        }
    }

    /// <summary>
    /// Represents an HTML-to-image to configuration options.
    /// </summary>
    public sealed class HtmlToImageOptions 
    {
        /// <summary>
        /// An execution directory - a path where wkhtmltoimage.exe is executed. If not specified then <see cref="TempExectuionDirectory"/> is used.
        /// </summary>
        public IExectuionDirectory ExectuionDirectory { get; set; }

        /// <summary>
        /// An execution timeout in seconds. Default 30 seconds. When timeout expires then <see cref="TimeoutException"/> is thrown.
        /// </summary>
        public TimeSpan ExecutionTimeout { get; set; }

        /// <summary>
        /// A dependency logger for tracking wkhtmltoimage.exe runs. If not specified then <see cref="Logging.Abstractions.DependencyLogger.Empty"/> is used.
        /// </summary>
        public IDepencencyLogger DependencyLogger { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="HtmlToImageOptions"/>.
        /// </summary>
        public HtmlToImageOptions()
        {
            ExectuionDirectory = new TempExectuionDirectory();
            DependencyLogger = Logging.Abstractions.DependencyLogger.Empty;
            ExecutionTimeout = TimeSpan.FromSeconds(30);
        }
    }
}