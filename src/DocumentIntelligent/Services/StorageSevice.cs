using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Configuration;

namespace DocumentIntelligent;

public class StorageSevice : IStorageSevice
{
    private BlobContainerClient _blobContainerClient;

    public StorageSevice(IConfiguration configuration)
    {
        var blobServiceClient = new BlobServiceClient(configuration["STORAGECNXSTRING"]);
        var containerName = configuration["CONTAINERNAME"];
        _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
    }

    public Uri GetSasBlobUrl(string blobName)
    {
        // Get a reference to the blob
        var blobClient = _blobContainerClient.GetBlobClient(blobName);

        // Define the permissions and expiry time for the SAS
        var blobSasBuilder = new BlobSasBuilder
        {
            StartsOn = DateTimeOffset.UtcNow.AddMinutes(-5),
            ExpiresOn = DateTimeOffset.UtcNow.AddHours(1), // Set the desired expiration time
            Resource = "b", // 'b' indicates a blob
            Protocol = SasProtocol.Https
        };

        blobSasBuilder.SetPermissions(BlobContainerSasPermissions.Read);

        return blobClient.GenerateSasUri(blobSasBuilder);
    }
}
