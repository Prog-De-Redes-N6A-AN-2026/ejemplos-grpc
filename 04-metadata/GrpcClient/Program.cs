using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcAutenticacion;

using var canal = GrpcChannel.ForAddress("http://localhost:5191");
var cliente = new Autenticacion.AutenticacionClient(canal);

Console.WriteLine("Intentando acceder sin autenticación...");

try
{
    await cliente.ObtenerPerfilAsync(new Empty());
}
catch (RpcException ex)
{
    Console.WriteLine($"{ex.StatusCode}: {ex.Status.Detail}");
}

Console.WriteLine();
Console.WriteLine("Iniciando sesión...");

var login = await cliente.IniciarSesionAsync(new SolicitudLogin
{
    Usuario = "usuario",
    Contrasena = "clave"
});

Console.WriteLine(login.Mensaje);
Console.WriteLine($"Token: {login.Token}");

var metadata = new Metadata
{
    { "authorization", $"Bearer {login.Token}" }
};

Console.WriteLine();
Console.WriteLine("Solicitando perfil con Metadata...");

var perfil = await cliente.ObtenerPerfilAsync(
    new Empty(),
    headers: metadata);

Console.WriteLine($"Usuario: {perfil.Usuario}");
Console.WriteLine(perfil.Mensaje);
