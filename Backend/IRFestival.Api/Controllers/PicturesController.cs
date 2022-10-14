using Azure.Storage.Blobs;
using IRFestival.Api.Common;
using IRFestival.Api.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public PicturesController(BlobUtility blobUtility)
        {
            BlobUtility = blobUtility;
        }
        [HttpGet]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string[]))]
        public string[] GetAllPictureUrls()
        {
           // var container = BlobUtility.GetPicturesContainer();
            var container = BlobUtility.GetThumbsContainer();
            return container.GetBlobs()
                .Select(blob => BlobUtility.GetSasUri(container, blob.Name))
                .ToArray();
        }

        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(AppSettingsOptions))]

        public async Task<ActionResult> PostPicture(IFormFile file)
        {
            BlobContainerClient container = BlobUtility.GetThumbsContainer();
            var fileName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{HttpUtility.UrlPathEncode(file.FileName.Replace(" ",""))}";
            await container.UploadBlobAsync(fileName, file.OpenReadStream());
            return Ok();
        }
    }
}
