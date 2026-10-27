# Ejemplos de gRPC

Ejemplos de **gRPC** con **C#** (`Grpc.AspNetCore` en el servidor y `Grpc.Net.Client` en el cliente),
definiendo los servicios con **Protocol Buffers** (`.proto`).

Cada carpeta tiene su propia solución con dos proyectos: un **servidor** (`GrpcServer`) y un **cliente** (`GrpcClient`).

| Carpeta | Servicio (`.proto`) | Qué muestra |
|---|---|---|
| `01-unary` | `Saludador` (`saludos.proto`) | Llamadas **unarias** (una solicitud, una respuesta): saludar, sumar dos números y obtener una lista de usuarios. Muestra `repeated`, `enum`, `optional` y el tipo `google.protobuf.Empty`. |
| `02-server-streaming` | `Progreso` (`progreso.proto`) | **Server streaming**: el cliente se suscribe y el servidor le va enviando los minutos restantes a medida que se ingresan por la consola del servidor. |
| `03-errores-deadlines` | `Calculadora` (`calculadora.proto`) | **Manejo de errores** con `RpcException` y `StatusCode` (dividir entre cero devuelve `InvalidArgument`) y **deadlines**: el cliente da 2 segundos a una operación que tarda 5 y recibe `DeadlineExceeded`. |
| `04-metadata` | `Autenticacion` (`autenticacion.proto`) | **Metadata** (headers): el cliente hace login, recibe un token y lo manda en el header `authorization: Bearer <token>` para acceder a un recurso protegido. |
| `05-completo` | `Contador` (`contador.proto`) | Los **cuatro tipos de llamada** sobre un contador compartido: unary, client streaming, server streaming y bidirectional streaming. |

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download). Los ejemplos `01`, `02` y `05` apuntan a `net8.0`, así que también hace falta el runtime de .NET 8 (o cambiar el `TargetFramework` de cada `.csproj`).

## Cómo ejecutarlos

Desde la carpeta de cada ejemplo, el servidor en una terminal y el cliente en otra:

```bash
dotnet run --project GrpcServer
dotnet run --project GrpcClient
```

El servidor escucha en `http://localhost:5191` (HTTP/2), salvo `02-server-streaming`, que usa `http://localhost:5266`.

- En `02-server-streaming`, después de que el cliente se conecte, se escriben los minutos restantes en la consola del **servidor**; con `0` termina.
- En `05-completo`, el cliente pregunta qué tipo de llamada probar (`0` a `3`).
