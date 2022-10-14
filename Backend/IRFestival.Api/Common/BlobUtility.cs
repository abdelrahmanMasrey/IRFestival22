﻿using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Sas;
using Microsoft.Extensions.Options;

namespace IRFestival.Api.Common
{
    public class BlobUtility
    {
        private StorageSharedKeyCredential Credential { get; }
        private BlobServiceClient Client { get; }
        private BlobSettingsOptions Options { get; }

        public BlobUtility(StorageSharedKeyCredential credential,
            BlobServiceClient client, IOptions<BlobSettingsOptions> options)
        {
            Credential = credential;
            Client = client;
            Options = options.Value;

        }
        public BlobContainerClient GetPicturesContainer()
            => Client.GetBlobContainerClient(Options.PicturesContainer);
        public BlobContainerClient GetThumbsContainer()
         => Client.GetBlobContainerClient(Options.ThumbsContainer);
        public string GetSasUri(BlobContainerClient container, string blobName)
        {
            BlobSasBuilder sasBuilder = new BlobSasBuilder()
            {
                BlobContainerName = container.Name,
                BlobName = blobName,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddMinutes(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddMinutes(2),
            };
            sasBuilder.SetPermissions(BlobContainerSasPermissions.Read);
            //use the key to get the sus token
            string sasToken = sasBuilder.ToSasQueryParameters(Credential).ToString();
            return $"{container.Uri.AbsoluteUri}/{blobName}?{sasToken}";
        }
    }
}
