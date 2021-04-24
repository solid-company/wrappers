using System;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SolidCompany.Wrappers.Logging.Abstractions;
using SolidCompany.Wrappers.WkHtmlToImage.Internals;
using SolidCompany.Wrappers.WkHtmlToImage.Registration;

namespace SolidCompany.Wrappers.WkHtmlToImage
{
    /// <inheritdoc cref="IHtmlToImage" />
    public sealed class HtmlToImage : IDisposable, IHtmlToImage
    {
        private readonly IExectuionDirectory exectuionDirectory;
        private readonly ILogger<HtmlToImage> logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly IDepencencyLogger depencencyLogger;
        private readonly TimeSpan executionTimeout;

        private readonly Task initializeTask;
        private readonly string workdir;
        private readonly string wkhtmlToImpageExePath;
        private readonly string resourceApplicationName;

        internal const string ApplicationName = "wkhtmltoimage.exe";
        
        /// <summary>
        /// Creates a new instance of <see cref="HtmlToImage"/> class.
        /// </summary>
        /// <param name="options">Options.</param>
        /// <param name="loggerFactory">Logger factory</param>
        public HtmlToImage(HtmlToImageOptions options, ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger<HtmlToImage>();
            this.loggerFactory = loggerFactory;
            this.exectuionDirectory = options.ExectuionDirectory ?? new TempExectuionDirectory();
            this.workdir = Path.Combine(exectuionDirectory.Directory, Path.GetRandomFileName() + ".dir");
            this.depencencyLogger = options.DependencyLogger;
            this.executionTimeout = options.ExecutionTimeout;

            if (!Directory.Exists(workdir))
                Directory.CreateDirectory(workdir);

            if (Environment.Is64BitProcess)
                resourceApplicationName = "SolidCompany.Wrappers.WkHtmlToImage.Resources.wkhtmltoimage-x64.zip";
            else
                resourceApplicationName = "SolidCompany.Wrappers.WkHtmlToImage.Resources.wkhtmltoimage-x86.zip";

            this.wkhtmlToImpageExePath = Path.Combine(exectuionDirectory.Directory, ApplicationName);

            this.initializeTask = InitializeAsync();
        }

        /// <inheritdoc />
        public async Task<Stream> CreateImageAsync(string htmlImput, int widthPx, ImageFormat imageFormat, CancellationToken cancellationToken = default)
        {
            await initializeTask;

            using var creator = new ImageCreator(
                loggerFactory.CreateLogger<ImageCreator>(),
                depencencyLogger,
                wkhtmlToImpageExePath,
                Path.Combine(exectuionDirectory.Directory, Path.GetRandomFileName()),
                executionTimeout
            );

            return await creator.CreateAsync(htmlImput, widthPx, imageFormat, cancellationToken);
        }

        private async Task InitializeAsync()
        {
            try
            {
                var destinationPath = Path.Combine(exectuionDirectory.Directory, ApplicationName);

                if (File.Exists(destinationPath))
                    logger.LogDebug($"Execution directory \"{{DirectoryName}}\" already exists. Overriding {ApplicationName} with latest version.", exectuionDirectory);
                else
                {
                    logger.LogDebug("Creating execution directory \"{DirectoryName}\".", exectuionDirectory);

                    exectuionDirectory.Create();

                    logger.LogDebug("Execution directory \"{DirectoryName}\" created.", exectuionDirectory);
                }
                
                await using var resourceZipStream = typeof(HtmlToImage).Assembly.GetManifestResourceStream(resourceApplicationName);

                using var zipArchive = new ZipArchive(resourceZipStream!, ZipArchiveMode.Read, leaveOpen: true);
                var entry = zipArchive.GetEntry(ApplicationName);

                await using var applicationStrem = entry!.Open();

                await using var destinationFileStream = File.Create(destinationPath);

                await applicationStrem.CopyToAsync(destinationFileStream);
            }
            catch (Exception e)
            {
                logger.LogError(e, "An error occured during initialization.");

                throw;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            initializeTask?.Dispose();

            try
            {
                logger.LogDebug("Deleting workdir \"{Workdkir}\".", workdir);

                Directory.Delete(workdir, true);

                logger.LogDebug("Workdir \"{Workdkir}\" deleted.", workdir);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Deleting workdir \"{Workdkir}\" failed.", workdir);
            }
        }
    }
}
