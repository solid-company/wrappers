`SolidCompany.Wrappers.WkHtmlToImage` is a .NET Core wrapper for a part of an open-source project [wkhtmltopdf](https://wkhtmltopdf.org) which is `wkhtmltoimage`. It supports converting an HTML to images in a selected format.

## Get Packages

You can get `SolidCompany.Wrappers.WkHtmlToImage` package by [downloading it from NuGet feed](https://www.nuget.org/packages/SolidCompany.Wrappers.WkHtmlToImage).

## Getting Started

`SolidCompany.Wrappers.WkHtmlToImage` easily integrates with .NET Core Dependency Injection. You need only one line of code to get everything working:

```C#
public void ConfigureServices(IServiceCollection services)
{
    // ...
    
    services.AddHtmlToImageConversion();

    // ...
}
```

Now you are free to use this powerful tool by injecting `IHtmlToImage` into a constructor:

```c#
public class SampleService
{
    private readonly IHtmlToImage htmlToImage;

    public SampleService(IHtmlToImage htmlToImage)
    {
        this.htmlToImage = htmlToImage;
    }

    public async Task<Stream> ConvertHtmlToImageAsync(string html, int widthPx)
    {
        return await htmlToImage.CreateImageAsync(html, widthPx, ImageFormat.Png);
    }
}
```
Image height is automatically scaled to width which preserves a valid ratio.

## Configuration

You can pass a few additional options to configuration:

```C#

services.AddHtmlToImageConversion(options =>
{
    options.DependencyLogger = new ApplicationInsightsDependencyLogger();
    options.ExecutionTimeout = TimeSpan.FromMinutes(2);
    options.ExectuionDirectory = new CustomDirectory("C:/Temp");
});

```

`DependencyLogger` allows to track every wkhtmltoimage call with [Azure Application Insights](https://docs.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview). To use it you need [`SolidCompany.Wrappers.Logging.ApplicationInsights`](https://www.nuget.org/packages/SolidCompany.Wrappers.Logging.ApplicationInsights) package.

`ExecutionTimeout` lets you specify a maximum `wkhtmltoimage` execution time. Default is 30 seconds.

`ExectuionDirectory` specifies where the exe file is run and where temporary files are created. By default `%TEMP%\SolidCompany.Wrappers.WkHtmlToImage` directory is used.