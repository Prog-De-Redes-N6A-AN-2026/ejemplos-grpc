using GrpcServer;
using GrpcServer.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenLocalhost(5191, listen =>
        listen.Protocols = HttpProtocols.Http2);
});

builder.Services.AddGrpc();
builder.Services.AddSingleton<AlmacenTokens>();

var app = builder.Build();

app.MapGrpcService<ServicioAutenticacion>();

Console.WriteLine("Servidor gRPC escuchando en http://localhost:5191");
app.Run();
