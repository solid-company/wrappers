using SolidCompany.Wrappers.Logging.Abstractions.Internals;

namespace SolidCompany.Wrappers.Logging.Abstractions
{
    /// <summary>
    /// Represents a dependency logger for tracking depencency call.
    /// </summary>
    public interface IDepencencyLogger
    {
        /// <summary>
        /// Creates a new logging scope.
        /// </summary>
        /// <param name="dependencyName">Name of the dependency to be logged.</param>
        /// <returns>Returns a new <see cref="ILoggingScope"/>.</returns>
        ILoggingScope CreateScope(string dependencyName);
    }
    
    /// <summary>
    /// Provides access to predefined <see cref="IDepencencyLogger"/> implementations.
    /// </summary>
    public static class DependencyLogger
    {
        /// <summary>
        /// An empty dependency logger.
        /// </summary>
        public static IDepencencyLogger Empty => new EmptyDepencendyLogger();
    }
}