using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcServer;

namespace GrpcClient
{
    internal class Program
    {
        private static readonly Random Aleatorio = new Random();

        static async Task Main(string[] args)
        {
            Console.WriteLine("Creando cliente gRPC");

            using var canal = GrpcChannel.ForAddress("http://localhost:5191");
            var cliente = new Contador.ContadorClient(canal);

            Console.WriteLine("Seleccione el tipo de llamada:");
            Console.WriteLine("- 0 = unary");
            Console.WriteLine("- 1 = client streaming");
            Console.WriteLine("- 2 = server streaming");
            Console.WriteLine("- 3 = bidirectional streaming");

            string? entrada = Console.ReadLine();

            if (!int.TryParse(entrada, out int tipo))
            {
                Console.WriteLine("No se pudo interpretar la opción.");
                return;
            }

            switch (tipo)
            {
                case 0:
                    await EjemploLlamadaUnaria(cliente);
                    break;
                case 1:
                    await EjemploStreamingCliente(cliente);
                    break;
                case 2:
                    await EjemploStreamingServidor(cliente);
                    break;
                case 3:
                    await EjemploStreamingBidireccional(cliente);
                    break;
                default:
                    Console.WriteLine("Opción inválida.");
                    break;
            }

            Console.ReadLine();
        }

        private static async Task EjemploLlamadaUnaria(Contador.ContadorClient cliente)
        {
            var respuesta = await cliente.IncrementarAsync(new Empty());
            Console.WriteLine("Valor: " + respuesta.Valor);
        }

        private static async Task EjemploStreamingCliente(Contador.ContadorClient cliente)
        {
            using var llamada = cliente.Acumular();

            for (var i = 0; i < 3; i++)
            {
                var cantidad = Aleatorio.Next(5);

                Console.WriteLine($"Enviando incremento de {cantidad}");

                await llamada.RequestStream.WriteAsync(
                    new SolicitudContador { Cantidad = cantidad });

                await Task.Delay(TimeSpan.FromSeconds(2));
            }

            await llamada.RequestStream.CompleteAsync();

            var respuesta = await llamada;
            Console.WriteLine($"Valor final: {respuesta.Valor}");
        }

        private static async Task EjemploStreamingServidor(Contador.ContadorClient cliente)
        {
            using var llamada = cliente.CuentaRegresiva(new Empty());

            await foreach (var mensaje in llamada.ResponseStream.ReadAllAsync())
            {
                Console.WriteLine($"Cuenta regresiva: {mensaje.Valor}");
            }
        }

        private static async Task EjemploStreamingBidireccional(
            Contador.ContadorClient cliente)
        {
            using var llamada = cliente.ContadorEnVivo();

            Console.WriteLine("Comenzando a recibir actualizaciones");

            var tareaLectura = Task.Run(async () =>
            {
                await foreach (var respuesta in llamada.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"Valor en vivo: {respuesta.Valor}");
                }
            });

            while (true)
            {
                Console.WriteLine(
                    "Ingrese un número para sumar o presione Enter para salir:");

                var entrada = Console.ReadLine();

                if (string.IsNullOrEmpty(entrada))
                {
                    Console.WriteLine("Saliendo...");
                    break;
                }

                if (int.TryParse(entrada, out var cantidad))
                {
                    Console.WriteLine($"Sumando {cantidad}");

                    await llamada.RequestStream.WriteAsync(
                        new SolicitudContador { Cantidad = cantidad });
                }
                else
                {
                    Console.WriteLine("Entrada inválida.");
                }
            }

            await llamada.RequestStream.CompleteAsync();
            await tareaLectura;
        }
    }
}
