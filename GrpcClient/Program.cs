using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcErrores;

using var canal = GrpcChannel.ForAddress("http://localhost:5191");
var cliente = new Calculadora.CalculadoraClient(canal);

await ProbarDivision(cliente, 10, 2);
Console.WriteLine();

await ProbarDivision(cliente, 10, 0);
Console.WriteLine();

await ProbarDeadline(cliente);

static async Task ProbarDivision(
    Calculadora.CalculadoraClient cliente,
    double numerador,
    double denominador)
{
    Console.WriteLine($"Solicitando {numerador} / {denominador}");

    try
    {
        var respuesta = await cliente.DividirAsync(new SolicitudDivision
        {
            Numerador = numerador,
            Denominador = denominador
        });

        Console.WriteLine($"Resultado: {respuesta.Resultado}");
    }
    catch (RpcException ex)
    {
        Console.WriteLine("La llamada gRPC terminó con error.");
        Console.WriteLine($"StatusCode: {ex.StatusCode}");
        Console.WriteLine($"Detalle: {ex.Status.Detail}");
    }
}

static async Task ProbarDeadline(
    Calculadora.CalculadoraClient cliente)
{
    Console.WriteLine("Solicitando una operación que tarda 5 segundos.");
    Console.WriteLine("El cliente establece un deadline de 2 segundos.");

    try
    {
        var respuesta = await cliente.OperacionLentaAsync(
            new Empty(),
            deadline: DateTime.UtcNow.AddSeconds(2));

        Console.WriteLine(respuesta.Mensaje);
    }
    catch (RpcException ex)
    {
        Console.WriteLine("La llamada gRPC terminó con error.");
        Console.WriteLine($"StatusCode: {ex.StatusCode}");
        Console.WriteLine($"Detalle: {ex.Status.Detail}");
    }
}
