namespace GrpcServer
{
    public class ConsolaProgreso
    {
        public static int UltimoProgreso = 1;
        public static readonly object Bloqueo = new object();

        public static void IniciarProgreso()
        {
            while (UltimoProgreso > 0)
            {
                Console.WriteLine("¿Cuántos minutos quedan?");
                string? entrada = Console.ReadLine();

                if (!int.TryParse(entrada, out int progreso))
                {
                    Console.WriteLine("Ingrese un número válido.");
                    continue;
                }

                lock (Bloqueo)
                {
                    UltimoProgreso = progreso;
                    Monitor.PulseAll(Bloqueo);
                }
            }

            Console.WriteLine("¡Finalizado! Presione Enter para salir.");
            Console.ReadLine();
        }
    }
}
