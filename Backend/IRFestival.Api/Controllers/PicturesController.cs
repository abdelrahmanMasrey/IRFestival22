using Azure.Messaging.EventGrid.SystemEvents;
using Azure.Messaging.ServiceBus;
using Azure.Storage.Blobs;
using IRFestival.Api.Common;
using IRFestival.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System;
using System.Net;
using System.Web;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PicturesController : ControllerBase
    {
        private BlobUtility BlobUtility { get; }
        private readonly IConfiguration config;
        public PicturesController(BlobUtility blobUtility, IConfiguration config)
        {
            BlobUtility = blobUtility;
            this.config = config;  
        }
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string[]))]
        public string[] GetAllPictureUrls()
        {
            var container = BlobUtility.GetPicturesContainer();
            //var container = BlobUtility.GetThumbsContainer();
            return container.GetBlobs()
                .Select(blob => BlobUtility.GetSasUri(container, blob.Name))
                .ToArray();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppSettingsOptions))]

        public async Task<ActionResult> PostPicture(IFormFile file)
        {
            BlobContainerClient container = BlobUtility.GetPicturesContainer();
          //  BlobContainerClient container = BlobUtility.GetThumbsContainer();
            var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{HttpUtility.UrlPathEncode(file.FileName.Replace(" ",""))}";
            await container.UploadBlobAsync(fileName, file.OpenReadStream());
            await using (var client = new ServiceBusClient(config.GetConnectionString("sender")))
            {
                //create a sender for queue
                ServiceBusSender sender = client.CreateSender(config.GetValue<string>("queueName"));

                //create a message that we can send
                ServiceBusMessage message = new ServiceBusMessage($"the picture {fileName} was uploaded");

                //send the message 
                await sender.SendMessageAsync(message);

            }
            return Ok();
        }
    }
}
