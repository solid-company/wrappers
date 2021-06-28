using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;
using SolidCompany.Wrappers.Logging.Abstractions;

namespace SolidCompany.Wrappers.Logging.ApplicationInsights
{
    /// <summary>
    /// Provides support for Azure Application Insights depencency logging.
    /// </summary>
    public sealed class ApplicationInsightsDependencyLogger : IDepencencyLogger
    {
        private readonly TelemetryClient telemetryClient;

        /// <summary>
        /// Creates a new instance of <see cref="ApplicationInsightsDependencyLogger"/> with default telemetry configuration.
        /// </summary>
        public ApplicationInsightsDependencyLogger()
        {
            this.telemetryClient = new TelemetryClient(TelemetryConfiguration.CreateDefault());
        }

        /// <summary>
        /// Creates a new instance of <see cref="ApplicationInsightsDependencyLogger"/>
        /// </summary>
        /// <param name="telemetryClient">Telemetry client provided via dependency injection</param>
        public ApplicationInsightsDependencyLogger(TelemetryClient telemetryClient)
        {
            this.telemetryClient = telemetryClient;
        }

        /// <inheritdoc />
        public ILoggingScope CreateScope(string dependencyName)
        {
            var dependency = telemetryClient.StartOperation<DependencyTelemetry>(dependencyName);

            return new ApplicationInsightsLoggingScope(dependency);
        }

        private sealed class ApplicationInsightsLoggingScope : ILoggingScope
        {
            private readonly IOperationHolder<DependencyTelemetry> operationHolder;
            private readonly DependencyTelemetry dependency;

            public ApplicationInsightsLoggingScope(IOperationHolder<DependencyTelemetry> operationHolder)
            {
                this.operationHolder = operationHolder;
                this.dependency = operationHolder.Telemetry;
            }

            public string Target
            {
                get => dependency.Target;
                set => dependency.Target = value;
            }

            public string Data
            {
                get => dependency.Data;
                set => dependency.Data = value;
            }

            public string ResultCode
            {
                get => dependency.ResultCode;
                set => dependency.ResultCode = value;
            }

            public bool? Success
            {
                get => dependency.Success;
                set => dependency.Success = value;
            }

            public void Dispose()
            {
                operationHolder.Dispose();
            }
        }
    }
}