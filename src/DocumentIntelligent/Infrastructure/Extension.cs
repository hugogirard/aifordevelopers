using Microsoft.Azure.Functions.Worker.Http;
using System.Net;
using System.Text.Json;

namespace DocumentIntelligent;

public static class Extension
{
    public static HttpResponseData CreateBadRequestResponse(this HttpRequestData req, string message) 
    {
        var response = req.CreateResponse(System.Net.HttpStatusCode.BadRequest);
        response.Headers.Add("Content-Type", "text/plain; charset=utf-8");
        response.WriteString(message);
        return response;
    }

    public static HttpResponseData CreateOkRequest<T>(this HttpRequestData req, T payload) where T : class
    {
        var response = req.CreateResponse(HttpStatusCode.OK);
        response.Headers.Add("Content-Type", "application/json; charset=utf-8");
        response.WriteString(JsonSerializer.Serialize(payload));

        return response;
    }
}
