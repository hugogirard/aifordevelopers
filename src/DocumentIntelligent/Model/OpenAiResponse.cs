using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DocumentIntelligent.Model;

public class OpenAiResponse
{
    [JsonPropertyName("data")]
    public List<DataOpenAi> Data { get; set; }
}

public class  DataOpenAi
{
    [JsonPropertyName("embedding")]
    public List<float> Embedding { get; set; }
}
