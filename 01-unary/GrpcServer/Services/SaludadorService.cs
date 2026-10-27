using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer;

namespace GrpcServer.Services
{
    public class SaludadorService : Saludador.SaludadorBase
    {
        private readonly ILogger<SaludadorService> _logger;

        public SaludadorService(ILogger<SaludadorService> logger)
        {
            _logger = logger;
        }

        public override Task<RespuestaSaludo> Saludar(SolicitudSaludo request, ServerCallContext context)
        {
            return Task.FromResult(new RespuestaSaludo
            {
                Mensaje = "Hola " + request.Nombre
            });
        }

        public override Task<RespuestaSuma> SumarNumeros(SolicitudSuma request, ServerCallContext context)
        {
            return Task.FromResult(new RespuestaSuma
            {
                Suma = request.NumeroA + request.NumeroB
            });
        }

        public override Task<RespuestaListaUsuarios> ObtenerListaUsuarios(
            Empty request,
            ServerCallContext context)
        {
            var respuesta = new RespuestaListaUsuarios();

            respuesta.Usuarios.Add(new Usuario
            {
                Nombre = "Juan",
                Edad = 22,
                Genero = Genero.Masculino
            });
            respuesta.Usuarios.Add(new Usuario
            {
                Nombre = "María",
                Edad = 12,
                Genero = Genero.Femenino
            });
            respuesta.Usuarios.Add(new Usuario
            {
                Nombre = "Pablo",
                Edad = 44,
                Genero = Genero.Masculino
            });

            respuesta.EdadMaxima = 44;

            return Task.FromResult(respuesta);
        }
    }
}
