namespace SolidCompany.Wrappers.Logging.Abstractions.Internals
{
    internal sealed class EmptyDepencendyLogger : IDepencencyLogger
    {
        public ILoggingScope CreateScope(string dependencyName)
        {
            return new EmptyLoggingScope();
        }

        private sealed class EmptyLoggingScope : ILoggingScope
        {
            public string Target { get; set; }
            public string Type { get; set; }
            public string Data { get; set; }
            public string ResultCode { get; set; }
            public bool? Success { get; set; }

            public void Dispose()
            {
            }
        }
    }
}