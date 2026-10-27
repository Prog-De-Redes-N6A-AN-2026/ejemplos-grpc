using System.Collections.Concurrent;

namespace GrpcServer;

public class AlmacenTokens
{
    private readonly ConcurrentDictionary<string, string> _tokens = new();

    public string Crear(string usuario)
    {
        string token = Guid.NewGuid().ToString();
        _tokens[token] = usuario;
        return token;
    }

    public bool IntentarObtenerUsuario(string token, out string usuario)
    {
        return _tokens.TryGetValue(token, out usuario!);
    }
}
