using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcAutenticacion;

namespace GrpcServer.Services;

public class ServicioAutenticacion : Autenticacion.AutenticacionBase
{
    private readonly AlmacenTokens _tokens;

    public ServicioAutenticacion(AlmacenTokens tokens)
    {
        _tokens = tokens;
    }

    public override Task<RespuestaLogin> IniciarSesion(
        SolicitudLogin request,
        ServerCallContext context)
    {
        if (request.Usuario != "usuario" || request.Contrasena != "clave")
        {
            throw new RpcException(
                new Status(
                    StatusCode.Unauthenticated,
                    "Usuario o contraseña incorrectos."));
        }

        string token = _tokens.Crear(request.Usuario);

        return Task.FromResult(new RespuestaLogin
        {
            Token = token,
            Mensaje = "Autenticación correcta."
        });
    }

    public override Task<RespuestaPerfil> ObtenerPerfil(
        Empty request,
        ServerCallContext context)
    {
        string token = ObtenerBearerToken(context);

        if (!_tokens.IntentarObtenerUsuario(token, out string usuario))
        {
            throw new RpcException(
                new Status(
                    StatusCode.Unauthenticated,
                    "Token inválido."));
        }

        return Task.FromResult(new RespuestaPerfil
        {
            Usuario = usuario,
            Mensaje = $"Datos privados de {usuario}."
        });
    }

    private static string ObtenerBearerToken(ServerCallContext context)
    {
        var entrada = context.RequestHeaders
            .FirstOrDefault(header => header.Key == "authorization");

        if (entrada is null ||
            !entrada.Value.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            throw new RpcException(
                new Status(
                    StatusCode.Unauthenticated,
                    "Falta el token de autenticación."));
        }

        return entrada.Value["Bearer ".Length..].Trim();
    }
}
