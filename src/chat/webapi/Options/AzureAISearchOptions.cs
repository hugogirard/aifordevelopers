// Copyright (c) Microsoft. All rights reserved.

namespace CopilotChat.WebApi.Options;

public class AzureAISearchOptions
{
    public const string PropertyName = "AzureAISearch";

    public string? Endpoint { get; set; } = string.Empty;

    public string? APIKey { get; set; } = string.Empty;

    public string? IndexName { get; set; } = string.Empty;
}
