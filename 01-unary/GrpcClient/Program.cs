using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using GrpcServer;

namespace GrpcClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Creando cliente gRPC");

            using var canal = GrpcChannel.ForAddress("http://localhost:5191");
            var cliente = new Saludador.SaludadorClient(canal);

            var saludo = await cliente.SaludarAsync(
                new SolicitudSaludo { Nombre = "Franco" });

            Console.WriteLine("Saludo: " + saludo.Mensaje);

            var suma = await cliente.SumarNumerosAsync(
                new SolicitudSuma { NumeroA = 4, NumeroB = 7 });

            Console.WriteLine("Suma: " + suma.Suma);

            RespuestaListaUsuarios respuestaUsuarios =
                await cliente.ObtenerListaUsuariosAsync(new Empty());

            foreach (Usuario usuario in respuestaUsuarios.Usuarios)
            {
                Console.WriteLine($"{usuario.Nombre} tiene {usuario.Edad} años");
            }

            if (respuestaUsuarios.HasEdadMaxima)
                Console.WriteLine($"La edad máxima fue establecida en {respuestaUsuarios.EdadMaxima}");
            else
                Console.WriteLine("La edad máxima no fue establecida");

            Console.ReadLine();
        }
    }
}
