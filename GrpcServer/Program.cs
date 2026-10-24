using GrpcServer.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

namespace GrpcServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.ConfigureKestrel(options =>
            {
                options.ListenLocalhost(5191, o => o.Protocols = HttpProtocols.Http2);
            });

            builder.Services.AddGrpc();
            builder.Services.AddSingleton<ContadorIncremental>();

            var app = builder.Build();

            app.MapGrpcService<ServicioContador>();
            app.MapGet("/", () => "Los endpoints gRPC deben consumirse mediante un cliente gRPC.");

            app.Run();
        }
    }
}
