using System.IO;

namespace SolidCompany.Wrappers.WkHtmlToImage.Registration
{
    /// <summary>
    /// Represents an execution directory - a path where wkhtmlimage.exe is run.
    /// </summary>
    public interface IExectuionDirectory
    {
        /// <summary>
        /// A path to execution directory.
        /// </summary>
        string Directory { get; }

        /// <summary>
        /// Created the execution directory.
        /// </summary>
        void Create();
    }

    /// <summary>
    /// A base implementation for <see cref="IExectuionDirectory"/> interface.
    /// </summary>
    public abstract class BaseExecutionDirectory : IExectuionDirectory
    {
        /// <inheritdoc />
        public abstract string Directory { get; }

        /// <inheritdoc />
        public virtual void Create()
        {
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);
        }
    }

    /// <summary>
    /// Represents a TEMP execution directory. User's TEMP environmental variable is used for a root followed by this assembly name.
    /// </summary>
    public sealed class TempExectuionDirectory : BaseExecutionDirectory
    {
        /// <inheritdoc />
        public override string Directory { get; } = Path.Combine(Path.GetTempPath(), typeof(TempExectuionDirectory).Assembly.ManifestModule.Name);
    }

    /// <summary>
    /// Represents a custom execution directory.
    /// </summary>
    public sealed class CustomDirectory : BaseExecutionDirectory
    {
        /// <inheritdoc />
        public override string Directory { get; }

        /// <summary>
        /// Creates a new instance of <see cref="CustomDirectory"/>.
        /// </summary>
        /// <param name="path">A path to be used for running wkhtmltoimage.exe.</param>
        public CustomDirectory(string path)
        {
            Directory = path;
        }
    }
}