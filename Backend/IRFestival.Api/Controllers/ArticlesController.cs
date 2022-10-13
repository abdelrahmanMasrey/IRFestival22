using System.Net;

using Microsoft.AspNetCore.Mvc;

using IRFestival.Api.Data;
using IRFestival.Api.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Azure.Cosmos;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Linq;

namespace IRFestival.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private CosmosClient _cosmosCLient { get; set; }
        private Container _websiteArticlesContainer { get; set; }
   
        public ArticlesController(IConfiguration config)
        {

            _cosmosCLient = new CosmosClient(config.GetConnectionString("CosmosConnection"));
            _websiteArticlesContainer = _cosmosCLient.GetContainer("IRFestivalArticles", "WebsiteArticles");
            
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody]Article dummyArticle)
        {
          //  var article = new Article() { Id = new Guid().ToString(), Date = DateTime.Now };
            await _websiteArticlesContainer.CreateItemAsync(dummyArticle);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> GetUnpublishedArticles()
        {
            var result = new List<Article>();
            var query = _websiteArticlesContainer.GetItemLinqQueryable<Article>()
                .Where(p => p.Status == nameof(Status.Unpublished))
                .OrderBy(p => p.Date);
            var it = query.ToFeedIterator();
            while (it.HasMoreResults)
            {
                var resp = await it.ReadNextAsync();
                result = resp.ToList();
            }
            return Ok(result);
        }

    }
}