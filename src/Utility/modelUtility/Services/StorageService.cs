using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace modelUtility.Services;

public class StorageService : IStorageService
{
    private BlobContainerClient _blobContainerClient;

    public StorageService(IConfiguration configuration)
    {
        var blobServiceClient = new BlobServiceClient(configuration["STORAGECNXSTRING"]);
        var containerName = configuration["CONTAINERNAME"];
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public string GetSasContainer()
    {
        var sasBuilder = new BlobSasBuilder
        {
            BlobContainerName = _blobContainerClient.Name,
            Resource = "c",
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1), // In case the model take longer to train
            Protocol = SasProtocol.Https,
        };

        sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

        var sasUri = _blobContainerClient.GenerateSasUri(sasBuilder);
        string sasToken = sasUri.ToString().Split('?')[1];

        return sasToken;
    }
}
