using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;

namespace trainModel.Services;

public class StorageService
{
    private readonly BlobContainerClient _containerClient;

    public StorageService(IConfiguration configuration) 
    {
        var blobServiceClien = new BlobServiceClient(configuration["StorageCnxString"]);
    }
            
    
}
