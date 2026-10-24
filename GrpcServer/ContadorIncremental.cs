namespace GrpcServer
{
    public class ContadorIncremental
    {
        private int _valor;
        private readonly object _bloqueo = new object();

        public int Valor
        {
            get
            {
                lock (_bloqueo)
                {
                    return _valor;
                }
            }
        }

        public void Incrementar(int cantidad)
        {
            lock (_bloqueo)
            {
                _valor += cantidad;
                Monitor.PulseAll(_bloqueo);
            }
        }

        public void EsperarCambio(int valorAnterior)
        {
            lock (_bloqueo)
            {
                while (_valor == valorAnterior)
                {
                    Monitor.Wait(_bloqueo);
                }
            }
        }
    }
}
