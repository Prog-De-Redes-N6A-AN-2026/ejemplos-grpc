using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcServer;

namespace GrpcServer.Services
{
    public class ServicioContador : Contador.ContadorBase
    {
        private readonly ILogger<ServicioContador> _logger;
        private readonly ContadorIncremental _contador;

        public ServicioContador(
            ContadorIncremental contador,
            ILogger<ServicioContador> logger)
        {
            _contador = contador;
            _logger = logger;
        }

        public override Task<RespuestaContador> Incrementar(
            Empty request,
            ServerCallContext context)
        {
            _logger.LogInformation("Incrementando el contador en 1");
            _contador.Incrementar(1);

            return Task.FromResult(new RespuestaContador
            {
                Valor = _contador.Valor
            });
        }

        public override async Task<RespuestaContador> Acumular(
            IAsyncStreamReader<SolicitudContador> requestStream,
            ServerCallContext context)
        {
            await foreach (var mensaje in requestStream.ReadAllAsync())
            {
                _logger.LogInformation(
                    "Incrementando el contador en {Cantidad}",
                    mensaje.Cantidad);

                _contador.Incrementar(mensaje.Cantidad);
            }

            return new RespuestaContador
            {
                Valor = _contador.Valor
            };
        }

        public override async Task CuentaRegresiva(
            Empty request,
            IServerStreamWriter<RespuestaContador> responseStream,
            ServerCallContext context)
        {
            for (var i = _contador.Valor; i >= 0; i--)
            {
                await responseStream.WriteAsync(
                    new RespuestaContador { Valor = i });

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public override async Task ContadorEnVivo(
            IAsyncStreamReader<SolicitudContador> requestStream,
            IServerStreamWriter<RespuestaContador> responseStream,
            ServerCallContext context)
        {
            _ = Task.Run(async () =>
            {
                await foreach (var mensaje in requestStream.ReadAllAsync())
                {
                    _logger.LogInformation(
                        "Incrementando el contador en {Cantidad}",
                        mensaje.Cantidad);

                    _contador.Incrementar(mensaje.Cantidad);
                }
            });

            var ultimoEnviado = -1;

            while (!context.CancellationToken.IsCancellationRequested)
            {
                _contador.EsperarCambio(ultimoEnviado);

                var actual = _contador.Valor;

                if (actual != ultimoEnviado)
                {
                    await responseStream.WriteAsync(
                        new RespuestaContador { Valor = actual });

                    ultimoEnviado = actual;
                }
            }
        }
    }
}
