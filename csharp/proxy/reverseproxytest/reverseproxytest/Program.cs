using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets;
using Microsoft.Extensions.ObjectPool;
using System.Net;
using System.Security.Authentication;

namespace reverseproxytest
{
    static class ConnectionFactoryTypeUtil
    {
        private const string socketConnectionFactoryTypeName = "Microsoft.AspNetCore.Server.Kestrel.Transport.Sockets.SocketConnectionFactory";

        /// <summary>
        /// 查找SocketConnectionFactory的类型
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotSupportedException"></exception>
        public static Type FindSocketConnectionFactory()
        {
            var assembly = typeof(SocketTransportOptions).Assembly;
            var connectionFactoryType = assembly.GetType(socketConnectionFactoryTypeName);
            return connectionFactoryType ?? throw new NotSupportedException($"找不到类型{socketConnectionFactoryTypeName}");
        }
    }

    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSocketConnectionFactory(this IServiceCollection services)
        {
            var factoryType = ConnectionFactoryTypeUtil.FindSocketConnectionFactory();
            return services.AddSingleton(typeof(IConnectionFactory), factoryType);
        }
    }

    public class ProxyHandler : ConnectionHandler
    {
        private readonly ILogger<ProxyHandler> logger;
        private readonly IConnectionFactory connectionFactory;
        private readonly IPEndPoint proxyServer = new(IPAddress.Parse("14.215.177.38"), 80);
        private ConnectionContext upstream;
        public ProxyHandler(ILogger<ProxyHandler> logger,
            IConnectionFactory connectionFactory)
        {
            this.logger = logger;
            this.connectionFactory = connectionFactory;
        }

        public override async Task OnConnectedAsync(ConnectionContext connection)
        {
            upstream = await connectionFactory.ConnectAsync(proxyServer);
            var task1 = connection.Transport.Input.CopyToAsync(upstream.Transport.Output);
            var task2 = upstream.Transport.Input.CopyToAsync(connection.Transport.Output);
            await Task.WhenAny(task1, task2);
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureServices(ser => 
            {
                ser.AddSocketConnectionFactory();
            }).ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.Listen(IPAddress.Loopback, 5000, listenOptions =>
                {
                    listenOptions.UseConnectionHandler<ProxyHandler>();
                });
                
            });


            var app = builder.Build();


            app.Run();
        }
    }
}