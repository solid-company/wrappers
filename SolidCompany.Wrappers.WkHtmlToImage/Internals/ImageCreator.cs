using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SolidCompany.Wrappers.Logging.Abstractions;

namespace SolidCompany.Wrappers.WkHtmlToImage.Internals
{
    internal sealed class ImageCreator : IDisposable
    {
        private readonly ILogger<ImageCreator> logger;
        private readonly IDepencencyLogger depencencyLogger;
        private readonly string wkhtmlToImpageExePath;
        private readonly string workdir;
        private readonly TimeSpan executionTimeout;

        public ImageCreator(ILogger<ImageCreator> logger,
            IDepencencyLogger depencencyLogger,
            string wkhtmlToImpageExePath,
            string workdir,
            TimeSpan executionTimeout)
        {
            this.logger = logger;
            this.depencencyLogger = depencencyLogger;
            this.wkhtmlToImpageExePath = wkhtmlToImpageExePath;
            this.workdir = workdir;
            this.executionTimeout = executionTimeout;
        }

        public async Task<Stream> CreateAsync(string htmlImput, int widthPx, ImageFormat imageFormat, CancellationToken cancellationToken = default)
        {
            if (!Directory.Exists(workdir))
                Directory.CreateDirectory(workdir);

            var inputFilePath = await CreateInputFileAsync(htmlImput, cancellationToken);
            var outputFilePath = Path.Combine(workdir, $"{Path.GetRandomFileName()}.{imageFormat.Name}");

            var parameterBuilder = new StringBuilder();

            parameterBuilder.AppendFormat(" -f {0}", imageFormat.Name);
            parameterBuilder.AppendFormat(" --width {0}", widthPx);
            parameterBuilder.AppendFormat(" \"{0}\"", inputFilePath);
            parameterBuilder.AppendFormat(" \"{0}\"", outputFilePath);

            var arguments = parameterBuilder.ToString();

            var processStartInfo = new ProcessStartInfo(wkhtmlToImpageExePath, arguments)
            {
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                UseShellExecute = false,
                WorkingDirectory = workdir,
                RedirectStandardError = true
            };

            var process = new Process();
            process.StartInfo = processStartInfo;

            using var dependencyScope = depencencyLogger.CreateScope(HtmlToImage.ApplicationName);

            dependencyScope.Target = HtmlToImage.ApplicationName;
            dependencyScope.Data = arguments;

            try
            {
                process.Start();

                logger.LogDebug("wkhtmltoimage.exe started.");

                process.WaitForExit((int) executionTimeout.TotalMilliseconds);

                if (!process.HasExited)
                    throw new TimeoutException($"wkhtmltoimage.exe running timeout expired. If this exception is thrown frequently consider chaning current timeout ({executionTimeout:g} to higher value.");

                dependencyScope.ResultCode = process.ExitCode.ToString();
                dependencyScope.Success = process.ExitCode == 0;

                logger.LogDebug("wkhtmltoimage.exe exited with code \"{ExitCode}\".", process.ExitCode);

                return await GetOutputStreamAsync(outputFilePath, cancellationToken);
            }
            catch (TimeoutException e)
            {
                logger.LogError(e, "HTML to image conversion failed - timeout expired.");

                dependencyScope.Success = false;

                throw;
            }
            catch (Exception e)
            {
                logger.LogError(e, "HTML to image conversion failed.");

                dependencyScope.Success = false;

                throw;
            }
        }

        private static async Task<MemoryStream> GetOutputStreamAsync(string outputFilePath, CancellationToken cancellationToken)
        {
            await using var result = File.OpenRead(outputFilePath);
            var outputStream = new MemoryStream();

            await result.CopyToAsync(outputStream, cancellationToken);
            outputStream.Position = 0;

            return outputStream;
        }


        private async Task<string> CreateInputFileAsync(string htmlImput, CancellationToken cancellationToken)
        {
            var inputBytes = Encoding.UTF8.GetBytes(htmlImput);

            // wkhtmltoimage doesn't support random extension names
            var inputFilePath = Path.Combine(workdir, Path.GetRandomFileName() + ".htm");

            await using var inputStream = File.Create(inputFilePath);
            await inputStream.WriteAsync(inputBytes, 0, inputBytes.Length, cancellationToken);

            inputStream.Close();

            return inputFilePath;
        }

        public void Dispose()
        {
            try
            {
                Directory.Delete(workdir, true);
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed deleting a workdir directory \"{WorkdirDirectory}\".", workdir);
                
                throw;
            }
        }
    }
}