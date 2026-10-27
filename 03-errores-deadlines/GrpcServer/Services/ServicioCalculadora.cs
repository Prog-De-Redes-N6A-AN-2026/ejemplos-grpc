using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using GrpcErrores;

namespace GrpcServer.Services;

public class ServicioCalculadora : Calculadora.CalculadoraBase
{
    public override Task<RespuestaDivision> Dividir(
        SolicitudDivision request,
        ServerCallContext context)
    {
        if (request.Denominador == 0)
        {
            throw new RpcException(
                new Status(
                    StatusCode.InvalidArgument,
                    "No se puede dividir entre cero."));
        }

        return Task.FromResult(new RespuestaDivision
        {
            Resultado = request.Numerador / request.Denominador
        });
    }

    public override async Task<RespuestaOperacion> OperacionLenta(
        Empty request,
        ServerCallContext context)
    {
        Console.WriteLine($"OperacionLenta iniciada. Deadline: {context.Deadline:O}");

        await Task.Delay(
            TimeSpan.FromSeconds(5),
            context.CancellationToken);

        return new RespuestaOperacion
        {
            Mensaje = "La operación lenta terminó correctamente."
        };
    }
}
