using Grpc.Core;
using GrpcProgress;

namespace GrpcServer.Services
{
    public class ServicioProgreso : Progreso.ProgresoBase
    {
        private readonly ILogger<ServicioProgreso> _logger;

        public ServicioProgreso(ILogger<ServicioProgreso> logger)
        {
            _logger = logger;
        }

        public override async Task MostrarProgreso(
            Vacio request,
            IServerStreamWriter<TiempoRestante> responseStream,
            ServerCallContext context)
        {
            bool finalizado = false;

            while (!finalizado)
            {
                Monitor.Enter(ConsolaProgreso.Bloqueo);
                Monitor.Wait(ConsolaProgreso.Bloqueo);

                int progreso = ConsolaProgreso.UltimoProgreso;

                await responseStream.WriteAsync(new TiempoRestante
                {
                    MinutosRestantes = progreso
                });

                if (progreso == 0)
                {
                    finalizado = true;
                }

                Monitor.Exit(ConsolaProgreso.Bloqueo);
            }
        }
    }
}
