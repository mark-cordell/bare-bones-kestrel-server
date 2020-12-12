using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BareBonesKestrel
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var loggerFactory = new NoOpLoggerFactory();
            var kestrelServer = new KestrelServer(new ConfigureKestrelServerOptions(), new SocketTransportFactory(new ConfigureSocketTransportOptions(), loggerFactory), loggerFactory);
            kestrelServer.Options.ListenLocalhost(8080);
            kestrelServer.StartAsync(new HttpApp(), CancellationToken.None).GetAwaiter().GetResult();

            // todo: you can update this code to do some other processing on this thread, and you can remove the call to StopAsync() if you don't need it.
            Thread.Sleep(60000);
            Console.WriteLine("shutting down");
            kestrelServer.StopAsync(CancellationToken.None).GetAwaiter().GetResult();
        }

        private class HttpApp : IHttpApplication<IFeatureCollection>
        {
            private readonly byte[] helloWorldBytes = Encoding.UTF8.GetBytes("hello world");

            public IFeatureCollection CreateContext(IFeatureCollection contextFeatures)
            {
                return contextFeatures;
            }

            public void DisposeContext(IFeatureCollection context, Exception exception)
            {
            }

            public async Task ProcessRequestAsync(IFeatureCollection features)
            {
                var request = (IHttpRequestFeature)features[typeof(IHttpRequestFeature)];
                var response = (IHttpResponseFeature)features[typeof(IHttpResponseFeature)];
                var responseBody = (IHttpResponseBodyFeature)features[typeof(IHttpResponseBodyFeature)];
                ////var connection = (IHttpConnectionFeature)features[typeof(IHttpConnectionFeature)];
                response.Headers.ContentLength = this.helloWorldBytes.Length;
                response.Headers.Add("Content-Type", "text/plain");
                await responseBody.Stream.WriteAsync(this.helloWorldBytes, 0, this.helloWorldBytes.Length);
            }
        }

        private sealed class NoOpLogger : ILogger, IDisposable
        {
            public IDisposable BeginScope<TState>(TState state)
            {
                return this;
            }

            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
            {
            }

            public void Dispose()
            {
            }
        }

        private sealed class NoOpLoggerFactory : ILoggerFactory
        {
            private readonly ILogger logger = new NoOpLogger();

            public void Dispose()
            {
            }

            public void AddProvider(ILoggerProvider provider)
            {
            }

            public ILogger CreateLogger(string categoryName)
            {
                return this.logger;
            }
        }

        private sealed class ConfigureKestrelServerOptions : IOptions<KestrelServerOptions>
        {
            public ConfigureKestrelServerOptions()
            {
                this.Value = new KestrelServerOptions()
                {
                };
            }

            public KestrelServerOptions Value { get; }
        }

        public sealed class ConfigureSocketTransportOptions : IOptions<SocketTransportOptions>
        {
            public ConfigureSocketTransportOptions()
            {
                this.Value = new SocketTransportOptions()
                {
                };
            }

            public SocketTransportOptions Value { get; }
        }
    }
}
