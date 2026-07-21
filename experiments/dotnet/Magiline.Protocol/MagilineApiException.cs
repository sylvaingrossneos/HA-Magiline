using System.Net;

namespace Magiline.Protocol;

public sealed class MagilineApiException : Exception
{
    public MagilineApiException(string endpoint, HttpStatusCode statusCode, string responseBody)
        : base($"La requête vers {endpoint} a échoué avec le statut {(int)statusCode} ({statusCode}).")
    {
        Endpoint = endpoint;
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }

    public string Endpoint { get; }
    public HttpStatusCode StatusCode { get; }
    public string ResponseBody { get; }
}
