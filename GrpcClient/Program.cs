using Grpc.Core;
using Grpc.Net.Client;
using GrpcProgress;

namespace GrpcClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            using var canal = GrpcChannel.ForAddress("http://localhost:5266");
            var cliente = new Progreso.ProgresoClient(canal);

            Console.WriteLine("Presione Enter para mostrar el progreso en vivo.");
            Console.ReadLine();

            using var llamada = cliente.MostrarProgreso(new Vacio());

            Console.WriteLine("Comenzando a recibir actualizaciones...");

            await foreach (var mensaje in llamada.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Minutos restantes: {mensaje.MinutosRestantes}");
            }
        }
    }
}
