using System;

namespace SolidCompany.Wrappers.Logging.Abstractions
{
    /// <summary>
    /// Represents a logging scope for logging a dependency call.
    /// </summary>
    public interface ILoggingScope : IDisposable
    {
        /// <summary>
        /// Gets or sets logging Target.
        /// </summary>
        string Target { get; set; }

        /// <summary>
        /// Gets or sets Data to be logged.
        /// </summary>
        string Data { get; set; }

        /// <summary>
        /// Gets or sets ResultCode.
        /// </summary>
        string ResultCode { get; set; }

        /// <summary>
        /// Gets or sets Success.
        /// </summary>
        bool? Success { get; set; }
    }
}