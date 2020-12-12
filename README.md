# Overview
This is an example of a simple bare bones http server using Kestrel without other ASP.NET features. It's similar to an HttpListener application in that there is a method to process the http request.

# How I created the project
1. In Visual Studio, create a new **ASP.NET Core Web Application** project using the built in template/wizard
2. Edit the .csproj file, set `<OutputType>Exe</OutputType>` in the `PropertyGroup`
3. Remove the other files except Program.cs and launchSettings.json
4. Update Main in Program.cs to
```
    var kestrelServer = new KestrelServer(...);
    kestrelServer.StartAsync(...);
```
5. Add implementation classes for IHttpApplication<TContext>, IOptions<KestrelServerOptions>, IOptions<SocketTransportOptions>, ILoggerFactory, ILogger
6. In your IHttpApplication class, process the requests using either your own cusom HttpContext object, or the original IFeatureCollection
7. Remove the IIS stuff from launchSettings.json and make it just launch the project without a browser
8. Optional: if you want to, change `<Project Sdk="Microsoft.NET.Sdk.Web">` to `<Project Sdk="Microsoft.NET.Sdk">` in your csproj file and add all the references yourself and remove launchSettings.json

# Why not use normal ASP.NET stuff like MVC, Razor pages, etc?
Maybe you have an existing HttpListener application and you want to make it able to run on Linux with minimul changes.

Maybe you want to run some benchmarks or are just curious to see what the minimal project looks like
