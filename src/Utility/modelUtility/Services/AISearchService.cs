using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace modelUtility.Services;

public class AISearchService : IAISearchService
{    
    private readonly IConfiguration _configuration;
    private readonly HttpClient _http;

    public AISearchService(IHttpClientFactory httpClientFactory,
                           IConfiguration configuration)
    {        
        _http = httpClientFactory.CreateClient();

        _http.DefaultRequestHeaders.Add("api-key", _configuration["AISEARCH_APIKEY"]);

        _configuration = configuration;
    }

    public async Task CreateIndexingResources()
    {
        await CreateDataSource();

        await CreateIndex();


    }

    private async Task CreateDataSource()
    {
        // Read the datasource file and replace the connection string value
        string json = await File.ReadAllTextAsync("AISearchConfiguration\\datasource.json");

        // Parse the JSON object
        JObject jsonObject = JObject.Parse(json);

        // Replace the connection string value
        string newConnectionString = _configuration["STORAGECNXSTRING"];
        jsonObject["credentials"]["connectionString"] = newConnectionString;
        jsonObject["name"] = "test";

        string uri = $"{_configuration["AISEARCH_ENDPOINT"]}/datasources?api-version={_configuration["AISEARCH_VERSION"]}";

        await CallRestEndpoint(uri, jsonObject);
    }

    private async Task CreateIndex() 
    {
        string json = await File.ReadAllTextAsync("AISearchConfiguration\\index.json");
        JObject jsonObject = JObject.Parse(json);

        jsonObject["name"] = "test";

        string uri = $"{_configuration["AISEARCH_ENDPOINT"]}/indexes?api-version={_configuration["AISEARCH_VERSION"]}";

        await CallRestEndpoint(uri, jsonObject);
    }

    private async Task CreateSkillSet()
    {
        string json = await File.ReadAllTextAsync("AISearchConfiguration\\skilletset.json");
        JObject jsonObject = JObject.Parse(json);

        jsonObject["name"] = "test";

        string uri = $"{_configuration["AISEARCH_ENDPOINT"]}/indexes?api-version={_configuration["AISEARCH_VERSION"]}";

        await CallRestEndpoint(uri, jsonObject);
    }

    private async Task CallRestEndpoint(string uri, JObject jsonObject) 
    {
        var request = new HttpRequestMessage()
        {
            Method = HttpMethod.Post,
            RequestUri = new Uri(uri)
        };

        request.Content = new StringContent(jsonObject.ToString(), Encoding.UTF8, "application/json");
        var response = await _http.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {

        }
        else
        {
            string error = await response.Content.ReadAsStringAsync();
            throw new Exception(error);
        }
    }
}
